using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using InterfaceOfObjects;
using UnitsAnPathFinding;

namespace Controller
{
    interface IGameMode: IController, INotifyPropertyChanged
    {
        public UnitPresset Selected
        { get; set; }


        public GameModeState State
        { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public UnitPresset[,] GetUnits();

        public void SwitchTrun();

        public UnitPresset CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None");

        public (int X, int Y, int Z) TransformToCube((int X, int Y) fpos, (int X, int Y) center);

        public (int X, int Y) TransformToAxial((int X, int Y, int Z) fpos, (int X, int Y) center);

        public bool SpendResources(int action, int move, Player owner);

        public void ReturnResources(int action, int move, Player owner);

        public List<UnitPresset> getUnitsInBattle();

        public void AttackUnit(UnitPresset target);

        public List<PathToken> getWalkArea();

        public void MoveUnit(PathToken pathToken);

        public void CreateBuilding(Building build);

        public bool SelectUnit(UnitPresset token);

        public void SelectedUnitRaiseStand(StandPresset stand);

        public void SelectedUnitActivateAbility(AbilityPresset ability);

    }
}
