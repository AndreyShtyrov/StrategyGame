using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
using Tokens;

namespace Tokens.TokenFields
{
    public class Forest : TokenData
    {
        public override string fieldtype => nameof(Forest);

        public Forest((int X, int Y) fpos) : base(fpos)
        { }


        public override SolidColorBrush getBackGround()
        {
            return Brushes.DarkGreen;
        }

    }
}
