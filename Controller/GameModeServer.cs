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
        private UnitPresset target; 
        private List<UnitPresset> AddSelections = new List<UnitPresset>();
        private List<UnitPresset> AddTargets = new List<UnitPresset>();

        private List<PathToken> pathTokens;
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

        public AbilityPresset selectedAbility;

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
            foreach (var token in pathTokens)
            {
                if (token.fieldPosition == fpos)
                    return token;
            }
            return null;
        }

        public UnitPresset getUnit((int X, int Y) fpos)
        {
            foreach (var unit in units)
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

        public void AttackUnit(UnitPresset target)
        {
            UnitsInBattle.Clear();
            var Attack = selectedAbility;
            if (Attack.AbilityType == AbilityType.RangeAttack)
            {
                Attack.Use(target);
                selectedAbility = null;
                State = GameModeState.AwaitSelect;
                return;
            }

            AbilityPresset Response =null;
            foreach (var ability in target.Abilities)
            {
                if (ability.Name == "Melee")
                {
                    Response = ability;
                }
            }
            List<StandPresset> listStands = new List<StandPresset>();
            foreach (var unit in units)
            {
                foreach (var stand in unit.Stands)
                {
                    if (stand.CouldToReact(Selected, target))
                    {
                        listStands.Add(stand);
                        UnitsInBattle.Add(unit);
                    }
                }
            }
            foreach (var stand in listStands)
            {
                if (stand.AbilityType == AbilityType.PreemptiveAttack)
                    stand.Use(Selected, target);

            }
            if (Selected.currentHp > 0)
                Attack.Use(target);
            foreach (var stand in listStands)
            {
                if (stand.AbilityType == AbilityType.Attack)
                    stand.Use(Selected, target);
            }
            if( target.currentHp > 0)
            {
                if (Response != null)
                    Response.Use(Selected);
            }

            foreach (var unit in units)
            {
                if (unit.currentHp < 0)
                {

                }
            }
            selectedAbility = null;
            UnitsInBattle.Clear();
            State = GameModeState.AwaitSelect;
        }

        public List<PathToken> getWalkArea()
        {
            pathField.Refresh();
            return pathField.getWalkArea(Selected.currentSpeed, Selected, GetUnits());
        }

        public void MoveUnit(PathToken pathToken)
        {
            if(Selected.MoveActionPoint.Active(Selected.owner) || Selected.MoveActionPoint.State == ActionState.InProcess)
            {
                State = GameModeState.MoveMode;
                var unit = Selected;
                var distance = pathToken.pathLeght;
                unit.Move(pathToken.fieldPosition, distance);
                pathField.Refresh();
                unit.fieldPosition = pathToken.fieldPosition;
                GameTableController.Get().State = GameTableState.AwaitSelectAbility;
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

        public bool SelectUnit(UnitPresset token)
        {
            if (State == GameModeState.AwaitSelect)
            {
                if (token.owner != CurrentPlayer)
                    return false;
                if ( Selected == null)
                {
                    Selected = token;
                    Selected.isSelected = true;
                    State = GameModeState.MoveMode;
                    return true;
                }

                if ( Selected == token)
                { 
                    token.isSelected = false; 
                    Selected= null; 
                    return false; 
                }
                Selected.isSelected = false;
                Selected = token;
                Selected.isSelected = true;
                State = GameModeState.MoveMode;
                return true;
            }
            return false;
        }

        public void SelectedUnitRaiseStand(StandPresset stand)
        {
            stand.UpStand();
    }

        public void SelectedUnitActivateAbility(AbilityPresset ability)
        {
            ability.PrepareToUse();
        }

        public static GameModeServer Get()
        {
            return instance;
        }

        public void ParseJsonRequset(DataOfReqest data)
        {

            if (data.Selected != null)
            {
                Selected = getUnit(data.Selected.fieldPosition);
            }
            else
            {
                Selected.isSelected = false;
                Selected = getUnit(data.Selected.fieldPosition);
            }

            switch ((data.requst))
            {
                case RequstTypes.AttackUnit:
                    {
                        if (data.Target != null)
                            AttackUnit(getUnit(data.Target.fieldPosition));
                            break;
                    }
                case RequstTypes.MoveUnit:
                    {
                        pathField.Refresh();
                        pathTokens = pathField.getWalkArea(Selected.currentSpeed, Selected, GetUnits());
                        MoveUnit(GetPathToken(data.pathToken.fieldPosition));
                        break;
                    }
            }

            
        }

        
    }

    public delegate void GameModeEventHandler(UnitPresset sender, UnitPresset target, GameModEventArgs e);

    public enum GameModeState
    {
        AwaitSelect = 0,
        AtackMode = 1,
        AwaitEnemyEnd = 2,
        MoveMode = 3,
        MultipleSelect = 4,
        MovingMode = 5,
        SelectEnemy = 6,
    }

    public class GameModEventArgs
    {

    }
}
