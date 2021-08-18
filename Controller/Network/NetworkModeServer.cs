using Controller.Actions;
using Controller.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Network
{
    public class NetworkModeServer: INetworkMode
    {
        private GameModeServer GameMode;
        private GameModeState State;
        public List<IActions> Response
        { get; set; }
        private RequestContainer preparedContainer;

        public NetworkModeServer(GameModeServer gameMode)
        {
            GameMode = gameMode;
            State = GameModeState.Standart;
            Response = new List<IActions>();
        }

        public object ProcessRequset(object sender)
        {
            Response.Clear();
            if (sender is RequestContainer getNewStates)
            {
                if (getNewStates.Type == RequestType.GetNewStates)
                {
                    var response = GameMode.actionManager.GetMissedActions(getNewStates.CurrentActionIndex);
                    RequestContainer applyChangesRequest;
                    if (State == GameModeState.InteruptAndAwaitUserResponse
                        && getNewStates.RequestSender.Player == GameMode.ControllingPlayer.idx)
                    {
                        preparedContainer.Actions = response;
                        return preparedContainer;
                    }
                    else if (State != GameModeState.InteruptAndAwaitUserResponse &&
                        getNewStates.RequestSender.Player == GameMode.ControllingPlayer.idx)
                    {
                        applyChangesRequest = new RequestContainer(RequestType.ApplyChangesAndTakeControl);
                        applyChangesRequest.Actions = response;
                        return applyChangesRequest;
                    }
                    else if (State != GameModeState.InteruptAndAwaitUserResponse &&
                        getNewStates.RequestSender.Player != GameMode.ControllingPlayer.idx)
                    {
                        applyChangesRequest = new RequestContainer(RequestType.ApplyChanges);
                        applyChangesRequest.Actions = response;
                        return applyChangesRequest;
                    }
                    else if (State == GameModeState.InteruptAndAwaitUserResponse
                        && getNewStates.RequestSender.Player != GameMode.ControllingPlayer.idx)
                    {
                        applyChangesRequest = new RequestContainer(RequestType.ApplyChanges);
                        applyChangesRequest.Actions = response;
                        return applyChangesRequest;
                    }
                }
                else if (getNewStates.Type == RequestType.UserResponse)
                {
                    var unit = GameMode.GetUnit(getNewStates.Selected);
                    SendUserResponse(unit, getNewStates.Target);
                    RequestContainer applyChanges = new RequestContainer(RequestType.ApplyAndWait);
                    applyChanges.Actions = Response;
                    return applyChanges;
                }
                //else if (getNewStates.Type == RequestType.LoadMap)
                //{
                //    var map = field;
                //    RequestContainer applyChanges = new RequestContainer(RequestType.LoadMap);
                //    applyChanges.Map = map;
                //    applyChanges.Turns = Turns;
                //    applyChanges.ActionTurns = player1Querry;
                //    return applyChanges;

                //}
            }
            if (State == GameModeState.Standart)
            {
                if (sender is RequestContainer request)
                {
                    if (request.Actions != null)
                        GameMode.ProcessActions(request.Actions);
                    if (request.Type == RequestType.MoveUnit)
                    {
                        var unit = GameMode.GetUnit(request.Selected);
                        var pathToken = GameMode.GetPathToken(unit, request.Target);
                        GameMode.GameModeLogic.Move(unit, pathToken);
                    }
                    if (request.Type == RequestType.UseAbility)
                    {
                        var unit = GameMode.GetUnit(request.Selected);
                        var ability = unit.GetAbility(request.AbilityIdx);
                        if (ability.AbilityType == AbilityType.Attack ||
                            ability.AbilityType == AbilityType.RangeAttack)
                        {
                            var target = GameMode.GetUnit(request.Target);
                            GameMode.GameModeLogic.AttackUnit(unit, target, ability.idx);
                        }
                        else if (ability.AbilityType == AbilityType.ActionWitoutTargetSelect)
                        {
                            GameMode.GameModeLogic.ApplyAbilityWithoutSelection(unit, ability);
                        }
                    }
                    if (request.Type == RequestType.CreateUnit)
                    {
                        GameMode.GameModeLogic.CreateUnit(request.Name, request.Selected, Player.Get(request.Player));
                    }
                    if (request.Type == RequestType.UpDownStand)
                    {
                        var standUnit = GameMode.GetUnit(request.Selected);
                        GameMode.GameModeLogic.UpDownStand(standUnit, request.AbilityIdx);
                    }
                    if (request.Type == RequestType.SwitchTurn)
                    {
                        GameMode.GameModeLogic.SwitchTurn();
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

        public void RequestUserInput(RequestContainer container)
        {
            var gameTable = GameTableController.Get();
            if (gameTable != null)
            {
                var unit = GameMode.GetUnit(container.Selected);

                gameTable.State = GameTableState.InteruptAndAnswerOnRequest;
                if (container.TargetsTypeName == "UnitPresset")
                {
                    foreach (var posUnit in container.Targets)
                    {
                        GameMode.GetUnit(posUnit).isTarget = true;
                    }
                }
                else if (container.TargetsTypeName == "PathToken")
                {
                    var area = GameMode.GetWalkArea(unit);
                    gameTable.DrawWalkArea(area);
                }

            }
        }

        public void SendUserResponse(UnitPresset unit, (int X, int Y) targetPosition)
        {
            GameMode.GameModeLogic.ProcessIteraptedAndNextActions(unit, targetPosition);
            if (State == GameModeState.Standart)
            {
                var gameTableState = GameTableController.Get();
                if (gameTableState != null)
                {
                    gameTableState.State = GameTableState.AwaitSelect;
                }
            }
        }

    }
}
