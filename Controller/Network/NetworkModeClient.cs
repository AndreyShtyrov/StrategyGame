using Controller.Actions;
using Controller.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Network
{
    public class NetworkModeClient: INetworkMode
    {
        private IGameMode GameMode;
        private GameModeState State;
       

        public NetworkModeClient(IGameMode gameMode)
        {
            State = GameModeState.Standart;
            GameMode = gameMode;
        }

        public List<IActions> Response { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public object ProcessRequset(object sender)
        {
            if (sender is RequestContainer tsender)
            {
                if (tsender.Type == RequestType.ApplyChanges)
                    GameMode.ProcessActions(tsender.Actions);
                else if (tsender.Type == RequestType.ApplyChangesAndTakeControl)
                {
                    GameMode.ProcessActions(tsender.Actions);
                    GameMode.AcivateDeactivateGameTable(true);
                }
                else if (tsender.Type == RequestType.NeedResponse)
                {
                    GameMode.AcivateDeactivateGameTable(true);
                    GameMode.ProcessActions(tsender.Actions);
                    RequestUserInput(tsender);
                }
                else if (tsender.Type == RequestType.ResponseApplied)
                {
                    if (GameMode.CurrentPlayer.idx == GameMode.RequestSender.Player)
                    {
                        GameMode.ProcessActions(tsender.Actions);
                        State = GameModeState.Standart;
                        GameMode.AcivateDeactivateGameTable(true);
                    }
                    else
                    {
                        GameMode.ProcessActions(tsender.Actions);
                        State = GameModeState.AwaitResponse;
                        GameMode.AcivateDeactivateGameTable(false);
                    }
                }
                else if (tsender.Type == RequestType.ApplyAndWait)
                {
                    GameMode.ProcessActions(tsender.Actions);
                    GameMode.AcivateDeactivateGameTable(false);
                    State = GameModeState.AwaitResponse;
                }
                if (State == GameModeState.InteruptAndAwaitUserResponse)
                    GameTableController.Get().State = GameTableState.AwaitSelect;
            }
            return null;
        }


        public void RequestUserInput(RequestContainer container)
        {
            var gameTable = GameTableController.Get();
            var unit = GameMode.GetUnit(container.Selected);
            gameTable.State = GameTableState.InteruptAndAnswerOnRequest;
            gameTable.Selected = unit;
            if (gameTable != null)
            {
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
            RequestContainer requestContainer = new RequestContainer(RequestType.UserResponse);
            requestContainer.Selected = unit.fieldPosition;
            requestContainer.Target = targetPosition;
            Client.sendRequest(requestContainer);
        }
    }
}
