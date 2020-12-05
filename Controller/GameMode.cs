using System;
using System.Collections.Generic;
using System.Text;
using Tokens;
using InterfaceOfObjects;
using System.ComponentModel;
using UnitsAnPathFinding;

namespace Controller
{
    class GameMode: IGameMode
    {
        public UnitPresset Selected
        {
            set
            {
                _Selected = value;
                OnPropertyChanged("Selected");
            }
            get { return _Selected; }
        }

        public GameModeState State { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private UnitPresset target;
        private List<UnitPresset> AddSelections = new List<UnitPresset>();
        private List<UnitPresset> AddTargets = new List<UnitPresset>();
        private UnitPresset _Selected;
        private IListOfToken field;
        private List<UnitPresset> units = new List<UnitPresset>();
        private List<Building> buildings = new List<Building>();

        private static GameMode instance;

        public event PropertyChangedEventHandler PropertyChanged;

        GameMode(Field field )
        {
            this.field = field;
        }

        public static GameModeServer Get(Field field = null)
        {
            if (instance == null)
                instance = new GameMode(field);
            return instance;
        }

        private void OnGameModeChangeState(GameModeState prevState)
        {

            switch ((State))
            {
                case GameModeState.AwaitSelect:
                    {
                        if (prevState == GameModeState.MoveMode)
                        {
                            pathField.Refresh();
                            Selected.isSelected = false;
                            Selected = null;
                        }
                        if (Selected != null)
                        {
                            Selected.isSelected = false;
                            Selected = null;
                        }
                        if (selectedAbility != null)
                        {
                            selectedAbility.Return();
                            selectedAbility = null;

                            State = GameModeState.AwaitSelect;
                        }
                        if (PossibleTargets != null)
                        {
                            foreach (var unit in PossibleTargets)
                            { unit.isTarget = false; }
                            PossibleTargets = null;
                        }
                        break;
                    }
                case GameModeState.MoveMode:
                    {
                        if (Selected.MoveActionPoint.State == ActionState.Ended)
                            break;
                        break;
                    }
                case GameModeState.SelectEnemy:
                    {
                        if (Selected.MoveActionPoint.State == ActionState.InProcess)
                            Selected.MoveActionPoint.Spend();
                        if (prevState == GameModeState.MoveMode)
                        {
                            pathField.Refresh();
                        }

                        PossibleTargets = pathField.getListOfTargets(Selected,
                            selectedAbility.DeafaultRange, GetUnits());
                        foreach (var target in PossibleTargets)
                        { target.isTarget = true; }
                        break;
                    }
                case GameModeState.AwaitEnemyEnd:
                    {
                        break;
                    }
                default:
                    break;
            }
        }

        public UnitPresset[,] GetUnits()
        {
            throw new NotImplementedException();
        }

        public void SwitchTrun()
        {
            throw new NotImplementedException();
        }

        public UnitPresset CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            throw new NotImplementedException();
        }

        public (int X, int Y, int Z) TransformToCube((int X, int Y) fpos, (int X, int Y) center)
        {
            throw new NotImplementedException();
        }

        public (int X, int Y) TransformToAxial((int X, int Y, int Z) fpos, (int X, int Y) center)
        {
            throw new NotImplementedException();
        }

        public bool SpendResources(int action, int move, Player owner)
        {
            throw new NotImplementedException();
        }

        public void ReturnResources(int action, int move, Player owner)
        {
            throw new NotImplementedException();
        }

        public List<UnitPresset> getUnitsInBattle()
        {
            throw new NotImplementedException();
        }

        public void AttackUnit(UnitPresset target)
        {
            throw new NotImplementedException();
        }

        public List<PathToken> getWalkArea()
        {
            throw new NotImplementedException();
        }

        public void MoveUnit(PathToken pathToken)
        {
            throw new NotImplementedException();
        }

        public void CreateBuilding(Building build)
        {
            throw new NotImplementedException();
        }

        public bool SelectUnit(UnitPresset token)
        {
            throw new NotImplementedException();
        }

        public void SelectedUnitRaiseStand(StandPresset stand)
        {
            throw new NotImplementedException();
        }

        public void SelectedUnitActivateAbility(AbilityPresset ability)
        {
            throw new NotImplementedException();
        }

        public ITokenData getToken((int X, int Y) fpos)
        {
            throw new NotImplementedException();
        }

        public IUnitPresset getUnit((int X, int Y) fpos)
        {
            throw new NotImplementedException();
        }
    }

    public enum RequstTypes 
    {
        WalkedArea = 0,
        AttackUnit = 1,
        MoveUnit = 2,
        RaiseStand = 3,
        ActiveAbility = 4,
        CreateUnit = 5,
        SuncUnits = 6,
        SuncBuildings = 7,
        SwitchTurn = 8,
    }

}
