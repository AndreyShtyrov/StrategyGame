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

        public int SourceAbility
        { get; set; }

        public (int X, int Y) Destination
        { get; set; }

        public int idx { get; set; }

        [JsonConstructor]
        public DealDamage()
        { }

        public DealDamage((int X, int Y) Source, (int X, int Y) Destination  , int Ability) 
        {
            idx = GameModeContainer.Get().ActionIdx;
            this.Source = Source;
            this.SourceAbility = Ability;
            this.Destination = Destination;
        }

        public void forward()
        {
            var controller = GameModeContainer.Get();
            var attackUnit = controller.GetUnit(Source);
            var targetUnit = controller.GetUnit(Destination);
            var ability = attackUnit.GetAbility(SourceAbility);
            if (ability == null)
            {
                var stand = attackUnit.GetStand(SourceAbility);
                stand.Use(attackUnit, targetUnit);
            }
            else
            {
                ability.Use(targetUnit);
            }
        }

        public void reverse()
        {
            throw new NotImplementedException();
        }
    }
}
