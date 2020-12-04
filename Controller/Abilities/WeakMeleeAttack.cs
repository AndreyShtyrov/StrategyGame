using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Abilities
{
    public class WeakMeleeAttack: MeleeAttack
    {
        public override int damage => 1;

        public WeakMeleeAttack(UnitPresset unit, List<UnitActionPoint> bindActionPoint) : base(unit, bindActionPoint)
        {
        }

    }
}
