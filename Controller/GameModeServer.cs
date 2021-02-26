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
        private List<IActions> Response = new List<IActions>();
        private List<RequestContainer> DelaiedRequests = new List<RequestContainer>();
        private GameModeLogic GameModeLogic;
        private List<(int index, List<UnitPresset> Units, List<Building> buildings)> PrevState;
        private IListOfToken field;
        private List<UnitPresset> units = new List<UnitPresset>();
        private List<Building> buildings = new List<Building>();
        private PathField pathField;
        private ActionManager actionManager;
        private GameModeState _State;
        private List<UnitPresset> UnitsInBattle = new List<UnitPresset>();
        public Player CurrentPlayer => GameModeLogic.CurrentPlayer;
        private RequestManager requestManager;
        private Player ControllingPlayer;
        private RequestContainer preparedContainer;
        public List<TurnSpeciffication> AllTurns;
        private int _CurrentTurnNumber=0;
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
        public RequestSender RequestSender
        { get; }
        public int ActionIdx => actionManager.NextActionIdx;

        public List<TurnSpeciffication> Turns
        { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public GameModeServer(Field field, int reqestSender = -1)
        {
            this.field = field;
            pathField = new PathField(field);
            actionManager = new ActionManager();
            RequestSender = new RequestSender();
            RequestSender.SenderType = SenderType.Server;
            RequestSender.Player = reqestSender;
            GameModeLogic = new GameModeLogic(this, Player.Get(1));
        }

        public ITokenData getToken((int X, int Y) fpos)
        {
            return field.getData(fpos);
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

        public List<UnitPresset> GetUnits()
        {
            return units;
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

        public void ChangePlayers(Player PrivousPlayer, Player NextPlayer)
        {
            GameModeLogic.CurrentPlayer = NextPlayer;
            ControllingPlayer = NextPlayer;
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

        public List<UnitPresset> getUnitsInBattle()
        {
            return UnitsInBattle;
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void PrepareToRequestUserInput(RequestContainer requestContainer, Player player)
        {
            preparedContainer = requestContainer;
            State = GameModeState.InteruptAndAwaitUserResponse;
            
            ChangePlayers(CurrentPlayer, player);
            if (CurrentPlayer.idx == RequestSender.Player)
            {
                RequestUserInput(preparedContainer);
            }
        }

        public void AttackUnit(UnitPresset unit, UnitPresset target, int AbilityIdx)
        {
            Response = GameModeLogic.ProcessMeleeBattle(unit, target, AbilityIdx);
            if (GameTableController.Get() != null 
                && GameTableController.Get().State != GameTableState.InteruptAndAnswerOnRequest)
                GameTableController.Get().State = GameTableState.AwaitSelect;
        }

        public void UpDownStand(UnitPresset unit, int StandIdx)
        {
            Response = GameModeLogic.UpDownStand(unit, StandIdx);
        }

        public List<PathToken> GetWalkArea(UnitPresset unit)
        {
            pathField.Refresh();
            if (unit.MoveActionPoint.State == ActionState.Ended)
                return new List<PathToken>();
            return pathField.getWalkArea(unit.currentSpeed, unit, GetGridOfUnits());
        }

        public void Move(UnitPresset unit, PathToken pathToken)
        {
            Response = GameModeLogic.Move(unit, pathToken);
            pathField.Refresh();
            if (GameTableController.Get() != null)
                GameTableController.Get().State = GameTableState.AwaitSelect;
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
            Response = GameModeLogic.CreateUnit(name, fpos, owner, typeUnit);
        }

        public object ProcessRequset(object sender)
        {
            Response.Clear();
            if (sender is RequestContainer getNewStates)
            {
                if (getNewStates.Type == RequestType.GetNewStates)
                {
                    var response = actionManager.GetMissedActions(getNewStates.CurrentActionIndex);
                    RequestContainer applyChangesRequest;
                    if (State == GameModeState.InteruptAndAwaitUserResponse
                        && getNewStates.RequestSender.Player == ControllingPlayer.idx)
                    {
                        preparedContainer.Actions = response;
                        return preparedContainer;
                    }
                    else if (State != GameModeState.InteruptAndAwaitUserResponse && 
                        getNewStates.RequestSender.Player == ControllingPlayer.idx)
                    {
                        applyChangesRequest = new RequestContainer(RequestType.ApplyChangesAndTakeControl);
                        applyChangesRequest.Actions = response;
                        return applyChangesRequest;
                    }
                    else if ( State != GameModeState.InteruptAndAwaitUserResponse &&
                        getNewStates.RequestSender.Player != ControllingPlayer.idx)
                    {
                        applyChangesRequest = new RequestContainer(RequestType.ApplyChanges);
                        applyChangesRequest.Actions = response;
                        return applyChangesRequest;
                    }
                    else if (State == GameModeState.InteruptAndAwaitUserResponse 
                        && getNewStates.RequestSender.Player != ControllingPlayer.idx)
                    {
                        applyChangesRequest = new RequestContainer(RequestType.ApplyChanges);
                        applyChangesRequest.Actions = response;
                        return applyChangesRequest;
                    }
                }
                else if (getNewStates.Type == RequestType.UserResponse)
                {
                    var unit = GetUnit(getNewStates.Selected);
                    SendUserResponse(unit, getNewStates.Target);
                    RequestContainer applyChanges = new RequestContainer(RequestType.ApplyAndWait);
                    applyChanges.Actions = Response;
                    return applyChanges;
                }
            }
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
                    }
                    if (request.Type == RequestType.UseAbility)
                    {
                        var unit = GetUnit(request.Selected);
                        var ability = unit.GetAbility(request.AbilityIdx);
                        if (ability.AbilityType == AbilityType.Attack ||
                            ability.AbilityType == AbilityType.RangeAttack)
                        {
                            var target = GetUnit(request.Target);
                            AttackUnit(unit, target, ability.idx);
                        }
                        else if (ability.AbilityType == AbilityType.ActionWitoutTargetSelect)
                        {
                            ApplyAbilityWithoutSelection(unit, ability);
                        }
                    }
                    if (request.Type == RequestType.CreateUnit)
                    {
                        CreateUnit(request.Name, request.Selected, Player.Get(request.Player));
                    }
                    if (request.Type == RequestType.UpDownStand)
                    {
                        var standUnit = GetUnit(request.Selected);
                        UpDownStand(standUnit, request.AbilityIdx);
                    }
                    if (request.Type == RequestType.SwitchTurn)
                    {
                        SwitchTurn();
                    }
                    RequestContainer applyChangesRequest;
                    if (State != GameModeState.InteruptAndAwaitUserResponse)
                    {
                        applyChangesRequest = new RequestContainer(RequestType.ApplyChanges);
                        applyChangesRequest.Actions = Response;
                        return applyChangesRequest;
                    }
                    else
                    {
                        applyChangesRequest = new RequestContainer(RequestType.ApplyAndWait);
                        applyChangesRequest.Actions = Response;
                        return applyChangesRequest;
                    }
                }
            }
            return null;
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

        public void ProcessActions(List<IActions> actions)
        {
            actionManager.ApplyActions(actions);
        }

        public bool IsUnitSelected(UnitPresset token)
        {
            if (token.owner == CurrentPlayer)
                return true;
            else
                return false;
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

        public Task GetNewGameStates()
        {
            throw new NotImplementedException();
        }

        public void AddRequestManager(RequestManager Timer)
        {
            this.requestManager = Timer;
        }

        public void SwitchTurn()
        {
            if (CurrentPlayer.idx == 1)
            {
                Response = GameModeLogic.SwitchTurn(CurrentPlayer, Player.Get(0));
            }
            else
            {
                Response = GameModeLogic.SwitchTurn(CurrentPlayer, Player.Get(1));
            }
        }

        public void RequestUserInput(RequestContainer container)
        {
            var gameTable = GameTableController.Get();
            if (gameTable != null)
            {
                var unit = GetUnit(container.Selected);

                gameTable.State = GameTableState.InteruptAndAnswerOnRequest;
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

        public void ApplyAbilityWithoutSelection(UnitPresset unit, AbilityPresset Ability)
        {
            Response = GameModeLogic.ApplyAbilityWithoutSelection(unit, Ability);
        }

        public void SendUserResponse(UnitPresset unit, (int X, int Y) targetPosition)
        {
            Response = GameModeLogic.ProcessIteraptedAndNextActions(unit, targetPosition);
            if (State == GameModeState.Standart)
            {
                var gameTableState = GameTableController.Get();
                if (gameTableState != null)
                {
                    gameTableState.State = GameTableState.AwaitSelect;
                }
            }
        }

        public WeatherType GetWeather()
        {
            foreach (var turn in Turns)
            {
                if (turn.Number == CurrentTurnNumber)
                    return turn.Weather;
            }
            throw new NotImplementedException();
        }
    }

    public delegate void GameModeEventHandler(UnitPresset sender, UnitPresset target, GameModEventArgs e);

    public enum GameModeState
    {
        Standart = 0,
        AwaitResponse = 1,
        InteruptAndAwaitUserResponse = 2,
    }

    public delegate RequestContainer GetDelaiedResponse(RequestContainer requestContainer);

    public class GameModEventArgs
    {

    }
}
