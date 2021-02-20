using Controller.Actions;
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
            return actionPoint.IsReady(unit.owner);
        }

        public override List<IActions> Use(UnitPresset target)
        {
            return new List<IActions>() {
            new DealDamage(
                unit.fieldPosition,
                target.fieldPosition,
                idx,
                damage), new ChangeActionPointState(
                    unit.fieldPosition,
                    idx,
                    actionPoint.State,
                    ActionState.Ended),
            new SpendPlayerResources(
                actionPoint.neededAttackPoints,
                actionPoint.neededMovePoints,
                unit.owner.idx)};

        }

        public override void Return() => actionPoint.Return();

        public override void BreakAction() => actionPoint.Return();

    }
}
