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
        private List<IActions> Response = new List<IActions>(); 
        private List<UnitPresset> AddSelections = new List<UnitPresset>();
        private List<UnitPresset> AddTargets = new List<UnitPresset>();
        private int idx = 0;
        private List<UnitPresset> ChangeUnits = new List<UnitPresset>();
        private List<(int index,List<UnitPresset> Units, List<Building> buildings)> PrevState;
        private IListOfToken field;
        private List<UnitPresset> units = new List<UnitPresset>();
        private List<Building> buildings = new List<Building>();
        private PathField pathField;
        private GameModeState _State;
        private List<UnitPresset> PossibleTargets = null;
        private List<UnitPresset> UnitsInBattle = new List<UnitPresset>();
        private Player CurrentPlayer = Player.getPlayer(0);
        private bool isHotSeat = true;
        private int _ActionIdx = -1;

         
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

        public PathToken GetPathToken(UnitPresset unit ,(int X, int Y) fpos)
        {
            var pathTokens = getWalkArea(unit);
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

        public event OnUnitsListChange UnitsListChanged;

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
            ChangeUnits.Add(unit);
            if (Attack.AbilityType == AbilityType.RangeAttack)
            {
                ChangeUnits.Add(target);
                dealDamage.Source = unit.fieldPosition;
                dealDamage.Destination = target.fieldPosition;
                dealDamage.idx = AbilityIdx;
                Response.Add(dealDamage);
                Attack.Use(target);
                return;
            }
            

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
                    if (stand.CouldToReact(unit, target))
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
                    stand.Use(unit, target);
                }
            }
            if (unit.currentHp > 0)
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
                    stand.Use(unit, target);
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
                    response.Use(unit);
                    response.actionPoint.Return(target.owner);
                }
                    
            }

            foreach (var lunit in units)
            {
                if (lunit.currentHp < 0)
                {

                }
            }
            UnitsInBattle.Clear();
        }

        public List<PathToken> getWalkArea(UnitPresset unit)
        {
            pathField.Refresh();
            return pathField.getWalkArea(unit.currentSpeed, unit, GetUnits());
        }

        public void Move(UnitPresset unit, PathToken pathToken)
        {
            if(unit.MoveActionPoint.Active(unit.owner) || unit.MoveActionPoint.State == ActionState.InProcess)
            {
                MoveUnit moveUnit = new MoveUnit();
                moveUnit.StartPosition = unit.fieldPosition;
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

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            CreateUnit createUnit = new CreateUnit(name, fpos, owner.idx);
            var listResponse = new List<IActions>();
            listResponse.Add(createUnit);
            Response = listResponse;
            ProcessActions(Response);
        }

        public object ProcessRequset(object sender)
        {
            Response.Clear();
            if (State == GameModeState.Standart)
            {
                if (sender is RequestContainer request)
                {
                    if (request.Actions != null)
                        ProcessActions(request.Actions);
                    if (request.Type == RequestType.MoveUnit)
                    {
                        var unit = GetUnit(request.Selected);
                        var pathToken = GetPathToken(unit, request.Target);
                        Move(unit, pathToken);
                        ProcessActions(Response);
                    }
                    if (request.Type == RequestType.UseAbility)
                    {
                        var unit = GetUnit(request.Selected);
                        var ability = unit.GetAbility(request.AbilityIdx);
                        if (ability.AbilityType != AbilityType.Attack)
                        {
                            UseAction useAction = new UseAction();
                            useAction.Source = unit.fieldPosition;
                            useAction.SourceAbility = ability.idx;
                            Response.Add(useAction);
                        }
                        else
                        {
                            var target = GetUnit(request.Target);
                            AttackUnit(unit, target, ability.idx);
                        }
                    }
                    if (request.Type == RequestType.CreateUnit)
                    {
                        CreateUnit(request.Name, request.Selected, Player.getPlayer(request.Player));
                    }
                    RequestContainer applyChangesRequest = new RequestContainer(RequestType.ApplyChanges);
                    applyChangesRequest.Actions = Response;
                    return applyChangesRequest;
                }

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

        public void BacklightTargets(UnitPresset unit, AbilityPresset ability)
        {
            var targets = pathField.getListOfTargets(unit, ability.DeafaultRange, GetUnits());
            foreach (var target in targets)
            {
                target.isTarget = true;
            }
        }

        public void RefreshBacklight()
        {
            foreach ( var unit in units)
            {
                unit.isTarget = false;
            }
        }

        public void AddUnit(UnitPresset unitPresset)
        {
            units.Add(unitPresset);
            UnitsListChanged?.Invoke(unitPresset, true);
        }

        public void DeleteUnit(UnitPresset unitPresset)
        {
            units.Remove(unitPresset);
            UnitsListChanged?.Invoke(unitPresset, false);
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
