using System;
using System.Collections.Generic;

namespace shift
{
    public abstract class ScriptedEntity
    {
        public string Name
        {
            get => name;
            protected set
            {
                name = value.Replace(' ', '_');
            }
        }

        private string name;

        protected List<ScriptCommand> scriptKeys;

        public ScriptedEntity()
        {
            Name = "Anonymous";
        }

        public ScriptedEntity(List<ScriptLine> lines)
        {
            if (lines == null || lines.Count == 0)
                throw new Exception("No script lines provided to scripted entity");
            BindScriptKeys();
            foreach (var line in lines)
                TryParse(line);
        }

        public bool Matches(string name)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(this.Name, name.Replace(' ', '_'));
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

        protected virtual void BindScriptKeys() { }

        // returns whether the command was parsed (not whether there were problems)
        protected virtual bool TryParse(ScriptLine line)
        {
            if (scriptKeys == null || scriptKeys.Count == 0)
                throw new Exception("No commands set for scripted entity");
            foreach (var command in scriptKeys)
            {
                if (!command.IsMatch(line.Text))
                    continue;
                var problem = command.TryInvoke(line.Text);
                if (problem != null)
                    problem.Report(line.LineNumber);
                return true;
            }
            var key = line.Text.Split('/')[0];
            new Problem(ProblemType.Warning, $"No matching script key in `{this.GetType()}` for `{key}`").Report(line.LineNumber);
            return false;
        }
    }
}