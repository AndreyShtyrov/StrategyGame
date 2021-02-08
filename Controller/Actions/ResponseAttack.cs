using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    class ResponseAttack
    {
        public (int X, int Y) Source
        { get; set; }

        public (int X, int Y) Destination
        { get; set; }

        public int SourceAbility
        { get; set; }

        public ActionDirection Direction
        { get; set; }

        public int idx { get; set; }

        public ResponseAttack()
        {
            idx = GameModeContainer.Get().ActionIdx;
        }

        public void forward()
        {
            var controller = GameModeContainer.Get();
            var attackUnit = controller.GetUnit(Source);
            var targetUnit = controller.GetUnit(Destination);
            attackUnit.Response(targetUnit);
        }

        public void reverse()
        {
            throw new NotImplementedException();
        }
    }
}
