using System;
using System.Collections.Generic;
using System.Text;

namespace Controller.Actions
{
    public interface IActions
    {
        public int idx
        { get; set; }

        public void forward();

        public void reverse();

    }

    public enum ActionDirection
    { 
        Direct = 0,
        Reverse = 1,
    }

    public class ActionManager
    {
        private List<IActions> PreviousStates = new List<IActions>();
        private List<IActions> CashedActions = new List<IActions>();
        public int ActionIdx => _ActionIdx;
        public int CurrentActionIdx = 0;
        private int _ActionIdx = 0;
        public int NextActionIdx
        {
            get
            {
                _ActionIdx++;
                return _ActionIdx;
            }
        }

        public void ApplyActions(List<IActions> actions)
        {
            CashedActions.Clear();
            foreach (var action in actions)
            {
                if (CurrentActionIdx < action.idx)
                {
                    action.forward();
                    CurrentActionIdx = action.idx;
                    PreviousStates.Add(action);
                }
            }
        }

        public List<IActions> GetMissedActions(int clientActionIndx)
        {
            List<IActions> response = new List<IActions>();
            bool trigger = false;
            int lastSharredElement = 0;
            foreach (var action in PreviousStates)
            {
                if (trigger)
                    response.Add(action);
                if (action.idx == clientActionIndx)
                {
                    trigger = true;
                }
                else
                {
                    if (action.idx < clientActionIndx)
                        lastSharredElement = PreviousStates.IndexOf(action);
                }
            }

            if (!trigger)
            {
                for (int i = lastSharredElement; i < PreviousStates.Count; i++)
                {
                    response.Add(PreviousStates[i]);
                }
            }

            return response;
        }

        public void UnDoActions()
        {

        }
    }
}
