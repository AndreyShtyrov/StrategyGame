﻿using System;
using System.Collections.Generic;
using System.Text;
using Tokens;
using InterfaceOfObjects;
using System.ComponentModel;
using UnitsAnPathFinding;
using Controller.Abilities;
using Controller.Actions;
using Controller.Units;
using Controller.Requests;
using System.Threading.Tasks;
using Controller.Building;

namespace Controller
{
    public class GameMode: IGameMode
    {
        private List<IActions> Response = new List<IActions>();
        private IListOfToken field;
        private List<UnitPresset> units = new List<UnitPresset>();
        private List<BuildingPresset> buildings = new List<BuildingPresset>();
        private PathField pathField;
        private GameModeState _State;
        private List<UnitPresset> UnitsInBattle = new List<UnitPresset>();
        public Player CurrentPlayer => _CurrentPlayer;
        private ActionManager actionManager;
        private RequestManager requestManager;
        private Player _CurrentPlayer = Player.Get(0);
        public RequestSender RequestSender
        { get; }
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
        public int ActionIdx => actionManager.NextActionIdx;

        private int _CurrentTurnNumber = 0;
        public int CurrentTurnNumber
        {
            get
            {
                return _CurrentTurnNumber;
            }
            set
            {
                _CurrentTurnNumber = value;
                OnPropertyChanged("CurrentTurnNumber");
            }
        }

