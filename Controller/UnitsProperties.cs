﻿using System;
using System.Collections.Generic;
using System.Text;
using InterfaceOfObjects;
using Newtonsoft.Json;

namespace Controller
{
    public class MoveStep
    {
        float distance = 1f;
        bool isRoadBounce = true;
        bool isFree = false;
    }

    public enum ActionState
    {
        Ready = 0,
        InProcess = 1,
        Ended = 2,
    }

    public class ActionPoint
    {
        public ActionState State => _State;

        public readonly int neededAttackPoints;

        public readonly int neededMovePoints;

        private List<UnitActionPoint> bindUnitActions;

        public ActionPoint(UnitPresset unit, int attack, int move, List<UnitActionPoint> bindUnitActions)
        {
            neededAttackPoints = attack;
            neededMovePoints = move;
            this.bindUnitActions = new List<UnitActionPoint>(bindUnitActions);
            ActionsIsSpend += unit.OnSpendActionsHandler;
        }

        [JsonConstructor]
        public ActionPoint()
        { }

        public event SpendActions ActionsIsSpend;

        private ActionState _State = ActionState.Ready;

        public bool IsReady(Player owner)
        {
            var result = GameModeContainer.Get().
                IsEnoughResources(neededAttackPoints, neededMovePoints, owner);
            if (result)
                foreach (var unitAction in bindUnitActions)
                {
                    if (unitAction.State != ActionState.Ready)
                        return false;
                }
            return result;
        }

        public void ToInProgres()
        {
            foreach (var unitAction in bindUnitActions)
            {
                unitAction.State = ActionState.InProcess;   
            }
            _State = ActionState.InProcess;
            ActionsIsSpend?.Invoke();
        }

        public void Return()
        {
            foreach(var unitAction in bindUnitActions)
            {
                unitAction.State = ActionState.Ready;
            }
            _State = ActionState.Ready;
            ActionsIsSpend?.Invoke();
        }

        public void Spend()
        {
            _State = ActionState.Ended;
            foreach(var unitAction in bindUnitActions)
            {
                unitAction.State = ActionState.Ended;
            }
            ActionsIsSpend?.Invoke();
        }

        public void Refresh()
        {
            _State = ActionState.Ready;
            foreach(var unitAction in bindUnitActions)
            {
                unitAction.Refresh();
            }
            ActionsIsSpend?.Invoke();
        }
    }

    public class UnitActionPoint
    {
        public ActionName Name;

        public ActionState State = ActionState.Ready;

        public void Refresh() => State = ActionState.Ready;

        public UnitActionPoint(ActionName actionName)
        {
            Name = actionName;
        }
        [JsonConstructor]
        public UnitActionPoint()
        { }
    }

    public enum ActionName
    {
        Move = 0,
        Attack = 1,
    }

    public enum AbilityType
    {
        PreemptiveAttack = 0,
        Attack = 1,
        AfterBattle = 2,
        RangeAttack = 3,
        AttackWithoutResponse = 4,
        Heal = 5,
        ActionWitoutTargetSelect = 6,
        SelectAndAttack = 7,
        Move = 8,
    }

    public delegate void SpendActions();

    public enum BattleStage
    {
        Preemptive = 0,
        MainAttack = 1,
        ResponseAttack = 2,
        FinalStage = 3,
    }
}
