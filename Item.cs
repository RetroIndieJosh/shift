using System;

namespace csif
{
    public class Item
    {
        private string name;
        private string desc;

        private bool canTake = false;
        private bool canUse = false;
        private Item useTarget = null;

        public Item(string name, string desc)
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