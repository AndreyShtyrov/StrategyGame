using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Abilities
{
    public class WeakMeleeAttack: MeleeAttack
    {
        public override int damage => 1;

        [JsonConstructor]
        public WeakMeleeAttack() : base()
        { }

        public WeakMeleeAttack(UnitPresset unit, List<UnitActionPoint> bindActionPoint) : base(unit, bindActionPoint)
        {
        }

    }
}
