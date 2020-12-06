using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Requests
{
    public class SelectTargetRequest
    {
        public (int X, int Y) Selected
        { get; set; }

        public List<(int X, int Y)> Targets
        { get; set; }
    }
}
