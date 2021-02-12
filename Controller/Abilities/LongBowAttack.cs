using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Abilities
{
    public class LongBowAttack: AbilityPresset
    {
        private UnitPresset unit;
        public readonly int damage = 1;

        [JsonConstructor]
        public LongBowAttack() : base()
        { }

        public LongBowAttack(UnitPresset unit, List<UnitActionPoint> bindActionPoint)
        {
            this.unit = unit;
            actionPoint = new ActionPoint(unit, 1, 0, bindActionPoint);
            Name = "LongBow";
            AbilityType = AbilityType.RangeAttack;
            DeafaultRange = 3;
        }

        public override bool IsReadyToUse()
        {
            return actionPoint.Active(unit.owner);
        }

        public override void Use(UnitPresset target)
        {
            target.currentHp -= damage;
            actionPoint.Spend();
        }

        public override void Return() => actionPoint.Return(unit.owner);

        public override void BreakAction() => actionPoint.Return(unit.owner);

    }
}
