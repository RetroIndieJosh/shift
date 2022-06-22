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

namespace shift
{
    public enum ProblemType
    {
        Error, Warning, None
    }

    public class OverwriteWarning : Problem
    {
        public OverwriteWarning(string varName) : base(ProblemType.Warning, $"New value overwrites previous `{varName}`") { }
    }

    public class Problem
    {
        public string Message { get; private set; }

        public ProblemType Type { get; private set; }

        public Problem(ProblemType type, string message)
        {
            Message = message;
            Type = type;
        }

        public void Report(int lineNumber = 0)
        {
            if (Type == ProblemType.Error)
                Display.Error(Message, lineNumber);
            else if (Type == ProblemType.Warning)
                Display.Warn(Message, lineNumber);
        }
    }
}
