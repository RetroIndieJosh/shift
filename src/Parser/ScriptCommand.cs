using System;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    public class ScriptCommand
    {
        private string key;
        private int minimumArgCount = 0;
        private Func<List<string>, Problem> OnParse;

        // TODO max arg count?
        public ScriptCommand(string key, int minArgCount, Func<List<string>, Problem> OnParse)
        {
            this.key = key;
            this.minimumArgCount = minArgCount;
            this.OnParse = OnParse;
        }

        public bool IsMatch(string line)
        {
            return line.Split('/').ToList().Select(token => token.Trim()).ToList()[0] == key;
        }

        // returns whether invocation was attempted (matches key)
        public Problem TryInvoke(string line)
        {
            var tokens = ShiftParser.Tokenize(line);
            if (tokens.Count < minimumArgCount + 1)
                return new Problem(ProblemType.Error, $"Not enough arguments to command `{key}`");
            return OnParse.Invoke(tokens.Skip(1).ToList());
        }
    }
}