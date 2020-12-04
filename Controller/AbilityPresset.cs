using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Controller
{
    public abstract class AbilityPresset
    {
        public string Name;
        public AbilityType AbilityType;

        private UnitPresset unit;

        public ActionPoint actionPoint;
        public int DeafaultRange = 1;
        public int CurrentRange;

        public abstract void Use(UnitPresset target);

        public abstract void PrepareToUse();

        public abstract void BreakActiono();

        public abstract void Return();
    }

}
