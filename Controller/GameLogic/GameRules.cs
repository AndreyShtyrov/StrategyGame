using System;
using System.Collections.Generic;
using System.Text;
using Controller.Actions;
using Controller.Abilities;
using UnitsAnPathFinding;
using Controller.Requests;

namespace Controller
{
    public class GameRules
    {
        private bool isDelay = false;

        private GameModeServer GameMode;

        private AbilityType InteraptionAction;

        private Player PlayerBeforeIteration;

        private AbilityContanier AwaitSelection;

        private List<AbilityContanier> DelayedActions;

        public Player CurrentPlayer
        { get; set; }

        public GameRules(GameModeServer GameMode, Player FirstPlyaer)
        {
            this.GameMode = GameMode;
            CurrentPlayer = FirstPlyaer;
        }

        public void IteruptAndMakeUserRequest(UnitPresset sender, UnitPresset unit, UnitPresset target, int abilityIdx, List<IActions> actions)
        {
            RequestContainer requestContainer = new RequestContainer(RequestType.NeedResponse);
            requestContainer.Selected = unit.fieldPosition;
            requestContainer.Targets = new List<(int X, int Y)>();
            if (abilityIdx == 0)
            {
                var area = GameMode.GetWalkArea(unit);
                foreach (var ctarget in area)
                {
                    requestContainer.Targets.Add(ctarget.fieldPosition);
                }
                if (GameMode.State != GameModeState.InteruptAndAwaitUserResponse)
                    requestContainer.TargetsTypeName = "PathToken";
                PlayerBeforeIteration = GameMode.CurrentPlayer;
                GameMode.PrepareToRequestUserInput(requestContainer, unit.owner);
                return;
            }
            var ability = unit.GetAbility(abilityIdx);
            if (ability != null)
            {
                var stand = unit.GetStand(abilityIdx);
                foreach (var ctarget in stand.GetAllTargets(sender, target))
                {
                    requestContainer.Targets.Add(ctarget.fieldPosition);
                }
                requestContainer.TargetsTypeName = "UnitPresset";
            }
            if (GameMode.State != GameModeState.InteruptAndAwaitUserResponse)
                PlayerBeforeIteration = GameMode.CurrentPlayer;
            GameMode.PrepareToRequestUserInput(requestContainer, unit.owner);
        }

        private List<IActions> SwitchTurn(bool IsIterrupting)
        {
            List<IActions> result = new List<IActions>();
            result.Add(new MoveUnit());
            return result;
        }

        private List<UnitPresset> UnitsInBattle = new List<UnitPresset>();

        public List<IActions> Move(UnitPresset unit, PathToken pathToken)
        {
            List<IActions> result = new List<IActions>();
            if (unit.MoveActionPoint.IsReady(unit.owner) ||
                unit.MoveActionPoint.State == ActionState.InProcess)
            {
                result.AddRange(unit.Move(pathToken.fieldPosition));
            }
            var building = GameMode.GetBuilding(unit.fieldPosition);
            if (building != null)
                result.AddRange(building.Capture(unit));
            GameMode.ProcessActions(result);
            return result;

        }

        public IActions TraitePlayersResources(Player player, int attackPoints = 0, int movePoints = 0, bool isReverse = false)
        {
            SpendPlayerResources spendPlayerResources;
            if (isReverse)
                spendPlayerResources = new SpendPlayerResources(
                -attackPoints, -movePoints, player.idx);
            else
                spendPlayerResources = new SpendPlayerResources(
                attackPoints, movePoints, player.idx);
            return spendPlayerResources;

        }

        public List<IActions> CheckUnitsAfterBattle()
        { 
            DelayedActions = new List<AbilityContanier>();
            AwaitSelection = null;
            List<IActions> result = new List<IActions>();
            foreach (var unit in UnitsInBattle)
            {
                if (unit.currentHp < 0)
                {
                    result.Add(new KillUnit(unit));
                }
                else if (unit.currentHp == 0)
                {
                    if (AwaitSelection == null)
                    {
                        isDelay = true;
                        AwaitSelection = new AbilityContanier(unit, unit, AbilityType.Move);
                    }
                    else
                    {
                        DelayedActions.Add(new AbilityContanier(unit, unit, AbilityType.Move));
                    }
                }
            }
            UnitsInBattle = new List<UnitPresset>();
            return result;
        }

