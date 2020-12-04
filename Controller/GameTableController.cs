using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnitsAnPathFinding;
using DrawField;

namespace Controller
{
    public class GameTableController
    {
        private static GameTableController instance;
        public FieldGUI FieldGUI;
        public UnitPresset Selcted;

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
                        var walkArea = GameMode.Get().getWalkArea();
                        FieldGUI.addWalkedArea(walkArea);
                        break;
                    }
                case GameTableState.AwaitSelect:
                    {
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

                            if (GameMode.Get().SelectUnit(selectedUnit))
                                State = GameTableState.AwaitSelectAbility;
                        }
                        break;
                    }
                case GameTableState.AwaitSelectTarget:
                    {
                        if (sender is UnitPresset target)
                        {
                            GameMode.Get().AttackUnit(target);
                            State = GameTableState.AwaitSelect;
                        }
                        else
                        {
                            GameMode.Get().State = GameModeState.AwaitSelect;
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
                            GameMode.Get().MoveUnit(pathToken);
                            return;
                        }
                        else
                        {
                            var gameMode = GameMode.Get();
                            gameMode.State = GameModeState.AwaitSelect;
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

        private GameTableController(Player player, FieldGUI fieldGUI)
        {
            FieldGUI = fieldGUI;
            owner = player;
        }

        public static GameTableController Get(Player player, FieldGUI fieldGUI)
        {
            if (instance == null)
                instance = new GameTableController(player, fieldGUI);
            return instance;
        }

        public static GameTableController Get()
        {
            return instance;
        }

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            FieldGUI.addUnit(GameMode.Get().CreateUnit(name, fpos, owner, typeUnit));
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
