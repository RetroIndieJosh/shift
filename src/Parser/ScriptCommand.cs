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

        public static Problem SetOnce(ref string target, string value, string varLabel)
        {
            Problem problem = null;
            if (target != null)
                problem = new OverwriteWarning(varLabel);
            target = value;
            return problem;
        }

        // this doesn't work because we can't pass the ref to lambda
        /*
                public ScriptCommand(string key, ref string target, bool allowOverwrite = false)
                {
                    OnParse = args => {
                        Problem ret = null;
                        if (target != null)
                            ret = new OverwriteWarning("var");
                        target = value;
                        return ret;
                    }
                }
                */

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