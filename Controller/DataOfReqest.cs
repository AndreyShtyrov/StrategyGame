using System;
using System.Collections.Generic;
using System.Text;
using UnitsAnPathFinding;

namespace Controller
{
    public class DataOfReqest
    {

        public Player outCome;
        public UnitPresset Selected;
        public UnitPresset Target;
        public PathToken pathToken;
        public List<PathToken> pathTokens;

        public List<UnitPresset> units;
    }
}
