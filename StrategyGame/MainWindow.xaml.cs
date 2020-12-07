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
            fieldgui.clearField();
            field = Field.load(savefile);
            GameModeContainer.instance = new GameMode(field);
            var gameMode = GameModeContainer.Get();
            var player1 = Player.getPlayer(0, 5, 5);
            var player2 = Player.getPlayer(1, 5, 5);
            GameTableController.InitGameTableControler(player1, fieldgui);
            var gameTable = GameTableController.Get();
            fieldgui.gameModeHandler = gameTable.ActionOnMouseButton;
            fieldgui.drawGrid(field);
            player1.AttackPoints = 5;
            player1.MovePoints = 5;
            player2.MovePoints = 5;
            player2.AttackPoints = 5;
            gameTable.CreateUnit("Helbard", (4, 4), player1);
            gameTable.CreateUnit("Helbard", (6, 6), player2);
            gameTable.CreateUnit("Helbard", (7, 7), player2);
            gameTable.CreateUnit("LongBow", (5, 6), player1);
            gameTable.CreateUnit("LongBow", (7, 6), player2);
            gameMode.PropertyChanged += UnitPanel.OnSelectedHandler;
            PlayerWindow playerWindow1 = new PlayerWindow(0);
            PlayerWindow playerWindow2 = new PlayerWindow(1);
            TopPannel.Children.Add(playerWindow1);
            TopPannel.Children.Add(playerWindow2);
            Turn.Click += (object sender, RoutedEventArgs e) => gameMode.SwitchTrun();
        }
    }
}
