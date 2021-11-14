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
            if (Value != null)
                return new OverwriteWarning(Key);
            if (args.Count == 0)
                return new Problem(EmptyProblemType, $"No value provided to field `{Key}`.");
            return null;
        }
    }
}
