using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Requests
{
    public class MoveUnitRequest
    {
        public (int X, int Y) Selected
        { get; set; }
        public (int X, int Y) Target
        { get; set; }
    }
}
