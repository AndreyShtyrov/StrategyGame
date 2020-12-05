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
        private readonly int Damage = 2;
        private readonly UnitPresset unit;
        public bool active = false;

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

        public override bool CouldToReact(UnitPresset sender, UnitPresset target)
        {
            if (!active)
                return false;
            var controller = GameModeServer.Get();
            var pos = controller.TransformToCube(target.fieldPosition, unit.fieldPosition);
            if (Math.Abs(pos.X) + Math.Abs(pos.Y) + Math.Abs(pos.Z) == 2 
                && unit != sender && unit != target)
            {
                return true;
            }
            return false;
        }

        public override void UpStand()
        {
            if (unit.MoveActionPoint.State != ActionState.Ready)
                unit.MoveActionPoint.Spend();
            if (!active)
            {
                active = true;
                point.Active(unit.owner);
            }
            else
            {
                active = false;
                point.Return(unit.owner);
            }
            point.Spend();
        }

        public override void DownStand()
        {
            GameModeServer.Get().State = GameModeState.AwaitSelect;
            active = false;
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
                active = false;
                target.currentHp -= Damage;
            }
            else
            {
                active = false;
                sender.currentHp -= Damage;
            }
        }
    }
}
