using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
using Tokens;

namespace Tokens.TokenFields
{
    public class Water : TokenData
    {
        public override string fieldtype => nameof(Water);


        public Water((int X, int Y) fpos) : base(fpos)
        { }


        public override SolidColorBrush getBackGround()
        {
            return Brushes.Blue;
        }
    }
}
