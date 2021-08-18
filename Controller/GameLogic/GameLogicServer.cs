using System;
using System.Collections.Generic;
using System.Text;
using UnitsAnPathFinding;

namespace Controller.GameLogic
{
    public class GameLogicServer : IGameLogic
    {
        private GameModeServer GameMode;
        private GameRules GameModeLogic;
        public Player CurrentPlayer
        {
            get
            {
                return GameModeLogic.CurrentPlayer;
            }
            set
            {
                GameModeLogic.CurrentPlayer = value;
            }
        }

        public GameLogicServer(GameModeServer gameMode, Player FirstPlyaer)
        {
            GameMode = gameMode;
            GameModeLogic = new GameRules(gameMode, FirstPlyaer);
        }

        public void AttackUnit(UnitPresset unit, UnitPresset target, int AbilityIdx)
        {
            GameMode.NetworkMode.Response = GameModeLogic.ProcessMeleeBattle(unit, target, AbilityIdx);
            if (GameTableController.Get() != null
                && GameTableController.Get().State != GameTableState.InteruptAndAnswerOnRequest)
                GameTableController.Get().State = GameTableState.AwaitSelect;
        }

        public void Move(UnitPresset unit, PathToken pathToken)
        {
            GameMode.NetworkMode.Response = GameModeLogic.Move(unit, pathToken);
            GameMode.PathField.Refresh();
            if (GameTableController.Get() != null)
                GameTableController.Get().State = GameTableState.AwaitSelect;
        }

        public void UpDownStand(UnitPresset unit, int StandIdx)
        {
            GameMode.NetworkMode.Response = GameModeLogic.UpDownStand(unit, StandIdx);
        }

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            GameMode.NetworkMode.Response = GameModeLogic.CreateUnit(name, fpos, owner, typeUnit);
        }

        public void SwitchTurn()
        {
            if (CurrentPlayer.idx == 1)
            {
                GameMode.NetworkMode.Response = GameModeLogic.SwitchTurn(CurrentPlayer, Player.Get(0));
            }
            else
            {
                GameMode.NetworkMode.Response = GameModeLogic.SwitchTurn(CurrentPlayer, Player.Get(1));
            }
        }

        public void ApplyAbilityWithoutSelection(UnitPresset unit, AbilityPresset Ability)
        {
            GameMode.NetworkMode.Response = GameModeLogic.ApplyAbilityWithoutSelection(unit, Ability);
        }

        public void ProcessIteraptedAndNextActions(UnitPresset unit, (int X, int Y) fpos)
        {
            GameMode.NetworkMode.Response = GameModeLogic.ProcessIteraptedAndNextActions(unit, fpos);
        }
    }

}
