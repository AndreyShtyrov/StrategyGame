using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Tokens;
using InterfaceOfObjects;
using DrawField;
using UnitsAnPathFinding;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;
using Controller.Units;
using Controller.Actions;
using Controller.Requests;


namespace Controller
{
    public partial class GameModeServer : IGameMode
    {
        private static GameModeServer instance;
        public UnitPresset Selected
        {   set
            { 
                _Selected = value;
                OnPropertyChanged("Selected");
            }
            get { return _Selected; }
        }
        private List<IActions> Response = new List<IActions>(); 
        private List<UnitPresset> AddSelections = new List<UnitPresset>();
        private List<UnitPresset> AddTargets = new List<UnitPresset>();
        private int idx = 0;
        private List<UnitPresset> PrevUnits;
        private List<UnitPresset> ChangeUnits;
        private List<(int index,List<UnitPresset> Units, List<Building> buildings)> PrevState;
        private IListOfToken field;
        private List<UnitPresset> units = new List<UnitPresset>();
        private List<Building> buildings = new List<Building>();
        private PathField pathField;
        private GameModeState _State;
        private List<UnitPresset> PossibleTargets = null;
        private UnitPresset _Selected;
        private List<UnitPresset> UnitsInBattle = new List<UnitPresset>();
        private Player CurrentPlayer = Player.getPlayer(0);
        private bool isHotSeat = true;
        private int _ActionIdx = -1;


        public AbilityPresset selectedAbility
        { get; set; }
         
        public GameModeState State
        {
            set
            {
                _State = value;
            }
            get
            {
                return _State;
            }
        }

        public int ActionIdx => _ActionIdx+1;

        public event PropertyChangedEventHandler PropertyChanged;

        public GameModeServer(Field field)
        {
            this.field = field;
            pathField = new PathField(field);
        }

        public ITokenData getToken((int X, int Y) fpos)
        {
            return field.getData(fpos);
        }

        public PathToken GetPathToken((int X, int Y) fpos)
        {
            var unit = GetUnit(fpos);
            var pathTokens = pathField.getWalkArea(unit.currentSpeed, unit, GetUnits());
            foreach (var token in pathTokens)
            {
                if (token.fieldPosition == fpos)
                    return token;
            }
            return null;
        }

        public UnitPresset GetUnit((int X, int Y) fpos)
        {
            foreach (var unit in units)
            {
                if (unit.fieldPosition == fpos)
                    return unit;
            }
            return null;
        }

        public UnitPresset getPrevUnit((int X, int Y) fpos)
        {
            foreach (var unit in PrevUnits)
            {
                if (unit.fieldPosition == fpos)
                    return unit;
            }
            return null;
        }

        public UnitPresset[,] GetUnits()
        {
            UnitPresset[,] result = new UnitPresset[field.height, field.width];
            foreach (var unit in units)
            {
                result[unit.fieldPosition.X, unit.fieldPosition.Y] = unit;
            }
            return result;
        }

        public void SwitchTrun()
        {
            
            CurrentPlayer.getIncome();
            if (CurrentPlayer.idx == 0)
                CurrentPlayer = Player.getPlayer(1);
            else
                CurrentPlayer = Player.getPlayer(0);
            foreach (var unit in units)
            {
                if (unit.owner == CurrentPlayer)
                {
                    unit.Refresh();
                }
            }
        }

        public UnitPresset CreateUnit( string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            if (name == "Helbard")
            {
                Halberd unit = new Halberd(fpos, owner);
                units.Add(unit);
                return unit;
            }
            if (name == "LongBow")
            {
                LongBow unit = new LongBow(fpos, owner);
                units.Add(unit);
                return unit;
            }
            return null;
        }

        public (int X, int Y, int Z) TransformToCube((int X, int Y) fpos, (int X, int Y) center)
        {
            if (center.Y % 2 == 0)
            {
                int row = fpos.X - center.X;
                int col = fpos.Y - center.Y;
                int x = row - (int)(col + (col & 1)) / 2;
                int z = col;
                int y = -x - z;
                return (x, y, z);
            }
            else
            {
                int row = fpos.X - center.X;
                int col = fpos.Y - center.Y;
                int x = row - (col - (col & 1)) / 2;
                int z = col;
                int y = -x - z;
                return (x, y, z);
            }
        }

        public (int X, int Y) TransformToAxial((int X, int Y, int Z) fpos, (int X, int Y) center)
        {
            (int X, int Y) result;
            if ( center.Y % 2 == 0)
            {
                result.X = fpos.X + (fpos.Z + (fpos.Z & 1)) / 2;
                result.Y = fpos.Z;
            }
            else
            {
                result.X = fpos.X + (fpos.Z - (fpos.Z & 1)) / 2;
                result.Y = fpos.Z;
            }
            return (result.X + center.X, result.Y + center.Y);
        }

        public bool SpendResources(int action, int move, Player owner)
        {
            
            if (owner.AttackPoints - action >= 0 && owner.MovePoints - move >= 0)
            {
                owner.AttackPoints -= action;
                owner.MovePoints -= move;
                return true;
            }
            return false;
        }

        public void ReturnResources(int action, int move, Player owner)
        {
            owner.AttackPoints += action;
            owner.MovePoints += move;
        }

