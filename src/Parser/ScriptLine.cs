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
            var spaces = line.TakeWhile(c => c == ' ').Count();
            if (spaces % SpacesPerIndent != 0)
                throw new Exception($"Invalid spacing in SHIFT file:\n{line}");

            IndentLevel = spaces / SpacesPerIndent;
            Text = line.Trim();

            LineNumber = number;
        }
    }

}