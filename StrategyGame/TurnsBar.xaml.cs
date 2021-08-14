using Controller;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StrategyGame
{
    /// <summary>
    /// Interaction logic for TurnsBar.xaml
    /// </summary>
    public partial class TurnsBar : UserControl
    {
        public TurnsBar(List<TurnsSpeciffication> turns)
        {
            InitializeComponent();
            foreach (var turn in turns)
            {
                WeatherControl weather = new WeatherControl(turn);
                Bar.Children.Add(weather);
            }
        }
    }
}
