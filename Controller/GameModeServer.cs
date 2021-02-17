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


namespace Controller
{
    public partial class GameModeServer : IGameMode
    {
        private List<IActions> Response = new List<IActions>();
        private List<RequestContainer> DelaiedRequests = new List<RequestContainer>();
        private GameModeLogic GameModeLogic;
        private List<(int index, List<UnitPresset> Units, List<Building> buildings)> PrevState;
        private IListOfToken field;
        private List<UnitPresset> units = new List<UnitPresset>();
        private List<Building> buildings = new List<Building>();
        private PathField pathField;
        private ActionManager actionManager;
        private GameModeState _State;
        private List<UnitPresset> UnitsInBattle = new List<UnitPresset>();
        private Player CurrentPlayer => GameModeLogic.CurrentPlayer;
        private int _ActionIdx = 0;
        private RequestManager requestManager;

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

        public event PropertyChangedEventHandler PropertyChanged;

        public GameModeServer(Field field, int reqestSender = -1)
        {
            this.field = field;
            pathField = new PathField(field);
            actionManager = new ActionManager();
            this.RequestSender = new RequestSender();
            this.RequestSender.SenderType = SenderType.Server;
            this.RequestSender.Player = reqestSender;
            GameModeLogic = new GameModeLogic(this, Player.Get(0));
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

        //public async Task<UnitPresset> AwaitSelectTarget(UnitPresset unit, AbilityPresset ability, List<UnitPresset> targets)
        //{
        //    State = GameModeState.AwaitResponse;
        //}

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

        public void AttackUnit(UnitPresset unit, UnitPresset target, int AbilityIdx)
        {
            Response = GameModeLogic.ProcessMeleeBattle(unit, target, AbilityIdx);
            if (GameTableController.Get() != null)
                GameTableController.Get().State = GameTableState.AwaitSelect;
        }

        public void UpDownStand(UnitPresset unit, int StandIdx)
        {
            GameModeLogic.UpDownStand(unit, StandIdx);
        }

        public List<PathToken> GetWalkArea(UnitPresset unit)
        {
            pathField.Refresh();
            if (unit.MoveActionPoint.State == ActionState.Ended)
                return new List<PathToken>();
            return pathField.getWalkArea(unit.currentSpeed, unit, GetGridOfUnits());
        }

        public void Move(UnitPresset unit, PathToken pathToken)
        {
            Response = GameModeLogic.Move(unit, pathToken);
            pathField.Refresh();
            if (GameTableController.Get() != null)
                GameTableController.Get().State = GameTableState.AwaitSelect;
        }

        private void OnChangeOwner(Building sender, Player prevOwner)
        {
            if (sender.Type == BuildingTypes.AttackFlag)
            {
                sender.owner.IncomeAttackPoints += 1;
                prevOwner.IncomeAttackPoints -= 1;
            }
            if (sender.Type == BuildingTypes.MoveFlag)
            {
                sender.owner.IncomeAttackPoints += 1;
                prevOwner.IncomeAttackPoints -= 1;
            }
        }

        public void CreateBuilding(Building build)
        {
            buildings.Add(build);
            build.ChangeOwner += OnChangeOwner;
        }

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            Response = GameModeLogic.CreateUnit(name, fpos, owner, typeUnit);
        }

        public object ProcessRequset(object sender)
        {
            Response.Clear();
            if (sender is RequestContainer getNewStates)
            {
                if (getNewStates.Type == RequestType.GetNewStates)
                {
                    var response = actionManager.GetMissedActions(getNewStates.CurrentActionIndex);
                    RequestContainer applyChangesRequest = new RequestContainer(RequestType.ApplyChanges);
                    applyChangesRequest.Actions = response;
                    return applyChangesRequest;
                }
            }
            if (State == GameModeState.Standart)
            {
                if (sender is RequestContainer request)
                {
                    if (request.Actions != null)
                        ProcessActions(request.Actions);
                    if (request.Type == RequestType.MoveUnit)
                    {
                        var unit = GetUnit(request.Selected);
                        var pathToken = GetPathToken(unit, request.Target);
                        Move(unit, pathToken);
                    }
                    if (request.Type == RequestType.UseAbility)
                    {
                        var unit = GetUnit(request.Selected);
                        var ability = unit.GetAbility(request.AbilityIdx);
                        if (ability.AbilityType == AbilityType.Attack ||
                            ability.AbilityType == AbilityType.RangeAttack)
                        {
                            var target = GetUnit(request.Target);
                            AttackUnit(unit, target, ability.idx);
                        }
                        else
                        {
                            UseAction useAction = new UseAction();
                            useAction.Source = unit.fieldPosition;
                            useAction.SourceAbility = ability.idx;
                            Response.Add(useAction);
                        }
                    }
                    if (request.Type == RequestType.CreateUnit)
                    {
                        CreateUnit(request.Name, request.Selected, Player.Get(request.Player));
                    }
                    RequestContainer applyChangesRequest = new RequestContainer(RequestType.ApplyChanges);
                    applyChangesRequest.Actions = Response;
                    return applyChangesRequest;
                }
            }
            return null;
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
            var targets = pathField.getListOfTargets(unit, ability.DeafaultRange, GetGridOfUnits());
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

        public void AddRequestManager(RequestManager Timer)
        {
            this.requestManager = Timer;
        }

    }

    public delegate void GameModeEventHandler(UnitPresset sender, UnitPresset target, GameModEventArgs e);

    public enum GameModeState
    {
        Standart = 0,
        AwaitResponse = 1,
    }

    public delegate RequestContainer GetDelaiedResponse(RequestContainer requestContainer);

    public class GameModEventArgs
    {

    }
}
