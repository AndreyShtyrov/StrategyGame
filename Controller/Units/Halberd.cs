using System;
using System.Collections.Generic;
using System.Text;
using Controller.Abilities;
using Controller.Stands;
using Newtonsoft.Json;

namespace Controller.Units
{
    public class Halberd : UnitPresset
    {
        public override string Name => "A";
        public override int MaxHp => 4;

        [JsonConstructor]
        public Halberd() : base()
        { }

        public Halberd((int X, int Y) fpos, Player owner):base(fpos, owner)
        {
            maxSpeed = 2;

            currentHp = MaxHp;
            List<UnitActionPoint> movelistAction = new List<UnitActionPoint>();
            movelistAction.Add(MovePoints[0]);
            movelistAction.Add(MovePoints[1]);
            List<UnitActionPoint> attacklistAction1 = new List<UnitActionPoint>();
            attacklistAction1.Add(AttackPoints[0]);
            List<UnitActionPoint> attacklistAction2 = new List<UnitActionPoint>();
            attacklistAction2.Add(AttackPoints[1]);
            MoveActionPoint = new ActionPoint(this, 0, 1, movelistAction);
            Stands.Add(new HalberdStand(this, attacklistAction1));
            Abilities.Add(new MeleeAttack(this, attacklistAction2));
            var healList = new List<UnitActionPoint>(movelistAction);
            healList.AddRange(attacklistAction1);
            healList.AddRange(attacklistAction2);
            Abilities.Add(new Heal(this, movelistAction));
        }
    }
}
