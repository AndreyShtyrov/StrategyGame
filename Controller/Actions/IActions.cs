﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    public interface IActions
    {
        public int idx
        { get; set; }

        public void forward();

        public void reverse();
        
    }
}