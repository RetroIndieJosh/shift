using System;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    public class Item : Entity
    {
        const string DefaultTakeDesc = "You take {0}.";
        const string DefaultUseDesc = "You take {0}.";

        static public Item CurTarget { get; private set; } = null;

        static private List<Item> inventory = new List<Item>();
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
        private string takeDesc = null;
        private string useDesc = null;

        private bool CanTake { get => takeDesc != null; }
        private bool CanUse { get => useDesc != null; }
        private bool isCarried = false;

        private ItemStateMachine stateMachine;

        // find an item loaded in the game
        public static Item Find(string name)
        {
            return Find(new string[] { name }, items);
        }

        // find an item in a specific list i.e. room contents
        public static Item Find(string[] args, List<Item> items)
        {
            var matches = items.Where(item => item.Matches(args)).ToList();
            if (matches.Count == 0)
                return null;

            // TODO disambiguation
            return matches[0];
        }

        public static Item FindInInventory(string[] args)
        {
            return Find(args, inventory);
        }

        public static List<string> GetInventoryNames()
        {
            return inventory.Select(item => item.Name).ToList();
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
            inventory.ForEach(item => Display.WriteLine($"\t{item}"));
        }

        // item names use underscores internally for autocompletion
        public Item(string name, string desc, string takeDesc = null, string useDesc = null)
            : base(name.Replace(' ', '_'), desc)
        {
            this.takeDesc = (takeDesc == "" ? DefaultTakeDesc : takeDesc);
            this.useDesc = (useDesc == "" ? DefaultUseDesc : useDesc);

            this.stateMachine = new ItemStateMachine(name);

            items.Add(this);
        }

        public void AddState(string[] stateNames, int defaultStateIndex = 0)
        {
            stateMachine.AddState(stateNames, defaultStateIndex);
        }

        public void Target()
        {
            if (isCarried)
            {
                if (!CanUse)
                {
                    Use();
                    return;
                }

                Display.WriteLine($"Select an option for {this}:"
                        + "\n\te(x)amine"
                        + "\n\t(c)ombine"
                        + "\n\t(u)se"
                        + "\n\t(b)ack"
                );

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
            if (CanTake)
            {
                Take();
                return;
            }

            WriteDesc();
            Display.WriteLine();

            if (!isCarried)
                CurTarget = this;
        }

        public override void WriteDesc()
        {
            base.WriteDesc();
            Display.Write($" [{stateMachine}]");
        }

        private void Take()
        {
            Location.RemoveItem(this);
            inventory.Add(this);
            isCarried = true;
            Display.WriteLine(takeDesc, this);
            Display.WriteLine("[taken]");
            return;
        }

        private void Use()
        {
            WriteDesc();
            Display.WriteLine();
        }
    }
}