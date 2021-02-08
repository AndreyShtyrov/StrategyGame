using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Controller;
using InterfaceOfObjects;
using Newtonsoft.Json;

namespace Controller.Stands
{
    public class HalberdStand : StandPresset
    {
        public override int Damage => 2;
        private readonly UnitPresset unit;
        public new bool Active = false;

        [JsonConstructor]
        public HalberdStand()
        { }

        public HalberdStand(UnitPresset unit, List<UnitActionPoint> bindActionPoint)
        {
            this.unit = unit;
            point = new ActionPoint(unit, 1, 0, bindActionPoint);
            AbilityType = AbilityType.Attack;
            Name = "Halberd";
        }

        private bool CheckCorrectTargetsCondition(UnitPresset sender, UnitPresset target)
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
            Active = true;
            point.Active(unit.owner);
        }

        public override void DownStand()
        {
            Active = false;
            point.Return(unit.owner);
        }

        public override void Refresh()
        {
            point.Refresh();
        }

        public override void Use(UnitPresset sender, UnitPresset target)
        {
            if (unit.owner == sender.owner)
            {
                Active = false;
                target.currentHp -= Damage;
            }
            else
            {
                Active = false;
                sender.currentHp -= Damage;
            }
        }
    }
}