        public List<TurnsSpeciffication> Turns
        { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public GameMode(Field field)
        {
            this.field = field;
            pathField = new PathField(field);
            actionManager = new ActionManager();
            RequestSender = new RequestSender();
            RequestSender.SenderType = SenderType.Client;
            RequestSender.Player = GameTableController.Get().owner.idx;
            State = GameModeState.AwaitResponse;
            CurrentTurnNumber = 0;
            Player.Get(0).CurrentTurnNumber = 0;
            Player.Get(1).CurrentTurnNumber = 0;
        }

        public UnitPresset[,] GetGridOfUnits()
        {
            UnitPresset[,] result = new UnitPresset[field.height, field.width];
            foreach (var unit in units)
            {
                result[unit.fieldPosition.X, unit.fieldPosition.Y] = unit;
            }
            return result;
        }

        public void ChangePlayers(Player previusPlayer, Player nextPlayer)
        {
            _CurrentPlayer = nextPlayer;
            OnPropertyChanged("CurrentPlayer");
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

        public event OnUnitsListChange UnitsListChanged;

        public event OnBuildingListChange BuildingListChanged;

        public bool IsEnoughResources(int action, int move, Player owner)
        {
            if (owner.AttackPoints - action >= 0 && owner.MovePoints - move >= 0)
            {
                return true;
            }
            return false;
        }

        public void SpendResources(int action, int move, Player owner)
        {
            owner.AttackPoints -= action;
            owner.MovePoints -= move;
        }

        public void ReturnResources(int action, int move, Player owner)
        {
            owner.AttackPoints += action;
            owner.MovePoints += move;
        }

        public List<PathToken> GetWalkArea(UnitPresset unit)
        {
            pathField.Refresh();
            return pathField.getWalkArea(unit.currentSpeed, unit, GetGridOfUnits());
        }

        public PathToken GetPathToken(UnitPresset unit, (int X, int Y) fpos)
        {
            var pathTokens = GetWalkArea(unit);
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
            if (State == GameModeState.Standart)
            {
                State = GameModeState.AwaitResponse;
                RequestContainer requestContainer = new RequestContainer(RequestType.UseAbility);
                requestContainer.Selected = unit.fieldPosition;
                requestContainer.Target = target.fieldPosition;
                requestContainer.AbilityIdx = AbilityIdx;
                Client.sendRequest(requestContainer);
            }
        }

        public void Move(UnitPresset unit, PathToken pathToken)
        {
            if (State == GameModeState.Standart)
            {
                State = GameModeState.AwaitResponse;
                RequestContainer requestContainer = new RequestContainer(RequestType.MoveUnit);
                requestContainer.Selected = unit.fieldPosition;
                requestContainer.Target = pathToken.fieldPosition;
                Client.sendRequest(requestContainer);
            }
        }

        public void CreateBuilding(BuildingPresset build)
        {
            throw new NotImplementedException();
        }

        public BuildingPresset GetBuilding((int X ,int Y) position)
        {
            foreach ( var build in buildings)
            {
                if (build.fieldPosition == position)
                {
                    return build;
                }
            }
            return null;
        }

        public bool IsUnitSelected(UnitPresset token)
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
            ability.IsReadyToUse();
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

        public object ProcessRequset(object sender)
        {
            if (sender is RequestContainer tsender)
            {
                if (tsender.Type == RequestType.ApplyChanges)
                    ProcessActions(tsender.Actions);
                else if (tsender.Type == RequestType.ApplyChangesAndTakeControl)
                {
                    ProcessActions(tsender.Actions);
                    AcivateDeactivateGameTable(true);
                }
                else if (tsender.Type == RequestType.NeedResponse)
                {
                    AcivateDeactivateGameTable(true);
                    ProcessActions(tsender.Actions);
                    RequestUserInput(tsender);
                }
                else if (tsender.Type == RequestType.ResponseApplied)
                {
                    if (CurrentPlayer.idx == RequestSender.Player)
                    {
                        ProcessActions(tsender.Actions);
                        State = GameModeState.Standart;
                        AcivateDeactivateGameTable(true);
                    }
                    else
                    {
                        ProcessActions(tsender.Actions);
                        State = GameModeState.AwaitResponse;
                        AcivateDeactivateGameTable(false);
                    }
                }
                else if (tsender.Type == RequestType.ApplyAndWait)
                {
                    ProcessActions(tsender.Actions);
                    AcivateDeactivateGameTable(false);
                    State = GameModeState.AwaitResponse;
                }
                if (State == GameModeState.InteruptAndAwaitUserResponse)
                    GameTableController.Get().State = GameTableState.AwaitSelect;
            }
            return null;
        }

        public void ProcessActions(List<IActions> actions)
        {
            actionManager.ApplyActions(actions);
        }

        public Task GetNewGameStates()
        {
            RequestContainer requestContainer = new RequestContainer(RequestType.GetNewStates);
            requestContainer.CurrentActionIndex = actionManager.CurrentActionIdx;
            return Client.sendRequest(requestContainer);
        }

        public void BacklightTargets(UnitPresset unit, AbilityPresset ability)
        {
            var targets = pathField.getListOfTargets(unit, ability.DeafaultRange, GetGridOfUnits());
            foreach (var target in targets)
            {
                target.isTarget = true;
            }
        }

        public void RefreshBacklight()
        {
            foreach (var unit in units)
            {
                unit.isTarget = false;
            }
        }

        public void AddRequestManager(RequestManager Timer)
        {
            this.requestManager = Timer;
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

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            RequestContainer requestContainer = new RequestContainer(RequestType.CreateUnit);
            requestContainer.Name = name;
            requestContainer.Player = owner.idx;
            requestContainer.Selected = fpos;
            Client.sendRequest(requestContainer);
        }

        public void UpDownStand(UnitPresset unit, int StandIdx)
        {
            RequestContainer request = new RequestContainer(RequestType.UpDownStand);
            request.Player = GameTableController.Get().owner.idx;
            request.Selected = unit.fieldPosition;
            request.AbilityIdx = StandIdx;
            Client.sendRequest(request);
        }

        public List<UnitPresset> GetUnits() => units;

        public void ApplyAbilityWithoutSelection(UnitPresset unit, AbilityPresset Ability)
        {
            RequestContainer request = new RequestContainer(RequestType.UpDownStand);
            request.Player = GameTableController.Get().owner.idx;
            request.Selected = unit.fieldPosition;
            request.AbilityIdx = Ability.idx;
            Client.sendRequest(request);
        }

        public void SwitchTurn()
        {
            AcivateDeactivateGameTable(false);
            RequestContainer request = new RequestContainer(RequestType.SwitchTurn);
            request.Player = GameTableController.Get().owner.idx;
            Client.sendRequest(request);
        }

        public void RequestUserInput(RequestContainer container)
        {
            var gameTable = GameTableController.Get();
            var unit = GetUnit(container.Selected);
            gameTable.State = GameTableState.InteruptAndAnswerOnRequest;
            gameTable.Selected = unit;
            if (gameTable != null)
            {
                if (container.TargetsTypeName == "UnitPresset")
                {
                    foreach (var posUnit in container.Targets)
                    {
                        GetUnit(posUnit).isTarget = true;
                    }
                }
                else if (container.TargetsTypeName == "PathToken")
                {
                    var area = GetWalkArea(unit);
                    gameTable.DrawWalkArea(area);
                }
            }
        }

        public void SendUserResponse(UnitPresset unit, (int X, int Y) targetPosition)
        {
            RequestContainer requestContainer = new RequestContainer(RequestType.UserResponse);
            requestContainer.Selected = unit.fieldPosition;
            requestContainer.Target = targetPosition;            
            Client.sendRequest(requestContainer);
        }

        private void AcivateDeactivateGameTable(bool IsActive)
        {
            var gameTable = GameTableController.Get();
            if (gameTable == null)
                return;
            if (IsActive)
            {
                gameTable.State = GameTableState.AwaitSelect;
                State = GameModeState.Standart;
            }
            else
            {
                gameTable.State = GameTableState.AwaitEndEnemyTurn;
                State = GameModeState.AwaitResponse;
            }
        }

        public WeatherType GetWeather()
        {
            throw new NotImplementedException();
        }

        public void AddBuilding(BuildingPresset building)
        {
            buildings.Add(building);
            BuildingListChanged?.Invoke(building, true);
        }

        public void DeleteBuilding(BuildingPresset building)
        {
            buildings.Remove(building);
            BuildingListChanged?.Invoke(building, false);
        }

    }

    public delegate void OnBuildingListChange(ITokenData building, bool isExist);

    public delegate void OnUnitsListChange(UnitPresset unit, bool isExist);
}
