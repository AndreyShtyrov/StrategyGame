using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using InterfaceOfObjects;
using UnitsAnPathFinding;
using Controller.Actions;
using System.Threading.Tasks;
using Controller.Requests;

namespace Controller
{
    public interface IGameMode: IController, INotifyPropertyChanged
    {

        public int ActionIdx
        { get; }

        public GameModeState State
        { get; set; }

        public RequestSender RequestSender
        { get; }

        public PathToken GetPathToken(UnitPresset unit, (int X, int Y) fpos);

        public UnitPresset GetUnit((int X, int Y) fpos);

        public event PropertyChangedEventHandler PropertyChanged;

        public UnitPresset[,] GetGridOfUnits();

        public void SwitchTrun();

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None");

        public (int X, int Y, int Z) TransformToCube((int X, int Y) fpos, (int X, int Y) center);

        public (int X, int Y) TransformToAxial((int X, int Y, int Z) fpos, (int X, int Y) center);

        public bool IsEnoughResources(int action, int move, Player owner);

        public void ReturnResources(int action, int move, Player owner);

        public List<UnitPresset> getUnitsInBattle();

        public void AttackUnit(UnitPresset unit, UnitPresset target, int AbilityIdx);

        public List<PathToken> GetWalkArea(UnitPresset unit);

        public void Move(UnitPresset unit, PathToken pathToken);

        public void CreateBuilding(Building build);

        public bool IsUnitSelected(UnitPresset unit);

        public object ProcessRequset(object sender);

        public void ProcessActions(List<IActions> actions);

        public void BacklightTargets(UnitPresset unit, AbilityPresset ability);

        public void RefreshBacklight();

        public void AddUnit(UnitPresset unitPresset);

        public void DeleteUnit(UnitPresset unitPresset);

        public Task GetNewGameStates();

        public event OnUnitsListChange UnitsListChanged;

        public void UpDownStand(UnitPresset unit, int StandIdx);

        public List<UnitPresset> GetUnits();

        public void AddRequestManager(RequestManager Timer);

    }
}
