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

namespace UnitsAnPathFinding
{
    /// <summary>
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class test : Window
    {
        public test()
        {

            InitializeComponent();
            var template = Brushes.Blue;
            SolidColorBrush brash = new SolidColorBrush(template.Color);
            brash.Opacity = 0.65;
            TestP1.Fill = brash;
            TestP.Fill = Brushes.Red;
        }

            private void TestP_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TestP.Fill = Brushes.Red;
            move.X = 300;
            move.Y = 400;
        }

        private void TestP1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            move.X = 40;
            move.Y = 100;
        }
    }
}
