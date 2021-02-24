using System;
using System.Collections.Generic;
using System.Text;
using Controller.Actions;
using Controller.Abilities;
using UnitsAnPathFinding;
using Controller.Requests;

namespace Controller
{
    internal class GameModeLogic
    {
        private bool isDelay = false;
        private GameModeServer GameMode;

        private AbilityType InteraptionAction;

        private Player PlayerBeforeIteration;

        private (StandPresset stand, UnitPresset unit, UnitPresset sender,UnitPresset target) AwaitSelection;

        private List<(StandPresset stand, UnitPresset unit, UnitPresset sender,UnitPresset target)> DelayedActions;

        public Player CurrentPlayer
        { get; set; }

        public GameModeLogic(GameModeServer GameMode, Player FirstPlyaer)
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
                throw new NotImplementedException();
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

        public List<IActions> ProcessMeleeBattle(UnitPresset unit, UnitPresset target, int AbilityIdx)
        {
            isDelay = false;
            DelayedActions = new List<(StandPresset stand, UnitPresset unit, UnitPresset sender, UnitPresset target)>();
            List<IActions> result = new List<IActions>();
            UnitsInBattle.Clear();
            UnitsInBattle.Add(unit);
            UnitsInBattle.Add(target);

            var Attack = unit.GetAbility(AbilityIdx);


            if (Attack.AbilityType == AbilityType.RangeAttack)
            {
                result.AddRange(Attack.Use(target));
                GameMode.ProcessActions(result);
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
            int delayedActionsIdx = -1;
            foreach (var stand in listStands)
            {
                if (stand.stand.AbilityType == AbilityType.SelectAndAttack && !isDelay)
                {
                    isDelay = true;
                    isHereDelay = true;
                    AwaitSelection = (stand.stand, stand.unit, unit, target);
                    delayedActionsIdx = stand.stand.idx;
                    InteraptionAction = AbilityType.SelectAndAttack;
                    continue;
                }
                if (!isDelay)
                {
                    result.AddRange(stand.stand.Use(unit, target));
                }
                else
                {
                    DelayedActions.Add((stand.stand, stand.unit, unit, target));
                    continue;
                }
            }
            
            if (isHereDelay)
                IteruptAndMakeUserRequest(unit ,AwaitSelection.unit, target, delayedActionsIdx,result);
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
            GameMode.ProcessActions(result);
            return result;
        }

        public List<IActions> ApplyAbilityWithoutSelection(UnitPresset unit, AbilityPresset ability)
        {
            var result = new List<IActions>();
            result.AddRange(ability.Use(unit));
            GameMode.ProcessActions(result);
            return result;
        }

        public List<IActions> ProcessIteraptedAndNextActions(UnitPresset unit, (int X, int Y) fpos)
        {
            isDelay = false;
            List<IActions> result = new List<IActions>();
            if (InteraptionAction == AbilityType.SelectAndAttack)
            {
                var target = GameMode.GetUnit(fpos);
                result.AddRange(AwaitSelection.stand.Use(unit, target));
            }
            bool isDelaid = false;
            var newDealiedActions = new List<(StandPresset stand, UnitPresset unit, UnitPresset target)>();
            int delayedActionsIdx = -1;
            
            foreach (var ability in DelayedActions)
            {
                if (isDelaid)
                {
                    DelayedActions.Add(ability);
                    continue;
                }
                if ( ability.stand.AbilityType != AbilityType.SelectAndAttack)
                    result.AddRange(ability.stand.Use(ability.sender, ability.target));
                else
                {
                    isDelaid = true;
                    AwaitSelection = ability;
                    InteraptionAction = AbilityType.SelectAndAttack;
                    delayedActionsIdx = ability.stand.idx;
                }
            }
            if (isDelaid)
            {
                GameMode.ProcessActions(result);
                IteruptAndMakeUserRequest(AwaitSelection.sender, 
                    AwaitSelection.unit, 
                    AwaitSelection.target,
                    AwaitSelection.stand.idx, 
                    result);
                return result;
            }
            GameMode.ChangePlayers(GameMode.CurrentPlayer, PlayerBeforeIteration);
            GameMode.ProcessActions(result);
            GameMode.State = GameModeState.Standart;
            DelayedActions = null;
            return result;
        }

        private delegate List<IActions> DelayStand(UnitPresset unit, UnitPresset target);
    }
}
