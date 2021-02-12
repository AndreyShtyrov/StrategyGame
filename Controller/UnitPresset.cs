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


namespace Controller
{
    public class UnitPresset: UnitData, IUnitPresset, INotifyPropertyChanged
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
        public List<StandPresset> Stands = new List<StandPresset>();
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
        
        public UnitPresset():base()
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

        public virtual void Response(UnitPresset target)
        {
            target.currentHp -= 2;
        }

        public void Refresh()
        {
            MoveActionPoint.Refresh();
            foreach (var ability in Abilities)
            {
                ability.actionPoint.Refresh();
            }
            foreach (var stand in Stands)
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
            foreach (var stand in Stands)
            {
                if (stand.idx == idx)
                    return stand;
            }
            return null;
        }

        public int GetAbilityIndex(object action)
        {
            if (action is AbilityPresset )
            {
                for (int i=0; i< Abilities.Count; i++)
                {
                    if (Abilities[i] == action)
                        return i;
                }
            }
            else
            {
                for (int i = 0; i < Stands.Count; i++)
                {
                    if (Stands[i] == action)
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
            return null;
        }

        private void OnPropertyChange(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private bool _isTarget;
    }
}
