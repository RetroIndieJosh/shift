using System;
using System.Linq;

namespace shift
{

    public class ItemState
    {
        public string State
        {
            get => availableStates[curStateIndex];
            set => SetState(value);
        }

        int curStateIndex = 0;
        string[] availableStates;

        public ItemState(string[] availableStates, int defaultStateIndex = 0)
        {
            if (defaultStateIndex >= availableStates.Length)
                throw new IndexOutOfRangeException(
                        $"Default state index {defaultStateIndex} must be < number of available "
                        + $"states ({availableStates.Length})");
            this.availableStates = availableStates;
            curStateIndex = defaultStateIndex;
        }

        public bool HasState(string state)
        {
            return availableStates.Contains(state);
        }

        public void SetState(string state)
        {
            if (!availableStates.Contains(state))
                throw new Exception($"No '{state}' in item state machine (check HasState first!)");
            curStateIndex = Array.IndexOf(availableStates, state);
        }
    }
}