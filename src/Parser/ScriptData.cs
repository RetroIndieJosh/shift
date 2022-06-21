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

using System.Collections.Generic;

namespace shift
{
    public abstract class ScriptData<DataType> : ScriptCommand
    {
        public DataType Value { get; protected set; }
        public ProblemType EmptyProblemType { protected get; set; } = ProblemType.None;

        protected string Key { get; private set; }

        public ScriptData(string key, int minArgCount) : base(key, minArgCount)
        {
            Key = key;
            OnParse = Parse;
        }

        public virtual Problem Parse(List<string> args)
        {
            if (Value is not null)
                return new OverwriteWarning(Key);
            if (args.Count == 0)
                return new Problem(EmptyProblemType, $"No value provided to field `{Key}`.");
            return null;
        }
    }
}
