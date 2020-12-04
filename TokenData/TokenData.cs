using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using InterfaceOfObjects;


namespace Tokens
{
    public class TokenData : ITokenData
    {

        private (int X, int Y) _fieldPosition;


        public virtual string fieldtype
        {
            get
            {
                return "Empty";
            }
        }

        public virtual (int X, int Y) fieldPosition
        {
            set
            {
                _fieldPosition = value;
            }
            get
            {
                return _fieldPosition;
            }
        }


        public virtual SolidColorBrush getBackGround()
        {
            return Brushes.White;
        }

        [JsonConstructor]
        public TokenData()
        {
        }
        
        public TokenData((int X, int Y) fpos)
        {
            _fieldPosition = fpos;
        }
    }
}
