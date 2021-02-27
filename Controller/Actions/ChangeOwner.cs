using Controller.Building;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    public class ChangeOwner : IActions
    {
        public int idx { get; set; }

        public int PrevPlayer
        { get; set; }

        public int NextPlayer
        { get; set; }

        public (int X, int Y) Position
        { get; set; }

        [JsonConstructor]
        public ChangeOwner()
        { }

        public ChangeOwner(BuildingPresset building, Player newOwner)
        {
            idx = GameModeContainer.Get().ActionIdx;
            Position = building.fieldPosition;
            PrevPlayer = building.owner.idx;
            NextPlayer = newOwner.idx;
        }

        public void forward()
        {
            var building = GameModeContainer.Get().GetBuilding(Position);
            building.owner = Player.Get(NextPlayer);
        }

        public void reverse()
        {
            var building = GameModeContainer.Get().GetBuilding(Position);
            building.owner = Player.Get(PrevPlayer);
        }
    }
}
