using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using InterfaceOfObjects;

namespace Tokens
{
    public class UnitData : TokenData, ITokenData
    {
        public virtual string Name => "K";
        public virtual int MaxHp => 4;
        public virtual int currentHp
        { get; set; }

        public override string fieldtype => nameof(UnitData);
        private bool _isSelected = false;
        public virtual bool isSelected 
        { get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
            }
        }

        public override SolidColorBrush getBackGround()
        {
            var template = Brushes.Blue;
            SolidColorBrush brash = new SolidColorBrush(template.Color);
            brash.Opacity = 0.2;
            return brash;
        }

        public virtual string getUnitName()
        {
            return Name;
        }


        public UnitData(): base()
        { }

        public UnitData((int X, int Y) fpos) : base(fpos)
        { }
    }
}