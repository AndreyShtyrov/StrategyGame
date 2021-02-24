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

        public float Distance
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
            var unit = GameModeContainer.Get().GetUnit(StartPosition);
            Distance = GameModeContainer.Get().GetPathToken(unit, EndPosition).pathLeght;
        }


        public void forward()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(StartPosition);
            unit.fieldPosition = EndPosition;
            unit.currentSpeed -= Distance;
        }

        public void reverse()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(EndPosition);
            unit.fieldPosition = StartPosition;
            unit.currentSpeed += Distance;
        }
    }
}
