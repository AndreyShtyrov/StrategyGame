using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    class SpendActionPoints : IActions
    {
        public int idx 
        { get; set; }

        public int ActionsPoint
        { get; set; }

        public (int X, int Y) Source
        { get; set; }

        public void forward()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            var ability = unit.GetAbility(ActionsPoint);
            if (ability == null)
            {
                var stand = unit.GetStand(ActionsPoint);
                stand.point.Spend();
            }
            else
            {
                ability = unit.GetAbility(ActionsPoint);
                ability.actionPoint.Spend();
            }
        }

        public void reverse()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            var ability = unit.GetAbility(ActionsPoint);
            if (ability == null)
            {
                var stand = unit.GetStand(ActionsPoint);
                stand.point.Return(unit.owner);
            }
            else
            {
                ability = unit.GetAbility(ActionsPoint);
                ability.actionPoint.Return(unit.owner);
            }
        }
    }
}
