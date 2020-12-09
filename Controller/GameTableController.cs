using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnitsAnPathFinding;
using DrawField;
using System.ComponentModel;

namespace Controller
{
    public class GameTableController
    {
        private static GameTableController instance;
        public FieldGUI FieldGUI;
        public UnitPresset Selected;
        public AbilityPresset abilitySelected;

        public Player owner;
        private GameTableState _State = GameTableState.AwaitSelect;
        public GameTableState State
        {
            set
            {
                var prevState = _State;
                _State = value;
                OnStateChange(prevState);
            }
            get
            {
                return _State;
            }
        }

        private void OnStateChange(GameTableState prevState)
        {
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
                        if (abilitySelected != null)
                        {
                            abilitySelected.Return();
                            abilitySelected = null;
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
                            Selected = selectedUnit;
                            if (GameModeContainer.Get().SelectUnit(selectedUnit))
                                State = GameTableState.AwaitSelectAbility;
                        }
                        break;
                    }
                case GameTableState.AwaitSelectTarget:
                    {
                        if (sender is UnitPresset target)
                        {
                            GameModeContainer.Get().AttackUnit(Selected, target, abilitySelected.idx);
                            State = GameTableState.AwaitSelect;
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

        public void OnPropertyChage(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "selectedAbility")
            {
                abilitySelected = GameModeContainer.Get().selectedAbility;
            }
        }

        private GameTableController(Player player, FieldGUI fieldGUI)
        {
            FieldGUI = fieldGUI;
            owner = player;
        }

        public static void InitGameTableControler(Player player, FieldGUI fieldGUI)
        {
            instance = new GameTableController(player, fieldGUI);
        }

        public static GameTableController Get()
        {
            return instance;
        }

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            FieldGUI.addUnit(GameModeContainer.Get().CreateUnit(name, fpos, owner, typeUnit));
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
