using Controller.Actions;
using InterfaceOfObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Controller.Building
{
    public abstract class BuildingPresset: ITokenData
    {
        public Player owner;
        public bool Controllable;
        public bool Destroyable;
        public abstract string Name
        { get; }

        public abstract string fieldtype
        { get; }

        public (int X, int Y) fieldPosition
        { get; set; }

        public abstract SolidColorBrush getBackGround();

        static
        public BuildingPresset Build(string Name, (int X, int Y) fpos, Player owner)
        {
            if(Name == "Camp")
            {
                BuildingPresset building = new Camp(fpos, owner);
                return building;
            }
            return null;
        }

        public abstract List<IActions> Destoy();

        public abstract List<IActions> Capture();

        public abstract List<IActions> Use();
    }
}
