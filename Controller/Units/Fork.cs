using Controller.Abilities;
using Controller.Stands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Units
{
    class Fork:UnitPresset
    {

        public override string Name => "F";
        public override int MaxHp => 4;

        [JsonConstructor]
        public Fork(): base()
        { }

        public Fork((int X, int Y) fpos, Player owner) : base(fpos, owner)
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

            ForkStand fork = new ForkStand(this, attacklistAction1);
            fork.idx = 3;
            MeleeAttack melee = new MeleeAttack(this, attacklistAction2);
            melee.idx = 2;
            Abilities.Add(melee);
            Stends.Add(fork);

            var healList = new List<UnitActionPoint>(movelistAction);
            healList.AddRange(attacklistAction1);
            healList.AddRange(attacklistAction2);
            Heal heal = new Heal(this, movelistAction);
            heal.idx = 3;
            Abilities.Add(heal);
        }

    }
}
