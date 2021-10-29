using System;
using System.Collections.Generic;

namespace csif
{
    public class Item
    {
        static List<Item> items = new List<Item>();

        public Room Location
        {
            get => location;
            set
            {
                if (value != null && location != null)
                    throw new Exception($"Set item {this} to new location "
                        + $"{value} before removing from previous room {location}");
                location = value;
            }
        }

        private Room location;

        private string name;
        private string desc;

        private bool canTake = false;
        private bool canUse = false;
        private Item useTarget = null;

        public static void Where(string[] args)
        {
            foreach (var item in items)
            {
                if (!item.Matches(args))
                    continue;

                Console.WriteLine($"[{item} is in {item.location}]");
                return;
            }
            Console.WriteLine($"[could not find item {string.Join(' ', args)}]");
        }

        public Item(string name, string desc)
        {
            this.name = name;
            this.desc = desc;
            items.Add(this);
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