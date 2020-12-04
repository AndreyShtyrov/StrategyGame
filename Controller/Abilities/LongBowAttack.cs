using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Abilities
{
    public class LongBowAttack: AbilityPresset
    {
        private UnitPresset unit;
        private readonly int damage = 1;

        public LongBowAttack(UnitPresset unit, List<UnitActionPoint> bindActionPoint)
        {
            this.unit = unit;
            actionPoint = new ActionPoint(unit, 1, 0, bindActionPoint);
            Name = "LongBow";
            AbilityType = AbilityType.RangeAttack;
            DeafaultRange = 3;
        }

        public override void PrepareToUse()
        {
            if (actionPoint.Active(unit.owner))
            {
                var controller = GameMode.Get();
                CurrentRange = DeafaultRange;
                controller.selectedAbility = this;
                controller.State = GameModeState.SelectEnemy;
                GameTableController.Get().State = GameTableState.AwaitSelectTarget;
            }
        }

        public override void Use(UnitPresset target)
        {
            target.currentHp -= damage;
            actionPoint.Spend();
        }

        public override void Return() => actionPoint.Return(unit.owner);

        public override void BreakActiono() => actionPoint.Return(unit.owner);

    }
}
