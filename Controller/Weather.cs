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

        public TurnSpeciffication(WeatherType weather, int number)
        {
            Weather = weather;
            Number = number;
            Reinforcement = new List<(int Number, Player owner, (int X, int Y) position)>();
        }

        public TurnSpeciffication(WeatherType weather, int number, List<(int Number, Player owner, (int X, int Y) position)> ps)
        {
            Weather = weather;
            Number = number;
            Reinforcement = ps;
        }
    }


    public enum WeatherType
    {
        Normal = 1,
        Rain = 2,
        Hail = 3,
        Heat = 4,
    }

}
