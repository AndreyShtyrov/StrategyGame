using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Tokens;
using InterfaceOfObjects;
using DrawField;
using UnitsAnPathFinding;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;
using Controller.Units;
using Controller.Actions;
using Controller.Requests;
using Controller.Building;
using Controller.Network;
using Controller.GameLogic;

namespace Controller
{
    public partial class GameModeServer : IGameMode
    {
        private List<IActions> Response = new List<IActions>();
        private List<RequestContainer> DelaiedRequests = new List<RequestContainer>();
        public IGameLogic GameModeLogic
        { get; }
        public INetworkMode NetworkMode
        { get; }

        private Field field;
        private List<UnitPresset> units = new List<UnitPresset>();
        private List<BuildingPresset> _Buildings = new List<BuildingPresset>();
        public List<BuildingPresset> Buildings => _Buildings;
        public PathField PathField;
        public ActionManager actionManager;
        private GameModeState _State;
        private List<UnitPresset> UnitsInBattle = new List<UnitPresset>();
        public Player CurrentPlayer => GameModeLogic.CurrentPlayer;
        public RequestManager RequestManager
        { get; set; }
        public Player ControllingPlayer;
        private RequestContainer preparedContainer;
        public List<TurnsSpeciffication> AllTurns;
        private List<QuerryAction> player1Querry;
        private int ActionIndex = 0;

        private int _CurrentTurnNumber=0;
        
        public int CurrentTurnNumber
        {
            get
            {
                return _CurrentTurnNumber;
            }
            set
            {
                _CurrentTurnNumber = value;
                OnPropertyChanged("CurrentTurnNumber");
            }
        }
        public GameModeState State
        {
            set
            {
                _State = value;
            }
            get
            {
                return _State;
            }
        }
        public RequestSender RequestSender
        { get; }
        public int ActionIdx => actionManager.NextActionIdx;

