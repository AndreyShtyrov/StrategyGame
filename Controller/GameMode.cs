using System;
using System.Collections.Generic;
using System.Text;
using Tokens;
using InterfaceOfObjects;
using System.ComponentModel;
using UnitsAnPathFinding;
using Controller.Abilities;
using Controller.Actions;
using Controller.Units;
using Controller.Requests;
using System.Threading.Tasks;

namespace Controller
{
    public class GameMode: IGameMode
    {
        private List<IActions> Response = new List<IActions>();
        private List<(int index, List<UnitPresset> Units, List<Building> buildings)> PrevState;
        private IListOfToken field;
        private List<UnitPresset> units = new List<UnitPresset>();
        private List<Building> buildings = new List<Building>();
        private PathField pathField;
        private GameModeState _State;
        private List<UnitPresset> UnitsInBattle = new List<UnitPresset>();
        private Player CurrentPlayer = Player.getPlayer(0);
        private ActionManager actionManager;
        private RequestManager requestManager;

        public RequestSender RequestSender
        { get; }
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
        public int ActionIdx => actionManager.NextActionIdx;
        public event PropertyChangedEventHandler PropertyChanged;

        public GameMode(Field field)
        {
            this.field = field;
            pathField = new PathField(field);
            actionManager = new ActionManager();
            this.RequestSender = new RequestSender();
            this.RequestSender.SenderType = SenderType.Client;
            this.RequestSender.Player = GameTableController.Get().owner.idx;
            if (this.RequestSender.Player != CurrentPlayer.idx)
                State = GameModeState.AwaitResponse;
        }

        public UnitPresset[,] GetUnits()
        {
            UnitPresset[,] result = new UnitPresset[field.height, field.width];
            foreach (var unit in units)
            {
                result[unit.fieldPosition.X, unit.fieldPosition.Y] = unit;
            }
            return result;
        }

        public void SwitchTrun()
        {

            CurrentPlayer.getIncome();
            if (CurrentPlayer.idx == 0)
                CurrentPlayer = Player.getPlayer(1);
            else
                CurrentPlayer = Player.getPlayer(0);
            foreach (var unit in units)
            {
                if (unit.owner == CurrentPlayer)
                {
                    unit.Refresh();
                }
            }
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

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event OnUnitsListChange UnitsListChanged;

        public bool SpendResources(int action, int move, Player owner)
        {

            if (owner.AttackPoints - action >= 0 && owner.MovePoints - move >= 0)
            {
                owner.AttackPoints -= action;
                owner.MovePoints -= move;
                return true;
            }
            return false;
        }

        public void ReturnResources(int action, int move, Player owner)
        {
            owner.AttackPoints += action;
            owner.MovePoints += move;
        }

        public List<PathToken> getWalkArea(UnitPresset unit)
        {
            pathField.Refresh();
            return pathField.getWalkArea(unit.currentSpeed, unit, GetUnits());
        }

        public PathToken GetPathToken(UnitPresset unit, (int X, int Y) fpos)
        {
            var pathTokens = getWalkArea(unit);
            foreach (var token in pathTokens)
            {
                if (token.fieldPosition == fpos)
                    return token;
            }
            return null;
        }

        public List<UnitPresset> getUnitsInBattle()
        {
            return UnitsInBattle;
        }

        public void AttackUnit(UnitPresset unit, UnitPresset target, int AbilityIdx)
        {
            if (State == GameModeState.Standart)
            {
                State = GameModeState.AwaitResponse;
                RequestContainer requestContainer = new RequestContainer(RequestType.UseAbility);
                requestContainer.Selected = unit.fieldPosition;
                requestContainer.Target = target.fieldPosition;
                PrepareToUse action = new PrepareToUse();
                action.Source = unit.fieldPosition;
                action.SourceAbility = GameTableController.Get().selectedAbility.idx;
                requestContainer.Actions = new List<IActions>();
                requestContainer.Actions.Add(action);
                Client.sendRequest(requestContainer);
            }
        }

        public void Move(UnitPresset unit, PathToken pathToken)
        {
            if (State == GameModeState.Standart)
            {
                State = GameModeState.AwaitResponse;
                RequestContainer requestContainer = new RequestContainer(RequestType.MoveUnit);
                requestContainer.Selected = unit.fieldPosition;
                requestContainer.Target = pathToken.fieldPosition;
                Client.sendRequest(requestContainer);
            }
        }

        public void CreateBuilding(Building build)
        {
            throw new NotImplementedException();
        }

        public bool SelectUnit(UnitPresset token)
        {
            if (token.owner == CurrentPlayer)
                return true;
            else
                return false;
        }

        public void SelectedUnitRaiseStand(StandPresset stand)
        {
            stand.UpStand();
        }

        public void SelectedUnitActivateAbility(AbilityPresset ability)
        {
            ability.PrepareToUse();
        }

        public ITokenData getToken((int X, int Y) fpos)
        {
            throw new NotImplementedException();
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

        public object ProcessRequset(object sender)
        {
            if (sender is RequestContainer tsender)
            {
                if (tsender.Type == RequestType.ApplyChanges)
                    ProcessActions(tsender.Actions);
                GameTableController.Get().State = GameTableState.AwaitSelect;
            }
            State = GameModeState.Standart;
            return null;
        }

        public void ProcessActions(List<IActions> actions)
        {
            actionManager.ApplyActions(actions);
        }

        public async Task<object> GetNewGameStates()
        {
            RequestContainer requestContainer = new RequestContainer(RequestType.GetNewStates);
            requestContainer.CurrentActionIndex = actionManager.CurrentActionIdx;
            return await Client.sendRequestAsync(requestContainer);
        }

        public void BacklightTargets(UnitPresset unit, AbilityPresset ability)
        {
            var targets = pathField.getListOfTargets(unit, ability.DeafaultRange, GetUnits());
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

        public void AddRequestManager(RequestManager Timer)
        {
            this.requestManager = Timer;
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

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            RequestContainer requestContainer = new RequestContainer(RequestType.CreateUnit);
            requestContainer.Name = name;
            requestContainer.Player = owner.idx;
            requestContainer.Selected = fpos;
            Client.sendRequest(requestContainer);
        }
    }

    public delegate void OnUnitsListChange(UnitPresset unit, bool isExist);
}
