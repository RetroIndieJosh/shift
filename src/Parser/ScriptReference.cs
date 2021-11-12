using System.Collections.Generic;

namespace shift
{
    public class ScriptReference<RefType> : ScriptData<RefType> where RefType : ScriptedEntity<RefType>
    {
        public ScriptReference(string key, int minArgCount) : base(key, minArgCount)
        { }

        override public Problem Parse(List<string> args)
        {
            var problem = base.Parse(args);
            if (problem != null)
                return problem;

            Value = ScriptedEntity<RefType>.Find(args[0]);
            if (Value == null)
                return new Problem(ProblemType.Error,
                    $"Parsing ScriptRef: No {typeof(RefType)} found by name {args[0]}.");
            return null;
        }
    }
}