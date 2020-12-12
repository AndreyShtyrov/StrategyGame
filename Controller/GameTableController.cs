using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnitsAnPathFinding;
using DrawField;
using System.ComponentModel;

namespace Controller
{
    public class GameTableController: INotifyPropertyChanged
    {
        private static GameTableController instance;
        public FieldGUI FieldGUI;
        public UnitPresset _Selected;
        public UnitPresset Selected
        {
            set
            {
                _Selected = value;
                OnPropertyChanged("Selected");
            }

            get
            {
                return _Selected;
            }
        }
        public AbilityPresset selectedAbility;

        public Player owner;
        private GameTableState _State = GameTableState.AwaitSelect;

        public event PropertyChangedEventHandler PropertyChanged;

        public GameTableState State
        {
            set
            {
                var prevState = _State;
                _State = value;
                OnStateChanged(prevState);
            }
            get
            {
                return _State;
            }
        }

        private void OnStateChanged(GameTableState prevState)
        {
            if (prevState == GameTableState.AwaitSelectTarget)
            {
                GameModeContainer.Get().RefreshBacklight();
            }
            switch ((State))
            {
                case GameTableState.AwaitSelectAbility:
                    {
                        var walkArea = GameModeContainer.Get().getWalkArea(Selected);
                        FieldGUI.addWalkedArea(walkArea);
                        break;
                    }
                case GameTableState.AwaitSelect:
                    {
                        Selected = null;
                        if (selectedAbility != null)
                        {
                            selectedAbility.Return();
                            selectedAbility = null;
                        }    
                            
                        if (GameTableState.AwaitSelectAbility == prevState ||
                            GameTableState.AwaitApplyAbility == prevState)
                            FieldGUI.clearWalkedArea();
                        break;
                    }
                case GameTableState.AwaitSelectTarget:
                    {
                        if (GameTableState.AwaitSelectAbility == prevState)
                            FieldGUI.clearWalkedArea();
                        var controller = GameModeContainer.Get();
                        GameModeContainer.Get().BacklightTargets(Selected, selectedAbility);
                        break;
                    }
                
            }
        }

        public void ActionOnMouseButton(object sender, MouseEventArgs e)
        {
            switch ((State))
            {
                case GameTableState.AwaitSelect:
                    {
                        if (sender is UnitPresset selectedUnit)
                        {
                            if (GameModeContainer.Get().SelectUnit(selectedUnit))
                            {
                                if (Selected != null)
                                    Selected.isSelected = false;
                                Selected = null;
                                Selected = selectedUnit;
                                State = GameTableState.AwaitSelectAbility;
                            } 
                        }
                        break;
                    }
                case GameTableState.AwaitSelectTarget:
                    {
                        if (sender is UnitPresset target)
                        {
                            if (target.isTarget)
                            {
                                GameModeContainer.Get().AttackUnit(Selected, target, selectedAbility.idx);
                                State = GameTableState.AwaitSelect;
                            }
                        }
                        else
                        {
                            State = GameTableState.AwaitSelect;
                        }
                        break;
                    }
                case GameTableState.AwaitSelectAbility:
                    {
                        if (e.RightButton == MouseButtonState.Pressed
                            && sender is PathToken pathToken)
                        {
                            State = GameTableState.AwaitApplyAbility;
                            FieldGUI.clearWalkedArea();
                            GameModeContainer.Get().Move(Selected, pathToken);
                            return;
                        }
                        else
                        {
                            State = GameTableState.AwaitSelect; 
                        }
                        break;
                    }
                case GameTableState.AwaitGameModeResponse:
                    {
                        break;
                    }
            }

        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private GameTableController(Player player, FieldGUI fieldGUI)
        {
            FieldGUI = fieldGUI;
            owner = player;
        }

        public static void Create(Player player, FieldGUI fieldGUI)
        {
            instance = new GameTableController(player, fieldGUI);
        }

        public static GameTableController Get()
        {
            return instance;
        }

        public void SelectedUnitRaiseStand(StandPresset stand)
        {
            stand.UpStand();
        }

        public void SelectedUnitActivateAbility(AbilityPresset ability)
        {
            ability.PrepareToUse();
            State = GameTableState.AwaitSelectTarget;
        }

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            GameModeContainer.Get().CreateUnit(name, fpos, owner, typeUnit);
        }
    }

    public enum GameTableState
    {
        AwaitSelect = 0,
        AwaitSelectAbility = 1,
        AwaitSelectTarget = 2,
        AwaitEndEnemyTurn = 3,
        AwaitApplyAbility = 4,
        AwaitGameModeResponse = 5,
    }

}
