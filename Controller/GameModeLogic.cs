using System;
using System.Collections.Generic;
using System.Text;
using Controller.Actions;
using Controller.Abilities;
using UnitsAnPathFinding;

namespace Controller
{
    internal class GameModeLogic
    {
        private GameModeServer GameMode;

        public Player CurrentPlayer
        { get; set; }
        
        
        public GameModeLogic(GameModeServer GameMode, Player FirstPlyaer)
        {
            this.GameMode = GameMode;
            CurrentPlayer = FirstPlyaer;
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
                MoveUnit moveUnit = new MoveUnit(unit.fieldPosition,
                    pathToken.fieldPosition);
                ChangeActionPointState spendActionPoints = new ChangeActionPointState(
                    pathToken.fieldPosition, 0, ActionState.Ready ,ActionState.Ended);
                var spendPlayerResources = TraitePlayersResources(unit.owner, 0, 1);
                GameMode.ProcessActions(new List<IActions>() { moveUnit, spendActionPoints, spendPlayerResources });
                result.Add(moveUnit);
                result.Add(spendActionPoints);
                result.Add(spendPlayerResources);
            }   
            return result;
        }

        public IActions TraitePlayersResources(Player player,int attackPoints=0, int movePoints=0, bool isReverse=false)
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

            result.AddRange(target.Response(unit));
            GameMode.ProcessActions(result);

            standActions = CheckInAreaAbilities(unit, target, BattleStage.MainAttack);
            GameMode.ProcessActions(standActions);
            result.AddRange(standActions);

            standActions = CheckInAreaAbilities(unit, target, BattleStage.ResponseAttack);
            GameMode.ProcessActions(standActions);
            result.AddRange(standActions);
            return result;
            
        }

        private List<IActions> CheckInAreaAbilities(UnitPresset unit, UnitPresset target, BattleStage stage)
        {
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
                result.AddRange(stand.stand.Use(unit, target));
            }
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
    }
}
