using Controller.Actions;
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
            AbilityType = AbilityType.ActionWitoutTargetSelect;
            Name = "Heal";
        }

        public override bool IsReadyToUse()
        {
            return actionPoint.IsReady(unit.owner);
        }

        public override List<IActions> Use(UnitPresset target)
        {
            if (target.currentHp == target.MaxHp)
                return new List<IActions>() { };
            ChangeActionPointState changeAction = new ChangeActionPointState(
                unit.fieldPosition, idx, actionPoint.State, ActionState.Ended);
            SpendPlayerResources resources = new SpendPlayerResources(
                actionPoint.neededAttackPoints, actionPoint.neededMovePoints, unit.owner.idx);
            if (target.currentHp == 2 && target.MaxHp == 5)
            {
                target.currentHp = 5;


                return new List<IActions>() { new DealDamage(
                    unit.fieldPosition,
                    unit.fieldPosition,
                    idx,
                    -3), changeAction, resources};
            }
            int healDamage;
            if (target.MaxHp - target.currentHp < 2)
                healDamage = 1;
            else
                healDamage = 2;
            return new List<IActions>() { new DealDamage(
                    unit.fieldPosition,
                    unit.fieldPosition,
                    idx,
                    -healDamage), changeAction, resources };
        }

        public override void Return() => actionPoint.Return();

        public override void BreakAction() => actionPoint.Return();

    }
}
