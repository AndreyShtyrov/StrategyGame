using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Controller;

namespace StrategyGame
{
    /// <summary>
    /// Interaction logic for PlayerWindow.xaml
    /// </summary>
    public partial class PlayerWindow : UserControl
    {
        Player data;
        public PlayerWindow(int idx)
        {
            InitializeComponent();
            data = Player.getPlayer(idx);
            data.PropertyChanged += OnPropertyChange;
            PlayerName.Content = "Player " + idx.ToString();
            MoveP.Content = data.AttackPoints.ToString();
            AttackP.Content = data.AttackPoints.ToString();
        }

        private void OnPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AttackPoints")
            {
                AttackP.Content = data.AttackPoints.ToString();
            }
            if (e.PropertyName == "MovePoints")
            {
                MoveP.Content = data.MovePoints.ToString();
            }
        }

    }
}
