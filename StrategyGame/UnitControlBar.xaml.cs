﻿using System;
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
    /// Interaction logic for UnitControlBar.xaml
    /// </summary>
    public partial class UnitControlBar : UserControl
    {
        public UnitControlBar()
        {
            InitializeComponent();
        }

        public void OnSelectedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Selected")
                return;
            var unit = GameTableController.Get().Selected;
            if (unit != null)
            {
                foreach (var ability in unit.Abilities)
                {
                    Button button = new Button();
                    button.Content = ability.Name;
                    button.Click += 
                        (object sender, RoutedEventArgs e) 
                        => GameTableController.Get().
                        SelectedUnitActivateAbility(ability);
                    AbilityStack.Children.Add(button);
                }
                foreach ( var stand in unit.Stends)
                {
                    Button button = new Button();
                    button.Content = stand.Name;
                    button.Click += 
                        (object sender, RoutedEventArgs e) 
                        => GameTableController.Get().
                        SelectedUnitRaiseStand(stand);
                    AbilityStack.Children.Add(button);
                }
                Button button1 = new Button();
                button1.Content = "MultipleAttack";
                button1.Click += (object sender, RoutedEventArgs e)
                    => GameTableController.Get().
                    StartMulitpleSelection();
                AbilityStack.Children.Add(button1);

            }
            else
            {
                AbilityStack.Children.Clear();
            }
        }
    }
}
