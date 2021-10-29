using System;

namespace csif
{
    public abstract class Entity
    {
        private string name;
        private string desc;

        public Entity(string name, string desc)
        {
            this.name = name;
            this.desc = desc;
        }

        public bool Matches(string[] args)
        {
            var nameTokens = name.Split(' ');
            if (nameTokens.Length == 1)
                return this.name == args[0];
            throw new NotImplementedException("Matching multiword names not yet "
                + "supported. Please only use a single word for item names.");
        }

        public override string ToString()
        {
            return name;
        }

        public void WriteDesc()
        {
            Console.Write(desc);
        }

        public void WriteName()
        {
            Console.Write(name);
        }
    }
}