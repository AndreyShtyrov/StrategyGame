using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    class DealDamage : IActions
    {
        public (int X, int Y) Source
        { get; set; }

        public int SourceAbility
        { get; set; }

        public (int X, int Y) Destination
        { get; set; }

        public int idx { get; set; }

        public DealDamage()
        {
            idx = GameModeContainer.Get().ActionIdx;
        }

        public void forward()
        {
            var controller = GameModeContainer.Get();
            var attackUnit = controller.GetUnit(Source);
            var targetUnit = controller.GetUnit(Source);
            var ability = attackUnit.GetAbility(SourceAbility);
            if (ability == null)
            {
                var stand = attackUnit.GetStand(SourceAbility);
                stand.Use(attackUnit, targetUnit);
            }
            else
            {
                ability.Use(targetUnit);
            }
            
        }

        public void reverse()
        {
            throw new NotImplementedException();
        }
    }
}