        public List<TurnsSpeciffication> Turns
        { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public GameModeServer(Field field, int reqestSender = 0)
        {
            this.field = field;
            PathField = new PathField(field);
            actionManager = new ActionManager();
            RequestSender = new RequestSender();
            RequestSender.SenderType = SenderType.Server;
            RequestSender.Player = reqestSender;
            GameModeLogic = new GameLogicServer(this, Player.Get(1));
            Player.Get(0).CurrentTurnNumber = 0;
            Player.Get(1).CurrentTurnNumber = 0;
            player1Querry = new List<QuerryAction>();
            ActionIndex = 0;
            player1Querry.Add(QuerryAction.Deploy);
            player1Querry.Add(QuerryAction.Await);
            player1Querry.Add(QuerryAction.Deploy);
            player1Querry.Add(QuerryAction.Await);
            NetworkMode = new NetworkModeServer(this);
        }

        public ITokenData getToken((int X, int Y) fpos)
        {
            return field.getData(fpos);
        }

        public PathToken GetPathToken(UnitPresset unit, (int X, int Y) fpos)
        {
            var pathTokens = GetWalkArea(unit);
            foreach (var token in pathTokens)
            {
                if (token.fieldPosition == fpos)
                    return token;
            }
            return null;
        }

        public UnitPresset GetUnit((int X, int Y) fpos)
        {
            foreach (var unit in units)
            {
                if (unit.fieldPosition == fpos)
                    return unit;
            }
            return null;
        }

        public event OnUnitsListChange UnitsListChanged;

        public event OnBuildingListChange BuildingListChanged;

        public List<UnitPresset> GetUnits()
        {
            return units;
        }

        public UnitPresset[,] GetGridOfUnits()
        {
            UnitPresset[,] result = new UnitPresset[field.height, field.width];
            foreach (var unit in units)
            {
                result[unit.fieldPosition.X, unit.fieldPosition.Y] = unit;
            }
            return result;
        }

        public void ChangePlayers(Player PrivousPlayer, Player NextPlayer)
        {
            GameModeLogic.CurrentPlayer = NextPlayer;
            ControllingPlayer = NextPlayer;
            OnPropertyChanged("CurrentPlayer");
        }

        public (int X, int Y, int Z) TransformToCube((int X, int Y) fpos, (int X, int Y) center)
        {
            if (center.Y % 2 == 0)
            {
                int row = fpos.X - center.X;
                int col = fpos.Y - center.Y;
                int x = row - (int)(col + (col & 1)) / 2;
                int z = col;
                int y = -x - z;
                return (x, y, z);
            }
            else
            {
                int row = fpos.X - center.X;
                int col = fpos.Y - center.Y;
                int x = row - (col - (col & 1)) / 2;
                int z = col;
                int y = -x - z;
                return (x, y, z);
            }
        }

        public (int X, int Y) TransformToAxial((int X, int Y, int Z) fpos, (int X, int Y) center)
        {
            (int X, int Y) result;
            if (center.Y % 2 == 0)
            {
                result.X = fpos.X + (fpos.Z + (fpos.Z & 1)) / 2;
                result.Y = fpos.Z;
            }
            else
            {
                result.X = fpos.X + (fpos.Z - (fpos.Z & 1)) / 2;
                result.Y = fpos.Z;
            }
            return (result.X + center.X, result.Y + center.Y);
        }

        public bool IsEnoughResources(int action, int move, Player owner)
        {

            if (owner.AttackPoints - action >= 0 && owner.MovePoints - move >= 0)
            {
                return true;
            }
            return false;
        }

        public void SpendResources(int action, int move, Player owner)
        {
            owner.AttackPoints -= action;
            owner.MovePoints -= move;
        }

        public void ReturnResources(int action, int move, Player owner)
        {
            owner.AttackPoints += action;
            owner.MovePoints += move;
        }

        public List<UnitPresset> getUnitsInBattle()
        {
            return UnitsInBattle;
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void PrepareToRequestUserInput(RequestContainer requestContainer, Player player)
        {
            preparedContainer = requestContainer;
            State = GameModeState.InteruptAndAwaitUserResponse;
            
            ChangePlayers(CurrentPlayer, player);
            if (CurrentPlayer.idx == RequestSender.Player)
            {
                NetworkMode.RequestUserInput(preparedContainer);
            }
        }

        public List<PathToken> GetWalkArea(UnitPresset unit)
        {
            PathField.Refresh();
            if (unit.MoveActionPoint.State == ActionState.Ended)
                return new List<PathToken>();
            return PathField.getWalkArea(unit.currentSpeed, unit, GetGridOfUnits());
        }

        private void OnChangeOwner(BuildingPresset sender, Player prevOwner)
        {
        }

        public void CreateBuilding(BuildingPresset build)
        {
            AddBuilding(build);
        }

        public void AcivateDeactivateGameTable(bool IsActive)
        {
            var gameTable = GameTableController.Get();
            if (gameTable == null)
                return;
            if (IsActive)
            {
                gameTable.State = GameTableState.AwaitSelect;
                State = GameModeState.Standart;
            }
            else
            {
                gameTable.State = GameTableState.AwaitEndEnemyTurn;
                State = GameModeState.AwaitResponse;
            }
        }

        public void ProcessActions(List<IActions> actions)
        {
            actionManager.ApplyActions(actions);
        }

        public bool IsUnitSelected(UnitPresset token)
        {
            if (token.owner == CurrentPlayer)
                return true;
            else
                return false;
        }

        public void BacklightTargets(UnitPresset unit, AbilityPresset ability)
        {
            var targets = PathField.getListOfTargets(unit, ability.DeafaultRange, GetGridOfUnits());
            foreach (var target in targets)
            {
                target.isTarget = true;
            }
        }

        public void RefreshBacklight()
        {
            foreach (var unit in units)
            {
                unit.isTarget = false;
            }
        }

        public void AddBuilding(BuildingPresset building)
        {
            _Buildings.Add(building);
            BuildingListChanged?.Invoke(building, true);
        }

        public void DeleteBuilding(BuildingPresset building)
        {
            _Buildings.Remove(building);
            BuildingListChanged?.Invoke(building, false);
        }

        public void AddUnit(UnitPresset unitPresset)
        {
            units.Add(unitPresset);
            UnitsListChanged?.Invoke(unitPresset, true);
        }

        public void DeleteUnit(UnitPresset unitPresset)
        {
            units.Remove(unitPresset);
            UnitsListChanged?.Invoke(unitPresset, false);
        }

        public Task GetNewGameStates()
        {
            throw new NotImplementedException();
        }

        public WeatherType GetWeather()
        {
            foreach (var turn in Turns)
            {
                if (turn.ActionIndex == CurrentTurnNumber)
                    return turn.Weather;
            }
            throw new NotImplementedException();
        }

        public BuildingPresset GetBuilding((int X, int Y) position)
        {
            foreach (var build in Buildings)
            {
                if (build.fieldPosition == position)
                {
                    return build;
                }
            }
            return null;
        }
    }

    public delegate void GameModeEventHandler(UnitPresset sender, UnitPresset target, GameModEventArgs e);

    public enum GameModeState
    {
        Standart = 0,
        AwaitResponse = 1,
        InteruptAndAwaitUserResponse = 2,
    }

    public delegate RequestContainer GetDelaiedResponse(RequestContainer requestContainer);

    public class GameModEventArgs
    {

    }
}
