using System;
using System.Collections.Generic;
using System.IO;
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
using DrawField;
using Tokens;

namespace Redactor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller controller;
        readonly FileInfo savefile = new FileInfo("C:\\Users\\WorkPlace\\Documents\\stg.json");
        public MainWindow()
        {
            InitializeComponent();
            Field field = new Field(10, 10);
            controller = new Controller(field, fieldgui);
            
            fieldgui.drawGrid(field);
        }


        private void Load(object sender, RoutedEventArgs e)
        {
            fieldgui.clearField();
            var field = Field.load(savefile);
            controller = new Controller(field, fieldgui);
            
            fieldgui.drawGrid(field);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            controller.save(savefile);
        }

        private void switch_to_grass(object sender, RoutedEventArgs e)
        {
            controller.selected = "Grass";
        }

        private void switch_to_field(object sender, RoutedEventArgs e)
        {
            controller.selected = "Hill";
        }


        private void switch_to_water(object sender, RoutedEventArgs e)
        {
            controller.selected = "Water";
        }


        private void switch_to_montains(object sender, RoutedEventArgs e)
        {
            controller.selected = "Montain";
        }


        private void switch_to_forest(object sender, RoutedEventArgs e)
        {
            controller.selected = "Forest";
        }

        private void switch_to_road(object sender, RoutedEventArgs e)
        {
            controller.selected = "Road";
        }
    }
}
