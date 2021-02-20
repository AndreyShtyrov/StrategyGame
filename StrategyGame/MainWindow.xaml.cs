﻿using System;
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

            GameModeContainer.instance = new GameMode(field);
            var gameMode = GameModeContainer.Get();
            gameMode.UnitsListChanged += OnUnitsListChange;
            fieldgui.gameModeHandler = gameTable.ActionOnMouseButton;

            fieldgui.drawGrid(field);


            gameTable.PropertyChanged += UnitPanel.OnSelectedHandler;
            PlayerWindow playerWindow1 = new PlayerWindow(0);
            PlayerWindow playerWindow2 = new PlayerWindow(1);
            TopPannel.Children.Add(playerWindow1);
            TopPannel.Children.Add(playerWindow2);

            gameMode.GetNewGameStates();
            

        }

        private void StartServer(int ClientIdx)
        {
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

            GameModeContainer.instance = new GameModeServer(field);
            
            var gameMode = GameModeContainer.Get();
            gameMode.SwitchTurn();
            gameMode.UnitsListChanged += OnUnitsListChange;
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
            gameMode.CreateUnit("Helbard", (7, 7), player2);
            gameMode.CreateUnit("LongBow", (5, 6), player1);
            gameMode.CreateUnit("LongBow", (7, 6), player2);

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
            fieldgui.addUnit(unitPresset);
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
