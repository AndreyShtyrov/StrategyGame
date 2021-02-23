using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    public class ChangeUnitStatus : IActions
    {
        public int idx
        { get; set; }

        public UnitStatus previousState
        { get; set;}
        
        public UnitStatus nextState
        { get; set; }

        [JsonConstructor]
        public ChangeUnitStatus()
        { }

        public ChangeUnitStatus(UnitPresset unit, int CurrentHp=0, int CurrentSpeed=0)
        {
            previousState = new UnitStatus(unit);
            var tnextState = new UnitStatus(unit);
            tnextState.CurrentHp -= CurrentHp;
            tnextState.Move -= CurrentSpeed;
            nextState = tnextState;
        }

        public void forward()
        {
            var unit = GameModeContainer.Get().GetUnit(previousState.FieldPosition);
            unit.currentHp = nextState.CurrentHp;

        }

        public void reverse()
        {
            throw new NotImplementedException();
        }
    }
}
