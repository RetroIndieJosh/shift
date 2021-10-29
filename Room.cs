using System;
using System.Collections.Generic;

namespace csif
{
    public class Room : Item
    {
        private List<Item> items = new List<Item>();

        public Room(string name, string desc) : base(name, desc)
        {
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public void WriteAll()
        {
            Console.Write("-= ");
            WriteName();
            Console.Write("=- ");
            WriteDesc();

            if (items.Count == 0)
                return;

            var itemStr = string.Join(", ", items);
            Console.Write("You also see {0}.", itemStr);
        }
    }
}
