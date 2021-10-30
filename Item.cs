using System;
using System.Collections.Generic;

namespace csif
{
    public class Item : Entity
    {
        const string DefaultTakeDesc = "You take {0}.";
        const string DefaultUseDesc = "You take {0}.";

        static public Item CurTarget { get; private set; } = null;

        static private List<Item> inventory = new List<Item>();
        static List<Item> items = new List<Item>();

        private string takeDesc = null;
        private string useDesc = null;

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

        private bool canTake = false;
        private bool canUse = false;
        private List<Item> usableOn = new List<Item>();
        private bool isCarried = false;

        public static Item Find(string[] args, List<Item> items)
        {
            foreach (var item in items)
            {
                if (item.Matches(args))
                    return item;
            }
            return null;
        }

        public static Item FindInInventory(string[] args)
        {
            return Find(args, inventory);
        }

        public static void Where(string[] args)
        {
            var item = Item.Find(args, items);
            if (item == null)
            {
                Display.WriteLine($"[no item by name {string.Join(' ', args)}]");
                return;
            }

            string where = "";
            if (item.isCarried)
                where = "inventory";
            else if (item.location == null)
                where = "limbo";
            else
                where = item.location.ToString();

            Display.WriteLine($"[{item} is in {where}]");
        }

        static public void WriteInventory()
        {
            if (inventory.Count == 0)
            {
                Display.WriteLine("You are carrying nothing.");
                return;
            }

            Display.WriteLine("You are carrying:");
            foreach (var item in inventory)
                Display.WriteLine($"\t{item}");
        }

        public Item(string name, string desc, string takeDesc = null, string useDesc = null)
            : base(name, desc)
        {
            this.canTake = (takeDesc != null);
            this.takeDesc = (takeDesc == "" ? DefaultTakeDesc : takeDesc);

            this.canUse = (useDesc != null);
            this.useDesc = (useDesc == "" ? DefaultUseDesc : useDesc);

            items.Add(this);
        }

        public void Target()
        {
            if (isCarried)
            {
                if (!canUse)
                {
                    WriteDesc();
                    Display.WriteLine();
                    return;
                }

                Display.WriteLine("[{0}] Would you like to e(x)amine, (c)ombine, (u)se, or (b)ack?",
                    this);
                do
                {
                    var ch = Display.ReadKey(true);
                    if (ch.KeyChar == 'x' || ch.KeyChar == 'X')
                    {
                        WriteDesc();
                        Display.WriteLine();
                        return;
                    }
                    else if (ch.KeyChar == 'u' || ch.KeyChar == 'U')
                    {
                        Display.WriteLine(useDesc, this);
                        return;
                    }
                    else if (ch.KeyChar == 'c' || ch.KeyChar == 'C')
                    {
                        Display.WriteLine("[combine not yet implemented]");
                        return;
                    }
                    else if (ch.KeyChar == 'b' || ch.KeyChar == 'B' || ch.Key == ConsoleKey.Escape)
                    {
                        return;
                    }
                } while (true);
            }
            if (canTake)
            {
                Location.RemoveItem(this);
                inventory.Add(this);
                isCarried = true;
                Display.WriteLine(takeDesc, this);
                Display.WriteLine("[taken]");
                return;
            }

            WriteDesc();
            Display.WriteLine();

            if (!isCarried)
                CurTarget = this;
        }

    }
}