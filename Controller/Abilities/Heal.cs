using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Abilities
{
    public class Heal: AbilityPresset
    {
        private UnitPresset unit;
        public int damage = 2;

        public Heal(UnitPresset unit, List<UnitActionPoint> bindActionPoint)
        {
            this.unit = unit;
            actionPoint = new ActionPoint(unit, 0, 1, bindActionPoint);
            Name = "Heal";
        }

        public override void PrepareToUse()
        {
            if (actionPoint.Active(unit.owner))
            {
                Use(this.unit);
            }
        }

        public override void Use(UnitPresset target)
        {
            if (target.currentHp == 2 && target.MaxHp == 5)
            {
                target.currentHp = 5;
                return;
            }
            if (target.currentHp == target.MaxHp)
                return;
            if (target.MaxHp - target.currentHp < 2)
                target.currentHp += target.MaxHp - target.currentHp;
            else
                target.currentHp += 2;
            actionPoint.Spend();
        }

        public override void Return() => actionPoint.Return(unit.owner);

        public override void BreakActiono() => actionPoint.Return(unit.owner);
    }
}
