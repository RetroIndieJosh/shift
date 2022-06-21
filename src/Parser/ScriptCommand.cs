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
    public class ScriptCommand
    {
        private readonly string key;
        private readonly int minArgCount = 0;

        protected Func<List<string>, Problem> OnParse;

        public static Problem SetOnce(ref string target, string value, string varLabel)
        {
            Problem problem = null;
            if (target is not null)
                problem = new OverwriteWarning(varLabel);
            target = value;
            return problem;
        }

        // TODO max arg count?
        public ScriptCommand(string key, int minArgCount, Func<List<string>, Problem> OnParse = null)
        {
            this.key = key;
            this.minArgCount = minArgCount;
            this.OnParse = OnParse;
        }

        public bool IsMatch(string line)
        {
            return line.Split('/').ToList().Select(token => token.Trim()).ToList()[0] == key;
        }

        public Problem TryInvoke(string line)
        {
            if (OnParse is null)
                return new Problem(ProblemType.Warning, $"Command `{key}` not implemented (no OnParse)");

            var tokens = ShiftParser.Tokenize(line);
            if (tokens.Count < minArgCount + 1)
                return new Problem(ProblemType.Error, $"Not enough arguments to command `{key}`");

            return OnParse.Invoke(tokens.Skip(1).ToList());
        }
    }
}
