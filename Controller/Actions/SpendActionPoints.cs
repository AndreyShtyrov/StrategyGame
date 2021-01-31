using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    class SpendActionPoints : IActions
    {
        public int idx 
        { get; set; }

        public int AbilityIndx
        { get; set; }

        public (int X, int Y) Source
        { get; set; }

        public ActionDirection Direction
        { get; set; }

        public void forward()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            var ability = unit.GetAbility(AbilityIndx);
            if (ability == null)
            {
                var stand = unit.GetStand(AbilityIndx);
                stand.point.Spend();
            }
            else
            {
                ability = unit.GetAbility(AbilityIndx);
                ability.actionPoint.Spend();
            }
        }

        public void reverse()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            var ability = unit.GetAbility(AbilityIndx);
            if (ability == null)
            {
                var stand = unit.GetStand(AbilityIndx);
                stand.point.Return(unit.owner);
            }
            else
            {
                ability = unit.GetAbility(AbilityIndx);
                ability.actionPoint.Return(unit.owner);
            }
        }
    }
}
