using Controller.Actions;
using System;
using System.Collections.Generic;
using System.Text;


namespace Controller
{
    public class TurnsSpeciffication
    {
        public WeatherType Weather
        { get; set; }
        public int ActionIndex
        { get; set; }
        public List<(int Number, Player owner, (int X, int Y) position)> Reinforcement
        { get; set; }
        public List<QuerryAction> TurnsPlayer1
        { get; set; }
        public List<QuerryAction> TurnsPlayer2
        { get; set; }

        public int CurrentPlayerIndex
        { get; set; }

        [JsonConsturctor]
        public TurnsSpeciffication()
        { }

        public List<QuerryAction> GetTurns(int PlayerIndex)
        {
            if (CurrentPlayerIndex == PlayerIndex)
            {
                return TurnsPlayer1;
            }
            else
            {
                return TurnsPlayer2;

            }
        }

        public TurnsSpeciffication(WeatherType weather, int number)
        {
            Weather = weather;
            ActionIndex = number;
            Reinforcement = new List<(int Number, Player owner, (int X, int Y) position)>();
        }

        public TurnsSpeciffication(WeatherType weather, int number, List<(int Number, Player owner, (int X, int Y) position)> ps)
        {
            Weather = weather;
            ActionIndex = number;
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
