using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    public class ChangeActionPointState : IActions
    {
        public int idx 
        { get; set; }

        public int AbilityIndx
        { get; set; }

        public (int X, int Y) Source
        { get; set; }

        public (int X, int Y) Destination
        { get; set; }

        public ActionState StartState
        { get; set; }

        public ActionState EndState
        { get; set; }

        public ActionDirection Direction
        { get; set; }

        [JsonConstructor]
        public ChangeActionPointState()
        { }

        public ChangeActionPointState((int X, int Y) Source, int AbilityIndx, ActionState StartState,ActionState EndState)
        {
            idx = GameModeContainer.Get().ActionIdx;
            this.Source = Source;
            this.AbilityIndx = AbilityIndx;
            this.StartState = StartState;
            this.EndState = EndState;
        }

        public void forward()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            if (AbilityIndx == 0)
            { ChangeState(unit.MoveActionPoint, EndState); return; }
            var ability = unit.GetAbility(AbilityIndx);
            if (ability == null)
            {
                var stend = unit.GetStand(AbilityIndx);
                ChangeState(stend.point, EndState);
                return;
            }
            ChangeState(ability.actionPoint, EndState);
        }

        public void reverse()
        {
            var controller = GameModeContainer.Get();
            var unit = controller.GetUnit(Source);
            if (AbilityIndx == 0)
            { ChangeState(unit.MoveActionPoint, StartState); return; }
            var ability = unit.GetAbility(AbilityIndx);
            if (ability == null)
            {
                var stend = unit.GetStand(AbilityIndx);
                ChangeState(stend.point, StartState);
                return;
            }
            ChangeState(ability.actionPoint, StartState);
        }

        private void ChangeState(ActionPoint point, ActionState state)
        {
            if (state == ActionState.Ended)
                point.Spend();
            else if (state == ActionState.InProcess)
                point.ToInProgres();
            else 
                point.Return();
        }
    }
}
