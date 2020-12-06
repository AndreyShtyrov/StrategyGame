using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    class HightLightTargets : IActions
    {
        public (int X, int Y) Source
        { get; set; }

        public List<(int X, int Y)> Targets
        { set; get; }

        public int idx { get; set; }

        public HightLightTargets()
        {
            idx = GameModeContainer.Get().ActionIdx;
        }

        public void forward()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            unit.isSelected = true;
            foreach (var target in Targets)
            {
                unit = controller.GetUnit(target);
                unit.isTarget = true;
            }
        }

        public void reverse()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            unit.isSelected = false;
            foreach (var target in Targets)
            {
                unit = controller.GetUnit(target);
                unit.isTarget = true;
            }
        }
    }
}
