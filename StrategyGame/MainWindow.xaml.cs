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

        private void StartClient(int ClientIdx)
        {
            fieldgui.clearField();
            var player1 = Player.getPlayer(0, 5, 5);
            var player2 = Player.getPlayer(1, 5, 5);
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
            GameModeContainer.instance = new GameMode(field);
            var gameMode = GameModeContainer.Get();
            gameMode.UnitsListChanged += OnUnitsListChange;
            fieldgui.gameModeHandler = gameTable.ActionOnMouseButton;
            fieldgui.drawGrid(field);
            player1.AttackPoints = 5;
            player1.MovePoints = 5;
            player2.MovePoints = 5;
            player2.AttackPoints = 5;

            gameTable.PropertyChanged += UnitPanel.OnSelectedHandler;
            PlayerWindow playerWindow1 = new PlayerWindow(0);
            PlayerWindow playerWindow2 = new PlayerWindow(1);
            TopPannel.Children.Add(playerWindow1);
            TopPannel.Children.Add(playerWindow2);
            Turn.Click += (object sender, RoutedEventArgs e) => gameMode.SwitchTrun();
            //gameMode.GetNewGameStates();
        }

        private void OnUnitsListChange(UnitPresset unitPresset, bool isExist)
        {
            fieldgui.addUnit(unitPresset);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartClient(0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StartClient(1);
        }
    }
}
