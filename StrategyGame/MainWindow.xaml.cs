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
using UnitsAnPathFinding;
using Controller;
using Controller.Requests;

namespace StrategyGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool ispressed = false;
        PathField pathField;
        Field field;
        readonly FileInfo savefile = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\stg.json");
        public MainWindow()
        {
            InitializeComponent();

        }

        public override void EndInit()
        {
            base.EndInit();
            StartClient(0);
        }

        private void StartClient(int ClientIdx)
        {
            fieldgui.clearField();
            Player.Create(0, 5, 5);
            Player.Create(1, 5, 5);
            var player1 = Player.Get(0);
            var player2 = Player.Get(1);

            if (ClientIdx == 0)
            {
                GameTableController.Create(player1, fieldgui);
            }
            else
            {
                GameTableController.Create(player2, fieldgui);
            }
            var gameTable = GameTableController.Get();
            field = Field.load(savefile);

            GameModeContainer.instance = new GameModeServer(field);
            
            var gameMode = GameModeContainer.Get();
            gameMode.UnitsListChanged += OnUnitsListChange;
            fieldgui.gameModeHandler = gameTable.ActionOnMouseButton;
            fieldgui.drawGrid(field);
            

            gameTable.PropertyChanged += UnitPanel.OnSelectedHandler;
            PlayerWindow playerWindow1 = new PlayerWindow(0);
            PlayerWindow playerWindow2 = new PlayerWindow(1);
            TopPannel.Children.Add(playerWindow1);
            Turn.Click += (object sender, RoutedEventArgs e) => GameModeContainer.Get().ChangePlayers();
            RequestManager timer = new RequestManager();
            gameMode.AddRequestManager(timer);
            gameMode.CreateUnit("Helbard", (4, 4), player1);
            gameMode.CreateUnit("Helbard", (6, 6), player2);
            gameMode.CreateUnit("Helbard", (7, 7), player2);
            gameMode.CreateUnit("LongBow", (5, 6), player1);
            gameMode.CreateUnit("LongBow", (7, 6), player2);
            //gameMode.GetNewGameStates();
        }

        private void OnUnitsListChange(UnitPresset unitPresset, bool isExist)
        {
            fieldgui.addUnit(unitPresset);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //StartClient(0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //StartClient(1);
        }
    }
}
