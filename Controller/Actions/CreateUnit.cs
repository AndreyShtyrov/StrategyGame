using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Controller;

namespace Controller.Actions
{
    public class CreateUnit : IActions
    {
        public int idx { get; set; }

        public string Name
        { get; set; }

        public (int X, int Y) fieldPosition
        { get; set; }

        public int PlayerIndex
        { get; set; }

        [JsonConstructor]
        public CreateUnit()
        { }

        public CreateUnit(string Name, (int X, int Y) fpos, int playerIndex)
        {
            idx = GameModeContainer.Get().ActionIdx;
            this.Name = Name;
            this.fieldPosition = fpos;
            this.PlayerIndex = playerIndex;
        }

        public void forward()
        {
            var conntroller = GameModeContainer.Get();
            var unit = UnitPresset.CreateUnit(Name, fieldPosition, Player.getPlayer(PlayerIndex));
            conntroller.AddUnit(unit);
        }

        public void reverse()
        {
            var conntroller = GameModeContainer.Get();
            var unit = conntroller.GetUnit(fieldPosition);
            conntroller.DeleteUnit(unit);
        }
    }
}
