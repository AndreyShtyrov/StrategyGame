using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Controller.Actions
{
    public class CoverUnit : IActions
    {
        public int idx { get ; set; }

        public UnitStatus Unit
        { get; set; }

        [JsonConstructor]
        public CoverUnit()
        { }

        public CoverUnit(UnitPresset unit)
        {
            idx = GameModeContainer.Get().ActionIdx;
            Unit = new UnitStatus(unit);
        }

        public virtual void forward()
        {
            var unit = GameModeContainer.Get().GetUnit(Unit.FieldPosition);
            unit.Refresh();
        }

        public virtual void reverse()
        {
            SetStateStates(Unit);
        }

        private void SetStateStates(UnitStatus savedUnit)
        {
            var unit = GameModeContainer.Get().GetUnit(savedUnit.FieldPosition);
            foreach (var savedStend in savedUnit.Stends)
            {
                var stand = unit.GetStand(savedStend.Idx);
                if (savedStend.State == ActionState.InProcess)
                {
                    stand.Active = savedStend.IsActive;
                    stand.point.ToInProgres();
                }
                if (savedStend.State == ActionState.Ended)
                {
                    stand.Active = savedStend.IsActive;
                    stand.point.Spend();
                }
            }
            foreach (var saveAbility in savedUnit.Abilities)
            {
                var ability = unit.GetAbility(saveAbility.Idx);
                if (saveAbility.State == ActionState.InProcess)
                {
                    ability.actionPoint.ToInProgres();
                }
                if (saveAbility.State == ActionState.Ended)
                {
                    ability.actionPoint.ToInProgres();
                }
            }
            if (savedUnit.Move == ActionState.InProcess)
            {
                unit.MoveActionPoint.ToInProgres();
            }
            if (savedUnit.Move == ActionState.Ended)
            {
                unit.MoveActionPoint.Spend();
            }
        }
    }
}
