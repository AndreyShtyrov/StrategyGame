using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;

namespace Controller.Actions
{
    class DealDamage : IActions
    {
        public (int X, int Y) Source
        { get; set; }

        public int Damage
        { get; set; }

        public int SourceAbility
        { get; set; }

        public (int X, int Y) Destination
        { get; set; }

        public int idx { get; set; }

        [JsonConstructor]
        public DealDamage()
        { }

        public DealDamage((int X, int Y) Source, (int X, int Y) Destination  , int Ability, int Damage) 
        {
            idx = GameModeContainer.Get().ActionIdx;
            this.Source = Source;
            this.SourceAbility = Ability;
            this.Destination = Destination;
            this.Damage = Damage;
        }

        public void forward()
        {
            var controller = GameModeContainer.Get();
            var targetUnit = controller.GetUnit(Destination);
            targetUnit.currentHp -= Damage;
        }

        public void reverse()
        {
            var targetUnit = GameModeContainer.Get().GetUnit(Destination);
            targetUnit.currentHp += Damage;
        }
    }
}
