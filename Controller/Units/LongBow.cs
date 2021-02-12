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

            WeakMeleeAttack meleeAttack = new WeakMeleeAttack(this, attacklistAction);
            LongBowAttack longBowAttack = new LongBowAttack(this, attacklistAction);
            meleeAttack.idx = 1;
            longBowAttack.idx = 2;
            Abilities.Add(meleeAttack);
            Abilities.Add(longBowAttack);

            movelistAction.AddRange(attacklistAction);
            Heal heal = new Heal(this, movelistAction);
            heal.idx = 3;
            Abilities.Add(heal);
            currentHp = MaxHp;

        }

        public override void Response(UnitPresset target)
        {
            target.currentHp -= 1;
        }
    }
}
