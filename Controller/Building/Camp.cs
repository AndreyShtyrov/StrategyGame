using Controller.Actions;   
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Controller.Building
{
    public class Camp : BuildingPresset
    {

        public Camp((int X, int Y) fieldPosition, Player owner=null)
        {
            this.owner = owner;
            Controllable = false;
            if (owner == null)
                Destroyable = true;
            this.fieldPosition = fieldPosition;
        }

        public override string Name => "Camp";

        public override string fieldtype => throw new NotImplementedException();

        public override List<IActions> Capture()
        {
            throw new NotImplementedException();
        }

        public override List<IActions> Destoy()
        {
            throw new NotImplementedException();
        }

        public override SolidColorBrush getBackGround()
        {
            throw new NotImplementedException();
        }

        public override List<IActions> Use()
        {
            var unit = GameModeContainer.Get().GetUnit(fieldPosition);
            if (unit != null)
            {
                if (unit.currentHp < unit.MaxHp)
                {
                    DealDamage dealDamage = new DealDamage(fieldPosition, fieldPosition, -1, -1);
                    return new List<IActions>() { dealDamage };
                }
            }
            return new List<IActions>();
        }
    }
}
