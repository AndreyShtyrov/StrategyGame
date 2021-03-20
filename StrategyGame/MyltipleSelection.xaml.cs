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
using System.Windows.Shapes;
using DrawField;

namespace StrategyGame
{
    /// <summary>
    /// Interaction logic for MyltipleSelection.xaml
    /// </summary>
    public partial class MyltipleSelection : Window
    {
        public MyltipleSelection()
        {
            InitializeComponent();
        }

        public void AddUnit(UnitPresset unit, bool Attacker)
        {
            UnitIcon icon = new UnitIcon(unit);
            if (Attacker)
            {
                Allies.Children.Add(icon);
            }
            else
            {
                Enemies.Children.Add(icon);
            }
        }

        public void RemoveUnit(UnitPresset unit)
        {

        }
    }
}
