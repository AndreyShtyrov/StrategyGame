using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    class MoveUnit : IActions
    {
        public (int X, int Y) StartPosition
        { get; set; }

        public (int X, int Y) EndPosition
        { get; set; }

        public int idx { get; set; }

        public ActionDirection Direction
        { get; set; }

        [JsonConstructor]
        public MoveUnit()
        {
            
        }

        public MoveUnit((int X, int Y) StartPosition, (int X, int Y) EndPosition)
        {
            idx = GameModeContainer.Get().ActionIdx;
            this.StartPosition = StartPosition;
            this.EndPosition = EndPosition;
        }


        public void forward()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(StartPosition);
            var distance = controller.GetPathToken(unit, EndPosition).pathLeght;
            unit.Move(EndPosition, distance);
        }

        public void reverse()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(EndPosition);
            unit.Move(StartPosition, 0);
            var distance = controller.GetPathToken(unit, EndPosition).pathLeght;
            unit.Move(StartPosition, -distance);
        }
    }
}
