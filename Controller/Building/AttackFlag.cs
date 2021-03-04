using Controller.Actions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Controller.Building
{
    public class AttackFlag : BuildingPresset
    {
        public override string Name => "AttackFlag";

        public override string fieldtype => "Flag";



        public override List<IActions> Capture(UnitPresset unit)
        {
            if (owner == unit.owner)
                return new List<IActions>();
            if (unit.currentHp > 0)
            {
                ChangeOwner changeOwner = new ChangeOwner(this, unit.owner);
                ChangeIncome changeIncome = new ChangeIncome(unit.owner, AttackPoint: 1);
                ChangeIncome changeIncome1 = new ChangeIncome(this.owner, AttackPoint: -1);
                return new List<IActions>() { changeOwner, changeIncome, changeIncome1};
            }
            return new List<IActions>();
        }

        public override List<IActions> Destoy()
        {
            return new List<IActions>();
        }

        public override SolidColorBrush getBackGround()
        {
            return Brushes.Red;
        }

        public override List<IActions> Use()
        {
            return new List<IActions>();
        }
    }
}
