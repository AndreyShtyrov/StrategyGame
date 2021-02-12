using Newtonsoft.Json;
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

        public (int X, int Y) Destination
        { get; set; }

        public ActionDirection Direction
        { get; set; }

        [JsonConstructor]
        public SpendActionPoints()
        { }

        public SpendActionPoints((int X, int Y) Source, int AbilityIndx)
        {
            idx = GameModeContainer.Get().ActionIdx;
            this.Source = Source;
            this.AbilityIndx = AbilityIndx;
        }

        public void forward()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            if (AbilityIndx == 0)
            { unit.MoveActionPoint.Spend(); return; }
            var ability = unit.GetAbility(AbilityIndx);
            ability.actionPoint.Spend();
        }

        public void reverse()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            if (AbilityIndx == 0)
            { unit.MoveActionPoint.Refresh(); return; }
            var ability = unit.GetAbility(AbilityIndx);
            ability.actionPoint.Refresh();
        }
    }
}
