using System;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    public abstract class ScriptedEntity<EntityType> where EntityType : ScriptedEntity<EntityType>
    {
        private static List<EntityType> store = new List<EntityType>();

        // internal name (no spaces, only underscores)
        public string Name
        {
            // TODO null coalesce
            get => (name is null || name.Value is null)
                ? "(null)"
                : name.Value.Replace(' ', '_');
        }

        // display name (double underscore becomes underscore, underscore becomes space)
        public string DisplayName
        {
            get => Name;
        }

        #region Script Fields
        private readonly ScriptField<string> name = new("name", 1);
        #endregion

        protected bool isLoaded = false;
        protected List<ScriptCommand> scriptKeys;

        public ScriptedEntity()
        {
            store.Add((EntityType)this);
        }

        public ScriptedEntity(List<ScriptLine> lines, string nameKey = null) : this()
        {
            if (nameKey is not null)
            {
                if (!lines[0].Text.StartsWith(nameKey))
                    throw new Exception($"Illegal name key `{nameKey}` for line: {lines[0].Text}.");
                lines[0].ReplaceFirst(nameKey, "name");
            }
            LoadScript(lines);
        }

        protected void LoadScript(List<ScriptLine> lines)
        {
            if (lines is null || lines.Count == 0)
            {
                throw new Exception("No script lines provided to scripted entity.");
            }

            if (isLoaded)
                throw new Exception($"Tried to load {name} multiple times.");
            BindScriptKeys();
            foreach (var line in lines)
            {
                // TODO do something about the return? false means the line didn't match anything
                // otherwise make TryParse void
                _ = TryParse(line);
            }

            var problem = CheckName(Name);
            problem?.Report();
            isLoaded = true;
        }

        public static EntityType Find(string name)
        {
            return Find(name, store);
        }

        public static EntityType Find(string name, List<EntityType> list)
        {
            var matches = list.Where(e => e.Matches(name)).ToList();
            if (matches.Count == 0)
                return null;

            // TODO disambiguation
            return matches[0];
        }

        public bool Matches(string name)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(this.Name, name.Replace(' ', '_'));
        }

        public override string ToString()
        {
            return DisplayName;
        }

        protected virtual void BindScriptKeys()
        {
            if (scriptKeys is null)
                throw new Exception("Null scriptKeys. You must create scriptKeys before calling "
                    + "ScriptedEntity.BindScriptKeys().");
            scriptKeys.Add(name);
        }

        protected Problem CheckName(string name)
        {
            if (Game.instance.IsCommand(name))
                return new Problem(ProblemType.Error, $"Name clash: {name} is a command. Choose a different name.");
            else if (Item.Find(name) is not null)
                return new Problem(ProblemType.Error, $"Name clash: {name} is an existing item. Choose a different name.");
            else if (Room.Find(name) is not null)
                return new Problem(ProblemType.Error, $"Name clash: {name} is an existing room. Choose a different name.");
            return null;
        }

        // returns whether the command was parsed (not whether there were problems)
        protected virtual bool TryParse(ScriptLine line)
        {
            if (scriptKeys is null || scriptKeys.Count == 0)
            {
                throw new Exception("No commands set for scripted entity");
            }

            foreach (var command in scriptKeys)
            {
                if (command is null)
                {
                    new Problem(ProblemType.Warning,
                        $"Null command in `{this.GetType()}`")
                        .Report(line.LineNumber);
                    continue;
                }

                if (!command.IsMatch(line.Text))
                    continue;

                var problem = command.TryInvoke(line.Text);
                if (problem is not null)
                    problem.Report(line.LineNumber);
                return true;
            }
            var key = line.Text.Split('/')[0];
            new Problem(ProblemType.Warning,
                $"No matching script key in `{this.GetType()}` for `{key}`")
                .Report(line.LineNumber);
            return false;
        }
    }
}
