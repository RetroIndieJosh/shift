using System;
using System.Collections.Generic;

namespace csif
{
    public class Item : Entity
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

        public bool CanTake { get; set; }
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

        public Item(string name, string desc, bool canTake = false) : base(name, desc)
        {
            items.Add(this);
            CanTake = canTake;
        }
    }
}