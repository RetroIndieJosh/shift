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
    public class ScriptReference<RefType> : ScriptData<RefType> where RefType : ScriptedEntity<RefType>
    {
        public ScriptReference(string key, int minArgCount) : base(key, minArgCount)
        { }

        public override Problem Parse(List<string> args)
        {
            var problem = base.Parse(args);
            if (problem is not null)
                return problem;

            Value = ScriptedEntity<RefType>.Find(args[0]);
            if (Value is null)
                return new Problem(ProblemType.Error,
                    $"Parsing ScriptRef: No {typeof(RefType)} found by name {args[0]}.");
            return null;
        }
    }
}
