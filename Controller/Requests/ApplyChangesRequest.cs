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
    }
}
