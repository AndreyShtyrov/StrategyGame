using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using UnitsAnPathFinding;
using DrawField;
using System.ComponentModel;
using System.Threading.Tasks;
using InterfaceOfObjects;
using System.Windows;

namespace Controller
{
    public class GameTableController : INotifyPropertyChanged
    {
        private static GameTableController instance;
        public IDrawFieldGUI FieldGUI;
        public UnitPresset _Selected;
        public UnitPresset Selected
        {
            set
            {
                _Selected = value;
                if (State != GameTableState.MultipleSelections)
                    OnPropertyChanged("Selected");
            }

            get
            {
                return _Selected;
            }
        }
        public AbilityPresset selectedAbility;
        public MultiSelectContainer MultiSelect;
        private Window multipleSelection;

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
            if (prevState == GameTableState.InteruptAndAnswerOnRequest)
            {
                FieldGUI.clearWalkedArea();
                GameModeContainer.Get().RefreshBacklight();
            }
            switch ((State))
            {
                case GameTableState.AwaitSelectAbility:
                    {
                        var walkArea = GameModeContainer.Get().GetWalkArea(Selected);
                        FieldGUI.addWalkedArea(walkArea);
                        break;
                    }
                case GameTableState.AwaitSelect:
                    {
                        Selected = null;
                        if (selectedAbility != null)
                        {
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
                        if (Selected == null || selectedAbility == null)
                            State = GameTableState.AwaitSelect;
                        else
                            GameModeContainer.Get().BacklightTargets(Selected, selectedAbility);
                        break;
                    }
                case GameTableState.InteruptAndAnswerOnRequest:
                    {
                        Selected = null;
                        selectedAbility = null;
                        FieldGUI.clearWalkedArea();
                        GameModeContainer.Get().RefreshBacklight();
                        break;
                    }
                case GameTableState.MultipleSelections:
                    {
                        FieldGUI.clearWalkedArea();
                        Selected = null;
                        selectedAbility = null; 
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
                            if (GameModeContainer.Get().IsUnitSelected(selectedUnit))
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
                                if (State != GameTableState.InteruptAndAnswerOnRequest)
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
                case GameTableState.InteruptAndAnswerOnRequest:
                    {
                        if (sender is PathToken pathToken)
                        {
                            GameModeContainer.Get().SendUserResponse(Selected, pathToken.fieldPosition);
                        }
                        else if (sender is UnitPresset unitPresset)
                        {
                            if (unitPresset.isTarget)
                            {
                                GameModeContainer.Get().SendUserResponse(Selected, unitPresset.fieldPosition);
                            }
                        }
                        break;
                    }
                case GameTableState.MultipleSelections:
                    {
                        if (sender is UnitPresset selectUnit)
                        {
                            MultiSelect.AddUnit(selectUnit);
                            Selected = selectUnit;
                        }
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
            GameModeContainer.Get().UpDownStand(Selected, stand.idx);
        }

        public void SelectedUnitActivateAbility(AbilityPresset ability)
        {
            if (ability.IsReadyToUse())
            {
                if (ability.AbilityType == AbilityType.ActionWitoutTargetSelect)
                { GameModeContainer.Get().ApplyAbilityWithoutSelection(Selected, ability); return; }
                selectedAbility = ability;
                State = GameTableState.AwaitSelectTarget;
            }
            else
                State = GameTableState.AwaitSelect;
        }

        public void SwitchTurn()
        {
            GameModeContainer.Get().SwitchTurn();
        }

        public void DrawWalkArea(List<PathToken> paths)
        {
            FieldGUI.addWalkedArea(paths);
        }

        public void MultipleSelectionMouseDown(object sender, MouseEventArgs e)
        {
            if (sender is UnitIcon tsender)
            {
                var unit = GameModeContainer.Get().
                    GetUnit(tsender.Data.fieldPosition);
                unit.isSelected = true;
                Selected = unit;
            }
        }

        public StartMulitpleSelectWindowHandler StartMulitpleSelectWindow;

        public void StartMulitpleSelection()
        {
            State = GameTableState.MultipleSelections;
            MultiSelect = new MultiSelectContainer(owner);
            multipleSelection = FieldGUI.generateMultipleSelectWindow(MultiSelect);
            StartMulitpleSelectWindow(multipleSelection);
        }

        public void StartMultipleBattle()
        {
            AbortMulitpleSelection();
            if (multipleSelection != null)
                multipleSelection.Close();
        }

        public void AbortMulitpleSelection()
        {
            if (Selected != null)
            {
                Selected.isSelected = false;
                Selected = null;
            }
            MultiSelect = null;
            if (multipleSelection != null)
                multipleSelection.Close();
        }

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            GameModeContainer.Get().CreateUnit(name, fpos, owner, typeUnit);
        }
    }

    public delegate void StartMulitpleSelectWindowHandler(Window multipleSelection);

    public enum GameTableState
    {
        AwaitSelect = 0,
        AwaitSelectAbility = 1,
        AwaitSelectTarget = 2,
        AwaitEndEnemyTurn = 3,
        AwaitApplyAbility = 4,
        AwaitGameModeResponse = 5,
        InteruptAndAnswerOnRequest = 6,
        MultipleSelections = 7,
    }

}
