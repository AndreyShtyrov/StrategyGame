using Controller.Actions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Abilities
{
    class BuildCamp : AbilityPresset
    {
        private UnitPresset unit;

        public BuildCamp(UnitPresset unit, List<UnitActionPoint> bindActionPoint)
        {
            this.unit = unit;
            actionPoint = new ActionPoint(unit, 0, 0, bindActionPoint);
            AbilityType = AbilityType.ActionWitoutTargetSelect;
            Name = "Camp";
        }

        public override void BreakAction()
        {
            throw new NotImplementedException();
        }

        public override bool IsReadyToUse()
        {
            return unit.HaveSupply;
        }

        public override void Return()
        {
            return;
        }

        public override List<IActions> Use(UnitPresset target)
        {
            Build build = new Build(unit.fieldPosition, "Camp");
            SpendPlayerResources spend = new SpendPlayerResources(0, 1, unit.owner.idx);
            return new List<IActions>() { build, spend };
        }
    }
}
