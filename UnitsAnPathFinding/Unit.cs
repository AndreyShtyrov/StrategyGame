using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Grid;

namespace UnitsAnPathFinding
{
    public class Unit : TokenData
    {
        string name = "K";
        int maxHp = 4;
        int currentHp = 4;
        public override string fieldtype => nameof(Unit);

        public override SolidColorBrush getBackGround()
        {
            var template = Brushes.Gray;
            SolidColorBrush brash = new SolidColorBrush(template.Color);
            brash.Opacity = 0.15;
            return brash;
        }

        public virtual string getUnitName()
        {
            return name;
        }

        public Unit((int X, int Y) fpos): base(fpos)
        { }
    }
}
