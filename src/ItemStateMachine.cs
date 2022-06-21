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