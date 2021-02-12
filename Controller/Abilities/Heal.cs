using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Abilities
{
    public class Heal : AbilityPresset
    {
        private UnitPresset unit;
        public int damage = 2;

        [JsonConstructor]
        public Heal()
        { }

        public Heal(UnitPresset unit, List<UnitActionPoint> bindActionPoint)
        {
            this.unit = unit;
            actionPoint = new ActionPoint(unit, 0, 1, bindActionPoint);
            AbilityType = AbilityType.Heal;
            Name = "Heal";
        }

        public override bool IsReadyToUse()
        {
            return actionPoint.Active(unit.owner);
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

        public override void BreakAction() => actionPoint.Return(unit.owner);

    }
}
