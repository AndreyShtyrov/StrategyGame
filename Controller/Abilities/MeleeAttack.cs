using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Controller.Abilities
{
    public class MeleeAttack: AbilityPresset
    {
        private UnitPresset unit;
        public virtual int damage => 2;

        [JsonConstructor]
        public MeleeAttack()
        { }

        public MeleeAttack(UnitPresset unit, List<UnitActionPoint> bindActionPoint)
        {
            this.unit = unit;
            actionPoint = new ActionPoint(unit, 1, 0, bindActionPoint);
            Name = "Melee";
            AbilityType = AbilityType.Attack;
        }

        public override void PrepareToUse()
        {
            if (actionPoint.Active(unit.owner))
            {
                var controller = GameModeContainer.Get();
                CurrentRange = DeafaultRange;
                controller.selectedAbility = this;
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
