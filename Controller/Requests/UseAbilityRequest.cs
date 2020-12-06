using System;
using System.Collections.Generic;
using System.Text;
using Controller.Actions;

namespace Controller.Requests
{
    public class UseAbilityRequest
    {
        public List<IActions> Actions
        { get; set; }
        public (int X, int Y) Selected
        { get; set; }

        public (int X, int Y) Target
        { get; set; }

        public int AbilityIdx
        { get; set; }
    }
}
