using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    class SwitchTurn : IActions
    {
        public int idx { get; set; }

        public List<UnitStatus> PreviousPlayerUnitsStatuses;

        public List<UnitStatus> NextPlayerUnitsStatuses;

        public (int AttackPoint, int MovePoint) PreviousPointsPreviosPlayer
        { get; set; }

        public int PrevPlayer;

        public int NextPlayer;

        public (int AttackPoint, int MovePoint) NextPointsNextPlayer
        { get; set; }

        public (int AttackPoint, int MovePoint) PreviosPointsNextPlayer
        { get; set; }

        [JsonConstructor]
        public SwitchTurn()
        { }

        public SwitchTurn(Player prevPlayer, Player nextPlayer)
        {
            idx = GameModeContainer.Get().ActionIdx;
            PreviousPointsPreviosPlayer = (prevPlayer.AttackPoints, prevPlayer.MovePoints);
            NextPointsNextPlayer = (
                nextPlayer.AttackPoints + nextPlayer.IncomeAttackPoints,
                nextPlayer.MovePoints + nextPlayer.IncomeMovePoints);
            PrevPlayer = prevPlayer.idx;
            NextPlayer = nextPlayer.idx;
            PreviousPlayerUnitsStatuses = new List<UnitStatus>();
            NextPlayerUnitsStatuses = new List<UnitStatus>();
            PreviousPointsPreviosPlayer = (nextPlayer.AttackPoints, nextPlayer.MovePoints);
            foreach (var unit in GameModeContainer.Get().GetUnits())
            {
                if (unit.owner == prevPlayer)
                {
                    PreviousPlayerUnitsStatuses.Add( new UnitStatus(unit));
                }
                else
                {
                    NextPlayerUnitsStatuses.Add(new UnitStatus(unit));
                }
            }
        }

        public void forward()
        {
            var nextPlayer = Player.Get(NextPlayer);
            var prevPlayer = Player.Get(PrevPlayer);
            nextPlayer.MovePoints = NextPointsNextPlayer.MovePoint;
            nextPlayer.AttackPoints = NextPointsNextPlayer.AttackPoint;
            prevPlayer.MovePoints = 0;
            prevPlayer.AttackPoints = 0;
            GameModeContainer.Get().ChangePlayers(prevPlayer, nextPlayer);
            foreach (var unit in GameModeContainer.Get().GetUnits())
            {
                if (unit.owner == nextPlayer)
                {
                    unit.Refresh();
                }
            }
        }

        public void reverse()
        {
            var nextPlayer = Player.Get(NextPlayer);
            var prevPlayer = Player.Get(PrevPlayer);
            prevPlayer.MovePoints = 10000;
            prevPlayer.AttackPoints = 1000;
            foreach (var savedUnit in PreviousPlayerUnitsStatuses)
            {
                SetStateStates(savedUnit);
            }
            foreach (var savedUnit in NextPlayerUnitsStatuses)
            {
                SetStateStates(savedUnit);
            }

            prevPlayer.MovePoints = PreviousPointsPreviosPlayer.MovePoint;
            prevPlayer.AttackPoints = PreviousPointsPreviosPlayer.AttackPoint;
            nextPlayer.MovePoints = PreviosPointsNextPlayer.MovePoint;
            nextPlayer.AttackPoints = PreviosPointsNextPlayer.AttackPoint;

            GameModeContainer.Get().ChangePlayers(prevPlayer, nextPlayer);
        }

        private void SetStateStates(UnitStatus savedUnit)
        {
            var unit = GameModeContainer.Get().GetUnit(savedUnit.FieldPosition);
            foreach (var savedStend in savedUnit.Stends)
            {
                var stand = unit.GetStand(savedStend.Idx);
                if (savedStend.State == ActionState.InProcess)
                {
                    stand.Active = savedStend.IsActive;
                    stand.point.ToInProgres();
                }
                if (savedStend.State == ActionState.Ended)
                {
                    stand.Active = savedStend.IsActive;
                    stand.point.Spend();
                }
            }
            foreach (var saveAbility in savedUnit.Abilities)
            {
                var ability = unit.GetAbility(saveAbility.Idx);
                if (saveAbility.State == ActionState.InProcess)
                {
                    ability.actionPoint.ToInProgres();
                }
                if (saveAbility.State == ActionState.Ended)
                {
                    ability.actionPoint.ToInProgres();
                }
            }
            if (savedUnit.Move == ActionState.InProcess)
            {
                unit.MoveActionPoint.ToInProgres();
            }
            if (savedUnit.Move == ActionState.Ended)
            {
                unit.MoveActionPoint.Spend();
            }
        }
    }
}
