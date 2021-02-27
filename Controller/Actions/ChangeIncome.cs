using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    public class ChangeIncome: IActions
    {
        [JsonConsturctor]
        public ChangeIncome()
        { }

        public int PlayerIdx
        { get; set; }

        public int PreviousAttackPoint
        { get; set; }

        public int PreviousMovePoint
        { get; set; }

        public int NextAttackPoint
        { get; set; }

        public int NextMovePoint
        { get; set; }

        public ChangeIncome(Player player, int AttackPoint=0, int MovePoint=0)
        {
            idx = GameModeContainer.Get().ActionIdx;
            PlayerIdx = player.idx;
            PreviousAttackPoint = player.IncomeAttackPoints;
            PreviousMovePoint = player.IncomeMovePoints;
            NextAttackPoint = PreviousAttackPoint + AttackPoint;
            NextMovePoint = PreviousMovePoint + MovePoint;
        }

        public int idx { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void forward()
        {
            var player = Player.Get(PlayerIdx);
            player.IncomeAttackPoints = NextAttackPoint;
            player.IncomeMovePoints = NextMovePoint;
        }

        public void reverse()
        {
            var player = Player.Get(PlayerIdx);
            player.IncomeAttackPoints = NextAttackPoint;
            player.IncomeMovePoints = NextMovePoint;
        }
    }
}
