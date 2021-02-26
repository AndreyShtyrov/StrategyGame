using System;
using System.Collections.Generic;
using System.Text;

namespace Controller
{
    public class TurnSpeciffication
    {
        public WeatherType Weather;
        public int Number;
        public List<(int Number, Player owner, (int X, int Y) position)> Reinforcement;

    }

    public enum WeatherType
    {
        Normal = 1,
        Rain = 2,
        Hail = 3,
        Heat = 4,
    }

}
