using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    class PrepareToUse : IActions
    {
        public (int X, int Y) Source
        {get; set;}

        public int SourceAbility
        { get; set; }

        public ActionDirection Direction
        { get; set; }

        public int idx { get ; set ; }

        public PrepareToUse()
        {
            idx = GameModeContainer.Get().ActionIdx;
        }

        public void forward()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            var ability = unit.GetAbility(SourceAbility);
            if (ability ==null)
            {
                var stand = unit.GetStand(SourceAbility);
                stand.UpStand();
            }
            else
            {
                ability.PrepareToUse();
                GameModeContainer.Get().BacklightTargets(unit, ability);
            }
        }

        public void reverse()
        {
            throw new NotImplementedException();
        }
    }
}
