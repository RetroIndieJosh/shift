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
        public bool IsCarried { get; set; } = false;
        private Item useTarget = null;

        public static Item Find(string[] args, List<Item> items)
        {
            foreach (var item in items)
            {
                if (item.Matches(args))
                    return item;
            }
            return null;
        }

        public static void Where(string[] args)
        {
            var item = Item.Find(args, items);
            if (item == null)
            {
                Console.WriteLine($"[no item by name {string.Join(' ', args)}]");
                return;
            }

            string where = "";
            if (item.IsCarried)
                where = "inventory";
            else if (item.location == null)
                where = "limbo";
            else
                where = item.location.ToString();

            Console.WriteLine($"[{item} is in {where}]");
        }

        public Item(string name, string desc, bool canTake = false) : base(name, desc)
        {
            items.Add(this);
            CanTake = canTake;
        }
    }
}