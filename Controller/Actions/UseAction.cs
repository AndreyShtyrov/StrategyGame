using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    public class UseAction: IActions
    {
        public (int X, int Y) Source
        { get; set; }

        public int SourceAbility
        { get; set; }

        public ActionDirection Direction
        { get; set; }

        public int idx { get; set; }
 
        public UseAction()
        {
            idx = GameModeContainer.Get().ActionIdx;
        }

        public void forward()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            var ability = unit.GetAbility(SourceAbility);
            ability.Use(unit);
            GameModeContainer.Get().RefreshBacklight();
        }

        public void reverse()
        {
            throw new NotImplementedException();
        }
    }
}
