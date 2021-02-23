using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Tokens;
using Controller.Stands;
using InterfaceOfObjects;
using Controller.Abilities;
using Controller.Units;
using Newtonsoft.Json;
using Controller.Actions;

namespace Controller
{
    public class UnitPresset : UnitData, IUnitPresset, INotifyPropertyChanged
    {

        public override int currentHp
        {
            set
            {
                if (base.currentHp > 0 && value < 0)
                { base.currentHp = 0; }
                else
                { base.currentHp = value; }
                OnPropertyChange("currentHp");
            }
            get
            {
                return base.currentHp;
            }
        }
        public int ResponseDamage
        { get; set; }
        public List<StandPresset> Stends = new List<StandPresset>();
        public List<AbilityPresset> Abilities = new List<AbilityPresset>();
        public float maxSpeed = 2f;
        public float currentSpeed;
        public Player owner
        { set; get; }
        public override (int X, int Y) fieldPosition
        {
            set
            {
                base.fieldPosition = value;
                OnPropertyChange("fieldPosition");
            }
            get
            {
                return base.fieldPosition;
            }
        }
        public bool isTarget
        {
            set
            {
                _isTarget = value;
                OnPropertyChange("isTarget");
            }
            get
            {
                return _isTarget;
            }
        }
        public override bool isSelected
        {
            get
            {
                return base.isSelected;
            }
            set
            {
                base.isSelected = value;
                OnPropertyChange("isSelected");
            }
        }
        public UnitActionPoint[] AttackPoints;
        public UnitActionPoint[] MovePoints;
        public ActionPoint MoveActionPoint;

        public event PropertyChangedEventHandler PropertyChanged;

        public UnitPresset() : base()
        { }

        public UnitPresset((int X, int Y) fpos, Player owner) : base(fpos)
        {
            var gameMode = GameModeContainer.Get();
            this.owner = owner;
            currentHp = 4;

            fieldPosition = fpos;
            AttackPoints = new UnitActionPoint[2];
            AttackPoints[0] = new UnitActionPoint(ActionName.Attack);
            AttackPoints[1] = new UnitActionPoint(ActionName.Attack);
            MovePoints = new UnitActionPoint[2];
            MovePoints[0] = new UnitActionPoint(ActionName.Move);
            MovePoints[1] = new UnitActionPoint(ActionName.Move);
            currentSpeed = maxSpeed;
            ResponseDamage = 2;
        }

        public void Move((int X, int Y) fpos, float distance)
        {
            fieldPosition = fpos;
            currentSpeed -= distance;
        }

        public void OnSpendActionsHandler()
        {
            List<int> ActionIndexs = new List<int>();
            if (AttackPoints[0].State != ActionState.Ready)
                ActionIndexs.Add(0);
            if (AttackPoints[1].State != ActionState.Ready)
                ActionIndexs.Add(1);
            if (MovePoints[0].State != ActionState.Ready)
                ActionIndexs.Add(2);
            if (MovePoints[0].State != ActionState.Ready)
                ActionIndexs.Add(3);
            PropertyChanged?.Invoke(ActionIndexs, new PropertyChangedEventArgs("UnitActionPoint"));
        }

        public virtual List<IActions> Response(UnitPresset target)
        {
            if (currentHp > 0)
                return new List<IActions>()
                {
                    new DealDamage(
                        fieldPosition,
                        target.fieldPosition,
                        1,
                        ResponseDamage)
                };
            else
            {
                return new List<IActions>();
            }
        }

        public void Refresh()
        {
            MoveActionPoint.Refresh();
            foreach (var ability in Abilities)
            {
                ability.actionPoint.Refresh();
            }
            foreach (var stand in Stends)
            {
                stand.Refresh();
            }

            currentSpeed = maxSpeed;
        }

        public void RefreshForRetreat()
        {
            MoveActionPoint.Refresh();
        }

        public AbilityPresset GetAbility(int idx)
        {
            foreach (var ability in Abilities)
            {
                if (ability.idx == idx)
                    return ability;
            }
            return null;
        }

        public StandPresset GetStand(int idx)
        {
            foreach (var stand in Stends)
            {
                if (stand.idx == idx)
                    return stand;
            }
            return null;
        }

        public int GetAbilityIndex(object action)
        {
            if (action is AbilityPresset)
            {
                for (int i = 0; i < Abilities.Count; i++)
                {
                    if (Abilities[i] == action)
                        return i;
                }
            }
            else
            {
                for (int i = 0; i < Stends.Count; i++)
                {
                    if (Stends[i] == action)
                        return i;
                }
            }
            return -1;
        }

        public static UnitPresset CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            if (name == "Helbard")
            {
                Halberd unit = new Halberd(fpos, owner);
                return unit;
            }
            if (name == "LongBow")
            {
                LongBow unit = new LongBow(fpos, owner);
                return unit;
            }
            if (name == "Fork")
            {
                Fork unit = new Fork(fpos, owner);
                return unit;
            }
            return null;
        }

        private void OnPropertyChange(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private bool _isTarget;
    }

    public struct UnitStatus
    {
        public List<(int Idx, bool IsActive, ActionState State)> Stends
        { get; set; }
        public List<(int Idx, ActionState State)> Abilities
        { get; set; }
        public int CurrentHp
        { get; set; }
        public ActionState Move
        { get; set; }
        public (int X, int Y) FieldPosition
        { get; set; }

        [JsonConstructor]
        public UnitStatus(
            List<(int Idx, bool IsActive, ActionState State)> stends,
            List<(int Idx, ActionState State)> abilities,
            int currentHp,
            ActionState move,
            (int X, int Y) fieldPosition)
        {
            Stends = stends;
            Abilities = abilities;
            CurrentHp = currentHp;
            Move = move;
            FieldPosition = fieldPosition;
        }

        public UnitStatus(UnitPresset unit)
        {
            Stends = new List<(int Stand, bool IsActive, ActionState State)>();
            foreach (var stend in unit.Stends)
            {
                Stends.Add((stend.idx, stend.Active, stend.point.State));
            }
            Abilities = new List<(int Ability, ActionState State)>();
            foreach (var ability in unit.Abilities)
            {
                Abilities.Add((ability.idx, ability.actionPoint.State));
            }
            Move = unit.MoveActionPoint.State;
            CurrentHp = unit.currentHp;
            FieldPosition = unit.fieldPosition;
        }
    }

}
