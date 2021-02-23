using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Stands
{
    class ForkStand: HalberdStand
    {

        private readonly UnitPresset unit;

        public override int Damage => 2;


        public ForkStand(UnitPresset unit, List<UnitActionPoint> bindActionPoint): base(unit, bindActionPoint)
        {
            this.unit = unit;
            Active = false;
            point = new ActionPoint(unit, 1, 0, bindActionPoint);
            AbilityType = AbilityType.SelectAndAttack;
            Name = "Fork";
        }

        public override List<UnitPresset> GetAllTargets(UnitPresset sender, UnitPresset target)
        {
            List<UnitPresset> result = new List<UnitPresset>();
            if (sender.owner != unit.owner)
                result.Add(sender);
            else
                result.Add(target);
            var gameMode = GameModeContainer.Get();
            var units = gameMode.GetUnits();
            foreach (var selectedUnit in units)
            {
                if (selectedUnit == target)
                    continue;
                if (selectedUnit.owner != unit.owner)
                {
                    if (CheckCorrectTargetsCondition(selectedUnit, unit))
                        result.Add(selectedUnit);
                }
            }
            return result;
        }
    }
}
