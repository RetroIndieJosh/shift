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

        protected virtual void BindScriptKeys() { }

        protected virtual bool TryParse(ScriptLine line)
        {
            if (scriptKeys == null || scriptKeys.Count == 0)
                throw new Exception("No commands set for scripted entity");
            foreach (var command in scriptKeys)
            {
                if (!command.IsMatch(line.Text))
                    continue;
                var problem = command.TryInvoke(line.Text);
                if (problem == null)
                    return true;
                problem.Report(line.LineNumber);
            }
            return false;
        }
    }
}