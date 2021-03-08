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
    /// Interaction logic for WeatherControl.xaml
    /// </summary>
    public partial class WeatherControl : UserControl
    {
        public WeatherControl(TurnSpeciffication turn)
        {
            InitializeComponent();
            if (turn.Weather == WeatherType.Normal)
            {
                BitmapImage image = new BitmapImage(new Uri(@"/StrategyGame;component/WeatherData/Normal.png", UriKind.Relative));
                WeatherImage.Source = image;
            }
            else if (turn.Weather == WeatherType.Rain)
            {
                BitmapImage image = new BitmapImage(new Uri(@"/StrategyGame;component/WeatherData/Rain.png", UriKind.Relative));
                WeatherImage.Source = image;
            }
        }
    }
}
