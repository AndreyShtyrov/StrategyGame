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

        public GameModeLogic(GameModeServer GameMode)
        {
            this.GameMode = GameMode;
        }

        List<UnitPresset> UnitsInBattle = new List<UnitPresset>();
  
        public List<IActions> Move(UnitPresset unit, PathToken pathToken)
        {
            List<IActions> result = new List<IActions>();
            if (unit.MoveActionPoint.Active(unit.owner) ||
                unit.MoveActionPoint.State == ActionState.InProcess)
            {
                MoveUnit moveUnit = new MoveUnit();
                moveUnit.StartPosition = unit.fieldPosition;
                moveUnit.EndPosition = pathToken.fieldPosition;
                SpendActionPoints spendActionPoints = new SpendActionPoints();
                spendActionPoints.AbilityIndx = 0;
                spendActionPoints.Source = pathToken.fieldPosition;
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
            var playersResources = TraitePlayersResources(
                unit.owner,
                Attack.actionPoint.neededAttackPoints,
                Attack.actionPoint.neededMovePoints);
            GameMode.ProcessActions(new List<IActions>() { playersResources });
            result.Add(playersResources);

            DealDamage dealDamage;

            if (Attack.AbilityType == AbilityType.RangeAttack)
            {
                dealDamage = new DealDamage(
                    unit.fieldPosition,
                    target.fieldPosition,
                    AbilityIdx);
                result.Add(dealDamage);
                GameMode.ProcessActions(new List<IActions>() { dealDamage });
                return result;
            }

            var standActions = CheckInAreaAbilities(unit, target, BattleStage.Preemptive);
            GameMode.ProcessActions(standActions);
            result.AddRange(standActions);

            if (unit.currentHp > 0)
            {
                dealDamage = new DealDamage(
                    unit.fieldPosition,
                    target.fieldPosition,
                    1);
                result.Add(dealDamage);
                GameMode.ProcessActions(new List<IActions> { dealDamage });
            }

            standActions = CheckInAreaAbilities(unit, target, BattleStage.MainAttack);
            GameMode.ProcessActions(standActions);
            result.AddRange(standActions);

            if (target.currentHp > 0)
            {
                var responseDamage = new ResponseAttack(
                     unit.fieldPosition,
                     target.fieldPosition);
                result.Add(responseDamage);
                GameMode.ProcessActions(new List<IActions> { responseDamage });
            }

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
                foreach (var stand in lunit.Stands)
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
                result.Add(
                    new DealDamage(stand.unit.fieldPosition,
                        target.fieldPosition,
                        stand.stand.idx));
            }
            return result;
        }

        public List<IActions> UpDownStand(UnitPresset unit, int StandIdx)
        {
            List<IActions> result = new List<IActions>();
            var stand = unit.GetStand(StandIdx);
            IActions playerResources;
            if (stand.point.State == ActionState.Ended)
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
    }
}
