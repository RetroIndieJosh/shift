using System;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    public class ItemStateMachine
    {
        private string parent;
        private List<ItemState> states = new List<ItemState>();

        public ItemStateMachine(string parent)
        {
            this.parent = parent;
        }

        public void AddState(string[] stateNames, int defaultStateIndex = 0)
        {
            foreach (var stateName in stateNames)
                if (HasState(stateName))
                    throw new Exception($"Duplicate state {stateName} in {parent}");

            states.Add(new ItemState(stateNames, defaultStateIndex));
        }

        public bool HasState(string stateName)
        {
            return states.Select(s => s.HasState(stateName)).Count() > 0;
        }

        public void SetState(string stateName)
        {
            foreach (var state in states)
            {
                if (state.HasState(stateName))
                {
                    state.State = stateName;
                    return;
                }
            }

            throw new Exception($"No state {stateName} in {this}");
        }

        public override string ToString()
        {
            return String.Join(' ', states.Select(s => s.State));
        }
    }
}