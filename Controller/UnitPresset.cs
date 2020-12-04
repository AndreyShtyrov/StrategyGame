using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Tokens;
using Controller.Stands;
using InterfaceOfObjects;
using Controller.Abilities;


namespace Controller
{
    public class UnitPresset: UnitData, IUnitPresset, INotifyPropertyChanged
    {
        
        public int currentHp
        {
            set
            {
                if (_currentHp > 0 && value < 0)
                { _currentHp = 0; }
                else
                { _currentHp = value; }
                OnPropertyChange("currentHp");
            }
            get
            {
                return _currentHp;
            }
        }
        public List<StandPresset> Stands = new List<StandPresset>();
        public List<AbilityPresset> Abilities = new List<AbilityPresset>();
        public float maxSpeed = 2f;
        public float currentSpeed;
        public Player owner;
        public override (int X, int Y) fieldPosition
        {
            set
            {
                _fieldPosition = value;
                OnPropertyChange("fieldPosition");
            }
            get
            {
                return _fieldPosition;
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
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChange("isSelected");
            }
        }
        public UnitActionPoint[] AttackPoints;
        public UnitActionPoint[] MovePoints;
        public ActionPoint MoveActionPoint;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public UnitPresset((int X, int Y) fpos, Player owner) : base(fpos)
        {
            var gameMode = GameMode.Get();
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
        

        private void OnPropertyChange(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private bool _isSelected = false;
        private bool _isTarget;
        private int _currentHp;
        private (int X, int Y) _fieldPosition;

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
    }
}
