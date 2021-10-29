using System;

namespace csif
{
    public class Item
    {
        private string name;
        private string desc;

        public Item(string name, string desc)
        {
            this.name = name;
            this.desc = desc;
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