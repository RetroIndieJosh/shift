// SHIFT - a cross-platform toolkit for streamlined, scripted text adventures
// Copyright (C) 2022 Joshua D McLean
//
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// this program as LICENSE.txt. If not, see <https://www.gnu.org/licenses/>.

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
