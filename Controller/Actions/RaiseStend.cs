using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    class RaiseStend : IActions
    {
        public int idx { get; set ; }

        public (int X, int Y) Source
        { get; set; }

        public bool IsActivated
        { get; set; }

        public int Stand
        { get; set; }

        [JsonConstructor]
        public RaiseStend()
        {   }

        
        public RaiseStend((int X, int Y) Source, int Stand, bool IsActivated)
        {
            idx = GameModeContainer.Get().ActionIdx;
            this.Source = Source;
            this.Stand = Stand;
            this.IsActivated = IsActivated;
        }

        public void forward()
        {
            var unit = GameModeContainer.Get().GetUnit(Source);
            var stand = unit.GetStand(Stand);
            if (IsActivated)
                stand.UpStand();
            else
                stand.DownStand();
        }

        public void reverse()
        {
            var unit = GameModeContainer.Get().GetUnit(Source);
            var stand = unit.GetStand(Stand);
            if (IsActivated)
                stand.DownStand();
            else
                stand.UpStand();
        }
    }
}
