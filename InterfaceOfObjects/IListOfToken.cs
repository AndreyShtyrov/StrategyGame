using System;
using System.Collections.Generic;
using System.Text;

namespace InterfaceOfObjects
{
    public interface IListOfToken
    {
        public List<ITokenData> grids
        { get; set; }

        public ITokenData getData((int X, int Y) fpos);

        public int width
        { get; set; }
        public int height
        { get; set; }
    }
}
