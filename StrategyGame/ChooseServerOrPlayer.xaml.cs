using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Controller;
using Tokens;

namespace StrategyGame
{
    /// <summary>
    /// Interaction logic for ChooseServerOrPlayer.xaml
    /// </summary>
    public partial class ChooseServerOrPlayer : Window
    {

        readonly FileInfo savefile = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\stg.json");
        public ChooseServerOrPlayer()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var field = Field.load(savefile);
            var contaner = GameModeContainer.Get( true,field);
            HttpServer.listener = new HttpListener();
            HttpServer.listener.Prefixes.Add(HttpServer.url);
            HttpServer.listener.Start();
            Task listTask = HttpServer.HadlerIncominfConnections();
            listTask.GetAwaiter().GetResult();
            HttpServer.listener.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
