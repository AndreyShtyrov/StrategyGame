using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    public class SpendPlayerResources : IActions
    {
        public int idx { get; set; }

        public int MoveActions { get; set; }

        public int AttackActions { get; set; }

        public int PlayerIdx { get; set; }

        [JsonConstructor]
        public SpendPlayerResources()
        { }

        public SpendPlayerResources(int AttackActions, int MoveActions, int Player)
        {
            this.MoveActions = MoveActions;
            this.AttackActions = AttackActions;
            this.PlayerIdx = Player;
        }

        public void forward()
        {
            var player = Player.Get(this.PlayerIdx);
            player.AttackPoints -= this.AttackActions;
            player.MovePoints -= this.MoveActions;
        }

        public void reverse()
        {
            var player = Player.Get(this.PlayerIdx);
            player.AttackPoints += this.AttackActions;
            player.MovePoints += this.MoveActions;
        }
    }
}
