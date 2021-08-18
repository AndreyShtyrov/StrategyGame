using Controller.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsAnPathFinding;

namespace Controller.GameLogic
{
    public class GameLogicClient: IGameLogic
    {

        private GameMode GameMode;
        public Player CurrentPlayer
        { get; set; }


        public GameLogicClient(GameMode gameMode)
        {
            GameMode = gameMode;
        }

        public void AttackUnit(UnitPresset unit, UnitPresset target, int AbilityIdx)
        {
            if (GameMode.State == GameModeState.Standart)
            {
                GameMode.State = GameModeState.AwaitResponse;
                RequestContainer requestContainer = new RequestContainer(RequestType.UseAbility);
                requestContainer.Selected = unit.fieldPosition;
                requestContainer.Target = target.fieldPosition;
                requestContainer.AbilityIdx = AbilityIdx;
                Client.sendRequest(requestContainer);
            }
        }

        public void Move(UnitPresset unit, PathToken pathToken)
        {
            if (GameMode.State == GameModeState.Standart)
            {
                GameMode.State = GameModeState.AwaitResponse;
                RequestContainer requestContainer = new RequestContainer(RequestType.MoveUnit);
                requestContainer.Selected = unit.fieldPosition;
                requestContainer.Target = pathToken.fieldPosition;
                Client.sendRequest(requestContainer);
            }
        }

        public void UpDownStand(UnitPresset unit, int StandIdx)
        {
            RequestContainer request = new RequestContainer(RequestType.UpDownStand);
            request.Player = GameTableController.Get().owner.idx;
            request.Selected = unit.fieldPosition;
            request.AbilityIdx = StandIdx;
            Client.sendRequest(request);
        }

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            RequestContainer requestContainer = new RequestContainer(RequestType.CreateUnit);
            requestContainer.Name = name;
            requestContainer.Player = owner.idx;
            requestContainer.Selected = fpos;
            Client.sendRequest(requestContainer);
        }

        public void SwitchTurn()
        {
            GameMode.AcivateDeactivateGameTable(false);
            RequestContainer request = new RequestContainer(RequestType.SwitchTurn);
            request.Player = GameTableController.Get().owner.idx;
            Client.sendRequest(request);
        }

        public void ApplyAbilityWithoutSelection(UnitPresset unit, AbilityPresset Ability)
        {
            RequestContainer request = new RequestContainer(RequestType.UpDownStand);
            request.Player = GameTableController.Get().owner.idx;
            request.Selected = unit.fieldPosition;
            request.AbilityIdx = Ability.idx;
            Client.sendRequest(request);
        }

        public void ProcessIteraptedAndNextActions(UnitPresset unit, (int X, int Y) fpos)
        {
            throw new NotImplementedException();
        }
    }
}
