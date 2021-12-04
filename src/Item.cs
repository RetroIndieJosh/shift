using System;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    public class Item : ScriptedEntity<Item>
    {
        #region Default Strings
        #endregion

        public static Item CurTarget { get; private set; } = null;

        private static readonly List<Item> inventory = new();

        public Room Location
        {
            get => curLocation;
            set
            {
                if (curLocation is not null)
                    curLocation.RemoveItem(this);
                curLocation = value;
                if(curLocation is not null)
                    curLocation.AddItem(this);
            }
        }

        private Room curLocation = null;

        #region Script Fields and References
        private readonly ScriptReference<Room> startLocation = new("loc", 1);
        private readonly ScriptField<string> examineDesc = new("ex", 1);
        private readonly ScriptField<string> takeDesc = new("take", 0);
        #endregion
        private readonly ItemStateMachine stateMachine;

        private bool CanTake { get => takeDesc.Value is not null; }
        // TODO usable condition
        private bool CanUse { get => false; }
        private bool isCarried = false;

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
            var item = Find(name);
            if (item is null)
            {
                Display.WriteLine($"[no item by name {name}]");
                return;
            }

            string where;
            if (item.isCarried)
                where = "inventory";
            else if (item.Location is null)
                where = "limbo";
            else
                where = item.Location.ToString();

            Display.WriteLine($"[{item} is in {where}]");
        }

        public static void WriteInventory()
        {
            if (inventory.Count == 0)
            {
                Display.WriteLine("You are carrying nothing.");
                return;
            }

            Display.WriteLine("You are carrying:");
            inventory.ForEach(item => Display.WriteLine($"\t{item}"));
        }

        public Item(List<ScriptLine> lines) : base(lines, "item")
        {
            Location = startLocation.Value;
        }

        public void AddState(string[] stateNames, int defaultStateIndex = 0)
        {
            stateMachine.AddState(stateNames, defaultStateIndex);
        }

        public void Target()
        {
            if (isCarried)
            {
                // TODO remove use option if cannot be used
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
                        WriteExamine();
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

            WriteExamine();
            Display.WriteLine();

            if (!isCarried)
                CurTarget = this;
        }

        public void WriteExamine()
        {
            Display.Write($"{examineDesc.Value} [{stateMachine}]");
        }

        protected override void BindScriptKeys()
        {
            scriptKeys = new List<ScriptCommand>()
            {
                examineDesc,
                startLocation,
                takeDesc
            };

            base.BindScriptKeys();
        }

        private void Take()
        {
            Location = null;
            inventory.Add(this);
            isCarried = true;
            if (takeDesc is null)
                Display.WriteLine(Properties.Resources.DefaultTakeDesc, this);
            else
                Display.WriteLine(takeDesc.Value, this);
            Display.WriteLine("[taken]");
            return;
        }

        private void Use()
        {
            WriteExamine();
            Display.WriteLine();
        }
    }
}
