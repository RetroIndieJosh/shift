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
    public class ScriptLine
    {
        const int SpacesPerIndent = 4;

        public int LineNumber { get; private set; }
        public int IndentLevel { get; private set; }
        public string Text { get; private set; }

        public ScriptLine(string line, int number)
        {
            LineNumber = number;

            if (line is null)
            {
                IndentLevel = 0;
                return;
            }

            var spaces = line.TakeWhile(c => c == ' ').Count();
            if (spaces % SpacesPerIndent != 0)
            {
                ShiftParser.Error($"Invalid spacing: expected mult of 4, got {spaces}", number);
                return;
            }

            IndentLevel = spaces / SpacesPerIndent;
            Text = line.Trim();
        }

        public void Replace(string oldStr, string newStr)
        {
            Text = Text.Replace(oldStr, newStr);
        }

        public void ReplaceFirst(string oldStr, string newStr)
        {
            Text = Text.ReplaceFirst(oldStr, newStr);
        }
    }

}