        public List<IActions> ProcessMeleeBattle(UnitPresset unit, UnitPresset target, int AbilityIdx)
        {
            isDelay = false;
            DelayedActions = new List<AbilityContanier>();
            List<IActions> result = new List<IActions>();
            UnitsInBattle.Clear();
            UnitsInBattle.Add(unit);
            UnitsInBattle.Add(target);

            var Attack = unit.GetAbility(AbilityIdx);


            if (Attack.AbilityType == AbilityType.RangeAttack)
            {
                result.AddRange(Attack.Use(target));
                GameMode.ProcessActions(result);
                var kills = CheckUnitsAfterBattle();
                GameMode.ProcessActions(kills);
                result.AddRange(kills);
                if (isDelay)
                    IteruptAndMakeUserRequest(AwaitSelection.unit,
                        AwaitSelection.sender,
                        AwaitSelection.unit,
                        AwaitSelection.AbilityIdx,
                        result);
                return result;
            }

            var standActions = CheckInAreaAbilities(unit, target, BattleStage.Preemptive);
            GameMode.ProcessActions(standActions);
            result.AddRange(standActions);

            if (unit.currentHp > 0)
                result.AddRange(Attack.Use(target));
            GameMode.ProcessActions(result);

            standActions = CheckInAreaAbilities(unit, target, BattleStage.MainAttack);
            GameMode.ProcessActions(standActions);
            result.AddRange(standActions);

            result.AddRange(target.Response(unit));
            GameMode.ProcessActions(result);

            standActions = CheckInAreaAbilities(unit, target, BattleStage.ResponseAttack);
            GameMode.ProcessActions(standActions);
            result.AddRange(standActions);

            if (GameMode.State != GameModeState.InteruptAndAwaitUserResponse)
            {
                var kills = CheckUnitsAfterBattle();
                GameMode.ProcessActions(kills);
                result.AddRange(kills);
                if (isDelay)
                    IteruptAndMakeUserRequest(AwaitSelection.unit,
                        AwaitSelection.sender,
                        AwaitSelection.unit,
                        AwaitSelection.AbilityIdx,
                        result);
            }
            
            return result;

        }

        private List<IActions> CheckInAreaAbilities(UnitPresset unit, UnitPresset target, BattleStage stage)
        {
            bool isHereDelay = false;
            List<IActions> result = new List<IActions>();
            List<(UnitPresset unit, StandPresset stand)> listStands
                = new List<(UnitPresset unit, StandPresset stand)>();
            var units = GameModeContainer.Get().GetUnits();
            foreach (var lunit in units)
            {
                foreach (var stand in lunit.Stends)
                {
                    if (stand.CouldToReact(unit, target, stage))
                    {
                        listStands.Add((lunit, stand));
                        UnitsInBattle.Add(lunit);
                    }
                }
            }
            foreach (var stand in listStands)
            {
                if (stand.stand.AbilityType == AbilityType.SelectAndAttack && !isDelay)
                {
                    isDelay = true;
                    isHereDelay = true;
                    AwaitSelection = new AbilityContanier(stand.unit, unit, stand.stand.AbilityType, target, stand.stand);
                    InteraptionAction = AbilityType.SelectAndAttack;
                    continue;
                }
                if (!isDelay)
                {
                    result.AddRange(stand.stand.Use(unit, target));
                }
                else
                {
                    DelayedActions.Add(new AbilityContanier(stand.unit, unit, stand.stand.AbilityType, target, stand.stand));
                    continue;
                }
            }
            
            if (isHereDelay)
                IteruptAndMakeUserRequest(AwaitSelection.sender, AwaitSelection.unit, target, AwaitSelection.AbilityIdx, result);
            return result;
        }

        public List<IActions> UpDownStand(UnitPresset unit, int StandIdx)
        {
            List<IActions> result = new List<IActions>();
            var stand = unit.GetStand(StandIdx);
            IActions playerResources;
            if (stand.point.State == ActionState.Ended ||
                !GameMode.IsEnoughResources(
                    stand.point.neededAttackPoints,
                    stand.point.neededMovePoints, unit.owner))
                return result;

            if (!stand.Active)
            {
                result.Add(new RaiseStend(unit.fieldPosition, StandIdx, true));
                playerResources = TraitePlayersResources(
                    unit.owner,
                    stand.point.neededAttackPoints,
                    stand.point.neededMovePoints);
                result.Add(playerResources);
            }
            else
            {
                result.Add(new RaiseStend(unit.fieldPosition, StandIdx, false));
                playerResources = TraitePlayersResources(
                    unit.owner,
                    stand.point.neededAttackPoints,
                    stand.point.neededMovePoints,
                    true);
                result.Add(playerResources);
            }

            GameMode.ProcessActions(result);
            return result;
        }