        public List<UnitPresset> getUnitsInBattle()
        {
            return UnitsInBattle;
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void AttackUnit(UnitPresset unit, UnitPresset target, int AbilityIdx)
        {
            UnitsInBattle.Clear();
            var Attack = unit.GetAbility(AbilityIdx);
            DealDamage dealDamage = new DealDamage();
            ChangeUnits.Add(Selected);
            if (Attack.AbilityType == AbilityType.RangeAttack)
            {
                ChangeUnits.Add(target);
                selectedAbility = null;
                dealDamage.Source = unit.fieldPosition;
                dealDamage.Destination = target.fieldPosition;
                dealDamage.idx = AbilityIdx;
                Response.Add(dealDamage);
                Attack.Use(target);
                return;
            }
            Response.Add(dealDamage);

            AbilityPresset response = null;
            foreach (var ability in target.Abilities)
            {
                if (ability.Name == "Melee")
                {
                    response = ability;
                }
            }
            List<StandPresset> listStands = new List<StandPresset>();
            foreach (var lunit in units)
            {
                foreach (var stand in lunit.Stands)
                {
                    if (stand.CouldToReact(Selected, target))
                    {
                        listStands.Add(stand);
                        UnitsInBattle.Add(lunit);
                        ChangeUnits.Add(lunit);
                    }
                }
            }
            foreach (var stand in listStands)
            {
                if (stand.AbilityType == AbilityType.PreemptiveAttack)
                {
                    dealDamage = new DealDamage();
                    dealDamage.Source = unit.fieldPosition;
                    dealDamage.Destination = target.fieldPosition;
                    dealDamage.idx = stand.idx;
                    Response.Add(dealDamage);
                    stand.Use(Selected, target);
                }
            }
            if (Selected.currentHp > 0)
            {
                dealDamage.Source = unit.fieldPosition;
                dealDamage.Destination = target.fieldPosition;
                dealDamage.idx = AbilityIdx;
                Response.Add(dealDamage);
                Attack.Use(target);
            }
                
            ChangeUnits.Add(target);
            foreach (var stand in listStands)
            {
                if (stand.AbilityType == AbilityType.Attack)
                {
                    dealDamage = new DealDamage();
                    dealDamage.Source = unit.fieldPosition;
                    dealDamage.Destination = target.fieldPosition;
                    dealDamage.idx = stand.idx;
                    Response.Add(dealDamage);
                    stand.Use(Selected, target);
                }
                    
            }
            if( target.currentHp > 0)
            {
                if (response != null)
                {
                    dealDamage = new DealDamage();
                    dealDamage.Source = target.fieldPosition;
                    dealDamage.Destination = unit.fieldPosition;
                    dealDamage.idx = response.idx;
                    Response.Add(dealDamage);
                    response.Use(Selected);
                }
                    
            }

            foreach (var lunit in units)
            {
                if (lunit.currentHp < 0)
                {

                }
            }
            selectedAbility = null;
            UnitsInBattle.Clear();
        }

        public List<PathToken> getWalkArea()
        {
            pathField.Refresh();
            return pathField.getWalkArea(Selected.currentSpeed, Selected, GetUnits());
        }

        public void Move(UnitPresset unit, PathToken pathToken)
        {
            if(unit.MoveActionPoint.Active(unit.owner) || unit.MoveActionPoint.State == ActionState.InProcess)
            {
                var distance = pathToken.pathLeght;
                MoveUnit moveUnit = new MoveUnit();
                moveUnit.StartPosition = unit.fieldPosition;
                unit.Move(pathToken.fieldPosition, distance);
                moveUnit.EndPosition = pathToken.fieldPosition;
                pathField.Refresh();
                Response.Add(moveUnit);
            }
        }

        private void OnChangeOwner(Building sender, Player prevOwner)
        {
            if (sender.Type == BuildingTypes.AttackFlag)
            {
                sender.owner.IncomeAttackPoints += 1;
                prevOwner.IncomeAttackPoints -= 1;
            }
            if (sender.Type == BuildingTypes.MoveFlag)
            {
                sender.owner.IncomeAttackPoints += 1;
                prevOwner.IncomeAttackPoints -= 1;
            }
        }

        public void CreateBuilding(Building build)
        {
            buildings.Add(build);
            build.ChangeOwner += OnChangeOwner;
        }

        public void SelectedUnitRaiseStand(StandPresset stand)
        {
            stand.UpStand();
        }

        public void SelectedUnitActivateAbility(AbilityPresset ability)
        {
            ability.PrepareToUse();
        }

        public object ProcessRequset(object sender)
        {
            if (State == GameModeState.Standart)
            {
                if (sender is MoveUnitRequest moveRequest)
                {
                    var unit = GetUnit(moveRequest.Selected);
                    var pathToken = GetPathToken(moveRequest.Target);
                    Move(unit, pathToken);
                }
                if (sender is UseAbilityRequest abilityRequest)
                {
                    ProcessActions(abilityRequest.Actions);
                    var unit = GetUnit(abilityRequest.Selected);
                    var ability = unit.GetAbility(abilityRequest.AbilityIdx);
                    if (ability.AbilityType != AbilityType.Attack)
                    {
                        UseAction useAction = new UseAction();
                        useAction.Source = unit.fieldPosition;
                        useAction.SourceAbility = ability.idx;
                        Response.Add(useAction);
                    }
                    else
                    {
                        var target = GetUnit(abilityRequest.Target);
                        AttackUnit(unit, target, ability.idx);
                    }
                }
                ApplyChangesRequest applyChangesRequest = new ApplyChangesRequest();
                applyChangesRequest.Actions = Response;
                return applyChangesRequest;
            }

            return null;
        }

        public void ProcessActions(List<IActions> actions)
        {
            foreach (var action in actions)
            {
                action.forward();
            }
        }

        public bool SelectUnit(UnitPresset unit)
        {
            throw new NotImplementedException();
        }
    }

    public delegate void GameModeEventHandler(UnitPresset sender, UnitPresset target, GameModEventArgs e);

    public enum GameModeState
    {
        Standart = 0,
        AwaitResponse = 1,
    }

    public class GameModEventArgs
    {

    }
}
