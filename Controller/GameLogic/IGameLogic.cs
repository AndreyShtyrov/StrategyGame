using Controller.Actions;
using System;
using System.Collections.Generic;
using System.Text;
using UnitsAnPathFinding;

namespace Controller.GameLogic
{
    public interface IGameLogic
    {
        public Player CurrentPlayer
        { get; set; }

        public void AttackUnit(UnitPresset unit, UnitPresset target, int AbilityIdx);

        public void Move(UnitPresset unit, PathToken pathToken);

        public void UpDownStand(UnitPresset unit, int StandIdx);

        public void CreateUnit(string name, (int X, int Y) fpos, Player owner, string typeUnit = "None");

        public void SwitchTurn();

        public void ApplyAbilityWithoutSelection(UnitPresset unit, AbilityPresset Ability);

        public void ProcessIteraptedAndNextActions(UnitPresset unit, (int X, int Y) fpos);
    }
}
