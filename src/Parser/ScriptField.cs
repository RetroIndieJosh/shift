using System.Collections.Generic;

namespace shift
{
    public class ScriptField<FieldType> : ScriptData<FieldType>
    {
        public ScriptField(string key, int minArgCount) : base(key, minArgCount)
        { }

        override public Problem Parse(List<string> args)
        {
            var problem = base.Parse(args);
            if (problem != null)
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