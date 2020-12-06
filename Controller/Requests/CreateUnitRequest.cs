using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Requests
{
    public class CreateUnitRequest
    {
        public string Name
        { get; set; }

        public (int X, int Y) fieldPosition
        { get; set; }
    }
}
