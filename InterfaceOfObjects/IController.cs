using System;
using System.Collections.Generic;
using System.Text;

namespace InterfaceOfObjects
{
    public interface IController
    {
        ITokenData getToken((int X, int Y) fpos);
        IUnitPresset getUnit((int X, int Y) fpos);
    }
}
