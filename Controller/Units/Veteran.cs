using Controller.Abilities;
using Controller.Actions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Units
{
    public class Veteran:UnitPresset
    {
        public override string Name => "V";
        public override int MaxHp => 3;

        public Veteran((int X, int Y) fpos, Player owner) : base(fpos, owner)
        {
            maxSpeed = 2f;
            currentHp = MaxHp;
            List<UnitActionPoint> movelistActions = new List<UnitActionPoint>();
            movelistActions.Add(MovePoints[0]);
            List<UnitActionPoint> attackListAction = new List<UnitActionPoint>();
            movelistActions.Add(MovePoints[1]);
            attackListAction.Add(AttackPoints[0]);
            attackListAction.Add(AttackPoints[1]);
            MoveActionPoint = new ActionPoint(this, 0, 1, movelistActions);
            currentSpeed = maxSpeed;
            MeleeAttack melee = new MeleeAttack(this, attackListAction);
            melee.idx = 2;
            Abilities.Add(melee);

            var healList = new List<UnitActionPoint>(movelistActions);
            healList.AddRange(attackListAction);

            Heal heal = new Heal(this, healList);
            BuildCamp buildCamp = new BuildCamp(this, new List<UnitActionPoint>());
            Abilities.Add(buildCamp);
            heal.idx = 3;
            Abilities.Add(heal);
        }


        public override List<IActions> Move((int X, int Y) EndPosition)
        {
            var distance = GameModeContainer.Get().GetPathToken(this, EndPosition).pathLeght;
            if (distance == 1f && currentSpeed == maxSpeed)
                return new List<IActions>() { new MoveUnit(fieldPosition, EndPosition) };
            else
                return base.Move(EndPosition);
        }
    }
}
