using System;
using System.Collections.Generic;

namespace shift
{
    public abstract class ScriptedEntity
    {
        public string Name { get; protected set; }

        protected List<ScriptCommand> commands;

        public ScriptedEntity() : this("Nowhere", "An empty place.") { }

        public ScriptedEntity(string name, string desc)
        {
            Name = name;
        }

        public bool Matches(string name)
        {
            var nameTokens = Name.Split(' ');
            if (nameTokens.Length == 1)
                return this.Name == name;
            throw new NotImplementedException("Matching multiword names not yet "
                + "supported. Please only use a single word for item names.");
        }

        public override string ToString()
        {
            return Name;
        }

        // TODO get rid of this and access Name directly
        public void WriteName()
        {
            Display.Write(Name);
        }

        protected virtual bool TryParse(ScriptLine line)
        {
            foreach (var command in commands)
            {
                if (!command.IsMatch(line.Text))
                    continue;
                var problem = command.TryInvoke(line.Text);
                if (problem == null)
                    return true;
                if (problem.Type == ProblemType.Error)
                    ShiftParser.Error(problem.Message, line.LineNumber);
                else if (problem.Type == ProblemType.Warning)
                    ShiftParser.Warn(problem.Message, line.LineNumber);
            }
            return false;
        }
    }
}