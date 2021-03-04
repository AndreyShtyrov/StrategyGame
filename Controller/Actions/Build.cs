using Controller.Building;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    public class Build : IActions
    {
        public int idx { get ; set ; }

        public (int X, int Y) Position
        { get; set; }

        public string Name
        { get; set; }

        [JsonConsturctor]
        public Build()
        { }

        public Build((int X, int Y) fpos, String Name)
        {
            idx = GameModeContainer.Get().ActionIdx;
            Position = fpos;
            this.Name = Name;
        }

        public void forward()
        {
            var building = BuildingPresset.Build(Name, Position, null);
            GameModeContainer.Get().AddBuilding(building);
        }

        public void reverse()
        {
            var building = GameModeContainer.Get().GetBuilding(Position);
            GameModeContainer.Get().DeleteBuilding(building);
        }
    }
}
