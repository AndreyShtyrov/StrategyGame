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
using System.ComponentModel;
using System.Net;
using Controller.Building;
using InterfaceOfObjects;

namespace StrategyGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool ispressed = false;
        PathField pathField;
        Task task;
        Field field;
        TextBlock CurrentPlayer;
        readonly FileInfo savefile = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\stg.json");
        public MainWindow()
        {
            InitializeComponent();

        }

        private void StartMultipleSelectWindow(Window window)
        {
            window.Show();
        }

        private void StartClient(int ClientIdx)
        {

            fieldgui.clearField();
            List<TurnSpeciffication> turns = new List<TurnSpeciffication>();
            turns.Add(new TurnSpeciffication(WeatherType.Rain, 0));
            turns.Add(new TurnSpeciffication(WeatherType.Normal, 1));
            turns.Add(new TurnSpeciffication(WeatherType.Rain, 2));
            turns.Add(new TurnSpeciffication(WeatherType.Normal, 3));
            turns.Add(new TurnSpeciffication(WeatherType.Normal, 4));
            turns.Add(new TurnSpeciffication(WeatherType.Normal, 5));
            TurnsBar turnsBar = new TurnsBar(turns);
            Grid.SetColumn(turnsBar, 1);
            Grid.SetRow(turnsBar, 0);
            grid.Children.Add(turnsBar);


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

            GameModeContainer.instance = new GameMode(field);
            var gameMode = GameModeContainer.Get();
            gameMode.UnitsListChanged += OnUnitsListChange;
            gameMode.BuildingListChanged += OnBuildingListChange;
            fieldgui.gameModeHandler = gameTable.ActionOnMouseButton;
            Turn.Click += (object sender, RoutedEventArgs e) => gameTable.SwitchTurn();
            fieldgui.drawGrid(field);
            RequestManager timer = new RequestManager();
            gameMode.AddRequestManager(timer);

            gameTable.PropertyChanged += UnitPanel.OnSelectedHandler;
            PlayerWindow playerWindow1 = new PlayerWindow(0);
            PlayerWindow playerWindow2 = new PlayerWindow(1);
            TopPannel.Children.Add(playerWindow1);
            TopPannel.Children.Add(playerWindow2);

        }

        private void StartServer(int ClientIdx)
        {

            List<TurnSpeciffication> turns = new List<TurnSpeciffication>();
            turns.Add(new TurnSpeciffication(WeatherType.Rain, 0));
            turns.Add(new TurnSpeciffication(WeatherType.Normal, 1));
            turns.Add(new TurnSpeciffication(WeatherType.Rain, 2));
            turns.Add(new TurnSpeciffication(WeatherType.Normal, 3));
            turns.Add(new TurnSpeciffication(WeatherType.Normal, 4));
            turns.Add(new TurnSpeciffication(WeatherType.Normal, 5));
            TurnsBar turnsBar = new TurnsBar(turns);
            Grid.SetColumn(turnsBar, 1);
            Grid.SetRow(turnsBar, 0);
            grid.Children.Add(turnsBar);



            fieldgui.clearField();
            Player.Create(0, 5, 5);
            Player.Create(1, 5, 5);
            var player1 = Player.Get(0);
            var player2 = Player.Get(1);
            CurrentPlayer = new TextBlock();
            TopPannel.Children.Add(CurrentPlayer);
            CurrentPlayer.Text = "Player: " + 0;

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
            gameTable.StartMulitpleSelectWindow = StartMultipleSelectWindow;
            
            GameModeContainer.instance = new GameModeServer(field);
            
            var gameMode = GameModeContainer.Get();
            
            gameMode.SwitchTurn();
            gameMode.UnitsListChanged += OnUnitsListChange;
            gameMode.BuildingListChanged += OnBuildingListChange;
            fieldgui.gameModeHandler = gameTable.ActionOnMouseButton;
            
            fieldgui.drawGrid(field);
            

            gameTable.PropertyChanged += UnitPanel.OnSelectedHandler;
            gameMode.PropertyChanged += OnCurrentPlayerChanged;
            PlayerWindow playerWindow1 = new PlayerWindow(0);
            PlayerWindow playerWindow2 = new PlayerWindow(1);
            TopPannel.Children.Add(playerWindow1);
            TopPannel.Children.Add(playerWindow2);
            Turn.Click += (object sender, RoutedEventArgs e) => gameTable.SwitchTurn();
            RequestManager timer = new RequestManager();
            gameMode.AddRequestManager(timer);
            gameMode.CreateUnit("Helbard", (4, 4), player1);
            gameMode.CreateUnit("Helbard", (6, 6), player2);
            gameMode.CreateUnit("Fork", (7, 7), player2);
            gameMode.CreateUnit("Veteran", (5, 6), player1);
            gameMode.CreateUnit("LongBow", (7, 6), player2);
            gameMode.CreateUnit("Buckler", (7, 8), player1);
            gameMode.CreateUnit("Veteran", (6, 8), player1);
            gameMode.CreateUnit("Fork", (5, 8), player2);
            gameMode.AddBuilding(BuildingPresset.Build("Camp", (0, 0), null));

            //gameMode.GetNewGameStates();
        }

        private void OnCurrentPlayerChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != "CurrentPlayer")
                return;
            if (sender is IGameMode tsender)
                CurrentPlayer.Text = "Player: " + tsender.CurrentPlayer.idx;
        }

        private void OnUnitsListChange(UnitPresset unitPresset, bool isExist)
        {
            if (isExist)
                fieldgui.addUnit(unitPresset);
            else
                fieldgui.removeUnit(unitPresset);
        }

        private void OnBuildingListChange(ITokenData building, bool isExist)
        {
            if (isExist)
                fieldgui.addBuilding(building);
            else
                fieldgui.removeBuilding(building);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HttpServer.listener = new HttpListener();
            HttpServer.listener.Prefixes.Add(HttpServer.url);
            HttpServer.listener.Start();
            task = HttpServer.HandlerIncomingConnections();
            StartServer(0);
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StartClient(1);
        }
    }
}
