﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Controller;
using InterfaceOfObjects;
using Newtonsoft.Json;
using Controller.Actions;

namespace Controller.Stands
{
    public class HalberdStand : StandPresset
    {
        public override int Damage => 2;
        private readonly UnitPresset unit;
        public override bool Active
        { get; set; }
        [JsonConstructor]
        public HalberdStand()
        { }

        public HalberdStand(UnitPresset unit, List<UnitActionPoint> bindActionPoint)
        {
            this.unit = unit;
            Active = false;
            point = new ActionPoint(unit, 1, 0, bindActionPoint);
            AbilityType = AbilityType.Attack;
            Name = "Halberd";
        }

        internal bool CheckCorrectTargetsCondition(UnitPresset sender, UnitPresset target)
        {
            var controller = GameModeContainer.Get();
            var pos = controller.TransformToCube(target.fieldPosition, unit.fieldPosition);
            if (Math.Abs(pos.X) + Math.Abs(pos.Y) + Math.Abs(pos.Z) == 2
                && unit != sender && unit != target)
            {
                return true;
            }
            return false;
        }

        public override bool CouldToReact(UnitPresset sender, UnitPresset target, BattleStage stage)
        {
            if (!Active)
                return false;
            if (stage == BattleStage.MainAttack && unit.owner == sender.owner)
            {
                return CheckCorrectTargetsCondition(sender, target);
            }
            if (stage == BattleStage.MainAttack && unit.owner == target.owner)
            {
                return CheckCorrectTargetsCondition(sender, target);
            }
            return false;
        }

        public override void UpStand()
        {
            if (point.IsReady(unit.owner))
            {
                Active = true;
                point.ToInProgres();
            }
        }

        public override void DownStand()
        {
            Active = false;
            point.Return();
        }

        public override void Refresh()
        {
            point.Refresh();
        }

        public override List<IActions> Use(UnitPresset sender, UnitPresset target)
        {
            ChangeActionPointState changeAction = new ChangeActionPointState(
                this.unit.fieldPosition, this.idx, point.State, ActionState.Ended);
            DealDamage dealDamage;
            if (target.owner != unit.owner)
            {
                dealDamage = new DealDamage(
                unit.fieldPosition,
                target.fieldPosition,
                this.idx,
                this.Damage);
            }
            else
            {
                dealDamage = new DealDamage(
                unit.fieldPosition,
                sender.fieldPosition,
                this.idx,
                this.Damage);
            }
            Active = false;
            return new List<IActions>() { dealDamage, changeAction};
        }
    }
}
