using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    public class CreateUnit : IActions
    {
        public int idx { get; set; }

        string Name
        { get; set; }

        (int X, int Y) fieldPosition
        { get; set; }
            
        public void forward()
        {
            throw new NotImplementedException();
        }

        public void reverse()
        {
            throw new NotImplementedException();
        }
    }
}
