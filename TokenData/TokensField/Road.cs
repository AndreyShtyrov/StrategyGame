using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
using Tokens;

namespace Tokens.TokenFields
{
    public class Road : TokenData
    {
        public override string fieldtype => nameof(Road);


        public Road((int X, int Y) fpos) : base(fpos)
        { }


        public override SolidColorBrush getBackGround()
        {
            return Brushes.Brown;
        }
    }
}
