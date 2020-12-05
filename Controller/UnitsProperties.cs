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

        private readonly int neededAttackPoints;

        private readonly int neededMovePoints;

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

        public bool Active(Player owner)
        {
            foreach (var unitAction in bindUnitActions)
            {
                if (unitAction.State != ActionState.Ready)
                    return false;
                else
                    unitAction.State = ActionState.InProcess;
            }
            _State = ActionState.InProcess;
            var result = GameModeServer.Get().SpendResources(neededAttackPoints, neededMovePoints, owner);
            if (result)
                _State = ActionState.InProcess;
            else
            {
                foreach (var unitAction in bindUnitActions)
                {
                    unitAction.Refresh();
                }
            }
            ActionsIsSpend?.Invoke();
            return result;
        }
        
        public void Return(Player owner)
        {
            foreach(var unitAction in bindUnitActions)
            {
                unitAction.State = ActionState.Ready;
            }
            _State = ActionState.Ready;
            GameModeServer.Get().ReturnResources(neededAttackPoints, neededMovePoints, owner);
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

        public ActionState GetActionState() => _State;

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
    }

    public delegate void SpendActions();

}
