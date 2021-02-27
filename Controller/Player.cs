using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Controller
{
    public sealed class Player : INotifyPropertyChanged
    {
        private static List<Player> instances = new List<Player>();
        private int _AttackPoints;
        private int _MovePoints;
        private int _IncomeMovePoints;
        private int _IncomeAttackPoints;

        public readonly int idx;

        public event PropertyChangedEventHandler PropertyChanged;

        public int IncomeAttackPoints
        {
            set
            {
                _IncomeAttackPoints = value;
                OnPropertyChanged("AttackPoint");
            }
            get
            {
                return _IncomeAttackPoints;
            }
        }
        public int IncomeMovePoints
        {
            set
            {
                _IncomeMovePoints = value;
                OnPropertyChanged("AttackPoint");
            }
            get
            {
                return _IncomeMovePoints;
            }
        }
        public int AttackPoints
        {
            set
            {
                _AttackPoints = value;
                OnPropertyChanged("AttackPoints");
            }
            get
            {
                return _AttackPoints;
            }
        }
        public int MovePoints
        {
            set
            {
                _MovePoints = value;
                OnPropertyChanged("MovePoints");
            }
            get
            {
                return _MovePoints;
            }
        }
        public int BaseIncomeAttackPoints;
        public int BaseIncomeMovePoints;
        public int CurrentTurnNumber
        { get; set; }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static Player Get(int idx)
        {
            foreach (var instance in instances)
            {
                if (instance.idx == idx)
                    return instance;
            }
            throw new NotImplementedException(" Player with this index not define yet");
        }

        public static void Create(int idx, int baseMove = 5, int baseAttack = 5)
        {
            Player player = new Player(idx, baseMove, baseAttack);
            instances.Add(player);
        }

        Player(int idx, int baseMove, int baseAttack)
        {
            this.idx = idx;
            BaseIncomeMovePoints = baseMove;
            BaseIncomeAttackPoints = baseAttack;
            IncomeAttackPoints = 0;
            IncomeMovePoints = 0;
        }

        public void getIncome()
        {
           
            AttackPoints = IncomeAttackPoints;
            MovePoints = IncomeMovePoints;
        }
    }
}
