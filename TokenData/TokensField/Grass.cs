using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
using Tokens;

namespace Tokens.TokenFields
{
    public class Grass: TokenData
    {
        public override string fieldtype => nameof(Grass);


        public Grass((int X, int Y) fpos): base(fpos)
        { }

        public override SolidColorBrush getBackGround()
        {
            return Brushes.Green;
        }

    }
}
