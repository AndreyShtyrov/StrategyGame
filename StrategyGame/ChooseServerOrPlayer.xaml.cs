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

        Task task;

        readonly FileInfo savefile = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\stg.json");
        public ChooseServerOrPlayer()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var field = Field.load(savefile);
            GameModeContainer.instance = new GameModeServer(field);
            var contaner = GameModeContainer.Get();
            HttpServer.listener = new HttpListener();
            HttpServer.listener.Prefixes.Add(HttpServer.url);
            HttpServer.listener.Start();
            task = HttpServer.HandlerIncomingConnections();
            var player1 = Player.getPlayer(0, 5, 5);
            var player2 = Player.getPlayer(1, 5, 5);
            GameModeContainer.Get().CreateUnit("Helbard", (4, 4), player1);
            GameModeContainer.Get().CreateUnit("Helbard", (6, 6), player2);
            GameModeContainer.Get().CreateUnit("Helbard", (7, 7), player2);
            GameModeContainer.Get().CreateUnit("LongBow", (5, 6), player1);
            GameModeContainer.Get().CreateUnit("LongBow", (7, 6), player2);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (HttpServer.listener != null)
                HttpServer.listener.Close();
            base.OnClosed(e);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
