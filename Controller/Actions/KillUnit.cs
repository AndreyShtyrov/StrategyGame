using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    public class KillUnit : IActions
    {
        public int idx { get; set; }

        public (int X, int Y) fieldPosition
        { get; set; }

        public string Name
        { get; set; }

        public int PlayerIndex
        { get; set; }

        [JsonConstructor]
        public KillUnit()
        { }

        public KillUnit(UnitPresset unit)
        {
            idx = GameModeContainer.Get().ActionIdx;
            fieldPosition = unit.fieldPosition;
            Name = unit.Name;
            PlayerIndex = unit.owner.idx;
        }

        public void reverse()
        {
            var conntroller = GameModeContainer.Get();
            var unit = UnitPresset.CreateUnit(Name, fieldPosition, Player.Get(PlayerIndex));
            conntroller.AddUnit(unit);
        }

        public void forward()
        {
            var conntroller = GameModeContainer.Get();
            var unit = conntroller.GetUnit(fieldPosition);
            conntroller.DeleteUnit(unit);
        }
    }
}
