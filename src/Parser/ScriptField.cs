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
    public class ScriptField<FieldType> : ScriptData<FieldType>
    {
        public ScriptField(string key, int minArgCount) : base(key, minArgCount)
        { }

        public override Problem Parse(List<string> args)
        {
            var problem = base.Parse(args);
            if (problem is not null)
                return problem;

            if (typeof(FieldType).IsPrimitive || typeof(FieldType) == typeof(string))
            {
                Value = TConverter.ChangeType<FieldType>(args[0]);
                return null;
            }
            // TODO lists

            return new Problem(ProblemType.Error, $"Field type not supported: `{typeof(FieldType)}`");
        }
    }
}
