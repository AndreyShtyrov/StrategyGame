using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tokens;

namespace DrawField
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Field field = new Field(10, 10);
            fieldgui.drawGrid(field);
        }

        private void btn_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender is SimpleTokenGui tsender)
            {
                tsender.CPoligon.Fill = Brushes.Blue;
            }
        }
    }
}
