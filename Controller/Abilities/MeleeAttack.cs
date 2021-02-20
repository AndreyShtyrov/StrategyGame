using Controller.Actions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Controller.Abilities
{
    public class MeleeAttack : AbilityPresset
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

        public override bool IsReadyToUse()
        {
            return actionPoint.IsReady(unit.owner);
        }

        public override List<IActions> Use(UnitPresset target)
        {

            DealDamage dealDamage = new DealDamage(
                unit.fieldPosition,
                target.fieldPosition,
                idx,
                damage);
            ChangeActionPointState point = new ChangeActionPointState(
                unit.fieldPosition,
                    idx,
                    actionPoint.State,
                    ActionState.Ended);
            SpendPlayerResources resources = new SpendPlayerResources(
                actionPoint.neededAttackPoints,
                actionPoint.neededMovePoints,
                unit.owner.idx);
            return new List<IActions>() {dealDamage, point, resources};
        }

        public override void Return() => actionPoint.Return();

        public override void BreakAction() => actionPoint.Return();
    }
}
