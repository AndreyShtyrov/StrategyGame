using System;
using System.Collections.Generic;
using System.Text;
using Controller.Actions;

namespace Controller.Requests
{
    public class ApplyChangesRequest
    {
        public List<IActions> Actions
        { get; set; }


        public string Name
        { get; set; }

        public (int X, int Y) fieldPosition
        { get; set; }

        public int Player
        { get; set; }


    }
}
