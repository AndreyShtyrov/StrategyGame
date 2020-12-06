using System;
using System.Collections.Generic;
using System.Text;
using Tokens;
using InterfaceOfObjects;
using System.ComponentModel;
using UnitsAnPathFinding;
using Controller.Abilities;
using Controller.Actions;
using Controller.Units;

namespace Controller
{
    class GameMode: IGameMode
    {
        private static GameModeServer instance;
        public UnitPresset Selected
        {
            set
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
        private List<(int index, List<UnitPresset> Units, List<Building> buildings)> PrevState;
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
        private AbilityPresset _selectedAbility;

        public AbilityPresset selectedAbility
        { 
            get
            {
                return _selectedAbility;
            }
            set
            {
                _selectedAbility = value;
                OnPropertyChanged("selectedAbility");
            } 
        }

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

        public int ActionIdx => _ActionIdx + 1;

        public event PropertyChangedEventHandler PropertyChanged;

        public GameMode(Field field)
        {
            this.field = field;
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
            if (center.Y % 2 == 0)
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

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public UnitPresset CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
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

        public List<PathToken> getWalkArea()
        {
            pathField.Refresh();
            return pathField.getWalkArea(Selected.currentSpeed, Selected, GetUnits());
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

        public List<UnitPresset> getUnitsInBattle()
        {
            return UnitsInBattle;
        }

        public void AttackUnit(UnitPresset unit, UnitPresset target, int AbilityIdx)
        {
            throw new NotImplementedException();
        }

        public void Move(UnitPresset unit, PathToken pathToken)
        {
            throw new NotImplementedException();
        }

        public void CreateBuilding(Building build)
        {
            throw new NotImplementedException();
        }

        public bool SelectUnit(UnitPresset token)
        {
            if (token.owner == CurrentPlayer)
                return true;
            else
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

        public ITokenData getToken((int X, int Y) fpos)
        {
            throw new NotImplementedException();
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

        public void ProcessActions(List<IActions> actions)
        {
            foreach (var action in actions)
            {
                action.forward();
            }
        }
    }


}
