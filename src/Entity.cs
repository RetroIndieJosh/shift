using System;

namespace shift
{
    public abstract class Entity
    {
        public string Name { get; private set; }
        private string desc;

        public Entity(string name, string desc)
        {
            this.Name = name;
            this.desc = desc;
        }

        public bool Matches(string[] args)
        {
            var nameTokens = Name.Split(' ');
            if (nameTokens.Length == 1)
                return this.Name == args[0];
            throw new NotImplementedException("Matching multiword names not yet "
                + "supported. Please only use a single word for item names.");
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual void WriteDesc()
        {
            Display.Write(desc);
        }

        public void WriteName()
        {
            Display.Write(Name);
        }
    }
}