        public List<IActions> CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None")
        {
            var result = new List<IActions>();
            CreateUnit createUnit = new CreateUnit(name, fpos, owner.idx);
            result.Add(createUnit);
            GameMode.ProcessActions(result);
            return result;
        }

        public List<IActions> SwitchTurn(Player currentPlayer, Player NextPlayer)
        {
            var result = new List<IActions>() { new SwitchTurn(currentPlayer, NextPlayer) };
            
            foreach (var building in GameMode.Buildings)
            {
                result.AddRange(building.Use());
            }
            GameMode.ProcessActions(result);
            return result;
        }

        public List<IActions> ApplyAbilityWithoutSelection(UnitPresset unit, AbilityPresset ability)
        {
            var result = new List<IActions>();
            result.AddRange(ability.Use(unit));
            var building = GameMode.GetBuilding(unit.fieldPosition);
            if (building != null)
                result.AddRange(building.Capture(unit));
            GameMode.ProcessActions(result);
            GameMode.ProcessActions(result);
            return result;
        }

        public List<IActions> ProcessIteraptedAndNextActions(UnitPresset unit, (int X, int Y) fpos)
        {
            this.isDelay = false;
            List<IActions> result = new List<IActions>();
            if (AwaitSelection.Type == AbilityType.SelectAndAttack)
            {
                result.AddRange(AwaitSelection.Use(fpos));
            }
            else if (AwaitSelection.Type == AbilityType.Move)
            {
                result.AddRange(AwaitSelection.Use(fpos));
            }
            var newDealiedActions = new List<AbilityContanier>();
            
            foreach (var ability in DelayedActions)
            {
                if (isDelay)
                {
                    newDealiedActions.Add(ability);
                    continue;
                }
                if ( ability.Type != AbilityType.SelectAndAttack)
                    result.AddRange(ability.Use());
                else
                {
                    isDelay = true;
                    AwaitSelection = ability;
                    InteraptionAction = AbilityType.SelectAndAttack;
                }
            }
            if (isDelay)
            {
                DelayedActions = newDealiedActions;
                GameMode.ProcessActions(result);
                IteruptAndMakeUserRequest(AwaitSelection.sender, 
                    AwaitSelection.unit, 
                    AwaitSelection.target,
                    AwaitSelection.AbilityIdx, 
                    result);
                return result;
            }
            GameMode.ChangePlayers(GameMode.CurrentPlayer, PlayerBeforeIteration);
            GameMode.ProcessActions(result);
            GameMode.State = GameModeState.Standart;
            AwaitSelection = null;
            DelayedActions = null;
            if (!isDelay)
            {
                result.AddRange(CheckUnitsAfterBattle());
                if (isDelay)
                {
                    IteruptAndMakeUserRequest(AwaitSelection.sender,
                    AwaitSelection.unit,
                    AwaitSelection.unit,
                    AwaitSelection.AbilityIdx,
                    result);
                }
            }
                
            return result;
        }

        private delegate List<IActions> DelayStand(UnitPresset unit, UnitPresset target);

        private class AbilityContanier
        {
            public readonly UnitPresset unit;
            public readonly UnitPresset sender;
            public readonly UnitPresset target;
            public readonly AbilityType Type;
            StandPresset stand;

            public int AbilityIdx
            {
                get
                {
                    if (Type != AbilityType.Move)
                        return stand.idx;
                    else
                        return 0;
                }
            }

            public AbilityContanier(UnitPresset unit, UnitPresset sender, AbilityType type, UnitPresset target =null, StandPresset stand = null)
            {
                this.unit = unit;
                Type = type;
                this.sender = sender;
                if (type == AbilityType.SelectAndAttack)
                {
                    this.target = target;
                    this.stand = stand;
                }
            }

            public List<IActions> Use()
            {
                if (Type == AbilityType.SelectAndAttack ||
                    Type == AbilityType.Move )
                {
                    throw new NotImplementedException();
                }
                return stand.Use(unit, target);
            }
            public List<IActions> Use((int X, int Y) fpos)
            {
                List<IActions> result = new List<IActions>();
                if (Type == AbilityType.SelectAndAttack)
                {
                    var selected = GameModeContainer.Get().GetUnit(fpos);
                    result = stand.Use(unit, selected);
                    return result;
                }
                else
                {
                    MoveUnit move = new MoveUnit(unit.fieldPosition, fpos);
                    SpendPlayerResources spend = new SpendPlayerResources(0, 1, unit.owner.idx);
                    result.Add(move);
                    result.Add(spend);
                    return result;
                }
            }
        }
    }
}
