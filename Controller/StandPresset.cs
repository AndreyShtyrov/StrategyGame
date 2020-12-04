using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Controller
{
    public abstract class  StandPresset
    {
        public int damage = 2;

        public string Name = "Stand";

        public ActionPoint point;

        public AbilityType AbilityType;

        public abstract bool CouldToReact(UnitPresset sender, UnitPresset target);

        public abstract void Use(UnitPresset sender, UnitPresset target);

        public virtual void UpStand()
        { }

        public abstract void Refresh();

        public virtual void DownStand()
        { }
    }
}
