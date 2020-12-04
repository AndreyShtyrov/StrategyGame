using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Text;
using System.Windows.Media;

namespace Grid
{
    public interface ITokenData
    {
        public string fieldtype
        {get;}
        public (int X, int Y) fieldPosition
        { get; }

        public SolidColorBrush getBackGround();
    }
}
