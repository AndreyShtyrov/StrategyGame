using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Controller.Actions;

namespace Controller
{
    public abstract class AbilityPresset
    {
        public string Name;
        public AbilityType AbilityType;

        public int idx
        { get; set; }

        private UnitPresset unit;

        public ActionPoint actionPoint;
        public int DeafaultRange = 1;
        public int CurrentRange;

        public abstract List<IActions> Use(UnitPresset target);

        public abstract bool IsReadyToUse();

        public abstract void BreakAction();

        public abstract void Return();
    }

}
