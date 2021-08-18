using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using InterfaceOfObjects;
using UnitsAnPathFinding;
using Controller.Actions;
using System.Threading.Tasks;
using Controller.Requests;
using Controller.Building;
using Controller.Network;
using Controller.GameLogic;

namespace Controller
{
    public interface IGameMode : INotifyPropertyChanged
    {

        public int ActionIdx
        { get; }

        public GameModeState State
        { get; set; }

        public IGameLogic GameModeLogic
        { get; }

        public INetworkMode NetworkMode
        { get; }

        public RequestSender RequestSender
        { get; }

        public int CurrentTurnNumber
        { get; set; }

        public RequestManager RequestManager
        { get; set; }

        public Player CurrentPlayer
        { get; }

        public WeatherType GetWeather();

        public List<TurnsSpeciffication> Turns
        { get; }

        public PathToken GetPathToken(UnitPresset unit, (int X, int Y) fpos);

        public UnitPresset GetUnit((int X, int Y) fpos);

        public event PropertyChangedEventHandler PropertyChanged;

        public UnitPresset[,] GetGridOfUnits();

        public void ChangePlayers(Player previousPlayer, Player nextPlayer);

        public (int X, int Y, int Z) TransformToCube((int X, int Y) fpos, (int X, int Y) center);

        public (int X, int Y) TransformToAxial((int X, int Y, int Z) fpos, (int X, int Y) center);

        public bool IsEnoughResources(int action, int move, Player owner);

        public void ReturnResources(int action, int move, Player owner);

        public List<UnitPresset> getUnitsInBattle();

        public List<PathToken> GetWalkArea(UnitPresset unit);

        public void CreateBuilding(BuildingPresset build);

        public bool IsUnitSelected(UnitPresset unit);

        public void ProcessActions(List<IActions> actions);

        public void BacklightTargets(UnitPresset unit, AbilityPresset ability);

        public void RefreshBacklight();

        public void AddUnit(UnitPresset unitPresset);

        public void DeleteUnit(UnitPresset unitPresset);

        public void AddBuilding(BuildingPresset building);

        public void DeleteBuilding(BuildingPresset building);

        public Task GetNewGameStates();

        public event OnBuildingListChange BuildingListChanged;

        public event OnUnitsListChange UnitsListChanged;

        public List<UnitPresset> GetUnits();

        public BuildingPresset GetBuilding((int X, int Y) position);

        public void AcivateDeactivateGameTable(bool IsActive);
    }
}
