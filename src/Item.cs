using System;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    public class Item : ScriptedEntity
    {
        const string DefaultTakeDesc = "You take {0}.";

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
        private string examineDesc = null;
        private string takeDesc = null;

        private bool CanTake { get => takeDesc != null; }
        // TODO usable condition
        private bool CanUse { get => false; }
        private bool isCarried = false;

        private ItemStateMachine stateMachine;

        // find an item loaded in the game
        public static Item Find(string name)
        {
            return Find(name, items);
        }

        // find an item in a specific list i.e. room contents
        public static Item Find(string name, List<Item> items)
        {
            var matches = items.Where(item => item.Matches(name)).ToList();
            if (matches.Count == 0)
                return null;

            // TODO disambiguation
            return matches[0];
        }

        public static Item FindInInventory(string name)
        {
            return Find(name, inventory);
        }

        public static List<string> GetInventoryNames()
        {
            return inventory.Select(item => item.Name).ToList();
        }

        public static void Where(string name)
        {
            var item = Item.Find(name, items);
            if (item == null)
            {
                Display.WriteLine($"[no item by name {name}]");
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

        public Item(List<ScriptLine> lines) : base(lines)
        {
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
                        Use();
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

        public void WriteDesc()
        {
            Display.Write($" [{stateMachine}]");
        }

        protected override void BindScriptKeys()
        {
            scriptKeys = new List<ScriptCommand>()
            {
                new ScriptCommand("ex", 1, args => {
                    return ScriptCommand.SetOnce(ref examineDesc, args[0], "examine desc");
                }),
                new ScriptCommand("item", 1, args => {
                    Problem problem = null;
                    if(Name != null)
                        problem =  new OverwriteWarning("name");
                    Name = args[0];
                    return problem;
                }),
                new ScriptCommand("loc", 1, args => {
                    var room = Room.Find(args[0]);
                    if(room == null)
                        return new Problem(ProblemType.Error, $"Item `{Name}` location `{args[0]}` does not exist.");
                    room.AddItem(this);
                    return null;
                }),
                new ScriptCommand("take", 0, args => {
                    if(args.Count == 0) {
                        takeDesc = DefaultTakeDesc;
                        return null;
                    }
                    return ScriptCommand.SetOnce(ref takeDesc, args[0], "take desc");
                }),
            };
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