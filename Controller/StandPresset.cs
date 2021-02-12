using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Controller
{
    public abstract class  StandPresset
    {
        public abstract int Damage
        { get; }

        public int idx
        { get; set; }

        public abstract bool Active
        { get; set; }

        public string Name = "Stand";

        public ActionPoint point;

        public AbilityType AbilityType;

        public abstract bool CouldToReact(UnitPresset sender, UnitPresset target, BattleStage stage);

        public abstract void Use(UnitPresset sender, UnitPresset target);

        public virtual void UpStand()
        { }

        public abstract void Refresh();

        public virtual void DownStand()
        { }
    }
}
