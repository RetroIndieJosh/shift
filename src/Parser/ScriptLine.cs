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
