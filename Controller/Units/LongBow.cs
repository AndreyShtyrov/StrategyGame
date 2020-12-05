using System;
using System.Collections.Generic;
using System.Text;
using Controller.Abilities;
using Newtonsoft.Json;

namespace Controller.Units
{
    public class LongBow : UnitPresset
    {

        public override string Name => "DL";
        public override int MaxHp => 2;

        [JsonConstructor]
        public LongBow() : base()
        { }

        public LongBow((int X, int Y) fpos, Player owner) : base(fpos, owner)
        {
            List<UnitActionPoint> movelistAction = new List<UnitActionPoint>();
            movelistAction.Add(MovePoints[0]);
            movelistAction.Add(MovePoints[1]);
            List<UnitActionPoint> attacklistAction = new List<UnitActionPoint>();
            attacklistAction.Add(AttackPoints[0]);
            attacklistAction.Add(AttackPoints[1]);
            MoveActionPoint = new ActionPoint(this, 0, 1, movelistAction);

            Abilities.Add(new WeakMeleeAttack(this, attacklistAction));
            Abilities.Add(new LongBowAttack(this, attacklistAction));
            movelistAction.AddRange(attacklistAction);
            Abilities.Add(new Heal(this, movelistAction));
            currentHp = MaxHp;

        }
    }
}
