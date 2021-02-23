using Controller.Abilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Units
{
    public class Buckler : UnitPresset
    {
        public override string Name => "B";
        public override int MaxHp => 3;

        [JsonConstructor]
        public Buckler() : base()
        { }

        private ActionPoint _MoveActionPoint;

        public override ActionPoint MoveActionPoint
        {
            get
            {
                if (_MoveActionPoint.State == ActionState.Ready)
                {
                    return _MoveActionPoint;
                }
                else
                {
                    currentSpeed = 1;
                    return AdditionMovePoint;
                }
            }
            set
            {
                if (_MoveActionPoint.State == ActionState.Ready )
                {
                    _MoveActionPoint = value;
                }
                else
                {
                    currentSpeed = 1;
                    AdditionMovePoint = value;
                }
            }
        }

        private ActionPoint AdditionMovePoint;

        public Buckler((int X, int Y) fpos, Player owner): base(fpos, owner)
        {
            maxSpeed = 2f;
            currentHp = MaxHp;
            List<UnitActionPoint> movelistActions = new List<UnitActionPoint>();
            movelistActions.Add(MovePoints[0]);
            List<UnitActionPoint> movelistActions1 = new List<UnitActionPoint>();
            List<UnitActionPoint> attackListAction = new List<UnitActionPoint>();
            movelistActions1.Add(MovePoints[1]);
            attackListAction.Add(AttackPoints[0]);
            attackListAction.Add(AttackPoints[1]);
            _MoveActionPoint = new ActionPoint(this, 0, 1, movelistActions);
            AdditionMovePoint = new ActionPoint(this, 0, 1, movelistActions1);
            currentSpeed = maxSpeed;
            MeleeAttack melee = new MeleeAttack(this, attackListAction);
            melee.idx = 2;
            Abilities.Add(melee);

            var healList = new List<UnitActionPoint>(movelistActions);
            healList.AddRange(attackListAction);

            Heal heal = new Heal(this, healList);
            heal.idx = 3;
            Abilities.Add(heal);
        }

    }
}
