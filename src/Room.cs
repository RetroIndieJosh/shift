using System;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    public class Room : ScriptedEntity
    {
        static List<Room> rooms = new List<Room>();

        public enum Direction
        {
            East, North, Northeast, Northwest, South, Southeast, Southwest,
            West, Down, Up, Count
        }

        public string Desc
        {
            get => desc;
            private set => desc = value;
        }

        private string desc = null;

        private Room[] exits = new Room[(int)Direction.Count];
        private List<Item> items = new List<Item>();

        public static Room Find(string name)
        {
            var matches = rooms.Where(room => room.Matches(name)).ToList();
            if (matches.Count == 0)
                return null;

            return matches[0];
        }

        public Room(List<ScriptLine> lines) : base(lines)
        {
            rooms.Add(this);
        }

        public void AddItem(Item item)
        {
            item.Location = this;
            items.Add(item);
        }

        public void AddItems(Item[] items)
        {
            foreach (var item in items)
                AddItem(item);
        }

        public Item FindItem(string name)
        {
            return Item.Find(name, items);
        }

        public Room GetExit(Direction direction)
        {
            return exits[(int)direction];
        }

        public List<string> GetItemNames()
        {
            return items.Select(item => item.Name).ToList();
        }

        public Direction GetOppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    return Direction.West;
                case Direction.North:
                    return Direction.South;
                case Direction.South:
                    return Direction.North;
                case Direction.West:
                    return Direction.East;
                case Direction.Northeast:
                    return Direction.Southwest;
                case Direction.Northwest:
                    return Direction.Southeast;
                case Direction.Southeast:
                    return Direction.Northwest;
                case Direction.Southwest:
                    return Direction.Northeast;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Down;
                default:
                    return Direction.Count;
            }
        }

        public void SetExit(Direction direction, Room targetRoom, bool oneway = false)
        {
            int dirint = (int)direction;
            if (exits[dirint] != null)
                Display.WriteLine($"WARNING: overwriting {direction} exit from "
                    + $"{this} (orignally to {exits[dirint]} but now to {targetRoom})");
            exits[dirint] = targetRoom;

            if (oneway)
                return;

            var oppositeDirection = GetOppositeDirection(direction);
            targetRoom.SetExit(oppositeDirection, this, true);
        }

        public void RemoveItem(Item item)
        {
            if (!items.Contains(item))
                throw new Exception($"Tried to remove item {item} not in room {this}");
            items.Remove(item);
            item.Location = null;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public void WriteAll()
        {
            Display.WriteLine($"--= {DisplayName} =--");
            Display.WriteLine($"[{items.Count} items]");
            WriteDesc();
            Display.WriteLine();

            if (items.Count == 0)
                return;

            var itemStr = string.Join(", ", items);
            Display.WriteLine("You also see {0}.", itemStr);
        }

        private void WriteDesc()
        {
            Display.WriteLine(Desc);
        }

        protected override void BindScriptKeys()
        {
            scriptKeys = new List<ScriptCommand>()
            {
                new ScriptCommand("desc", 1, args => {
                    return ScriptCommand.SetOnce(ref desc, args[0], "author");
                }),
                new ScriptCommand("exit", 1, args => CreateExit(args)),
                new ScriptCommand("room", 1, args => {
                    Problem problem = null;
                    if(Name != null)
                        problem =  new OverwriteWarning("name");
                    Name = args[0];
                    return problem;
                }),
                new ScriptCommand("start", 0, args => {
                    if(ShiftParser.StartRoom != null)
                        return new Problem(ProblemType.Warning, "Multiple start rooms. Using last defined.");
                    ShiftParser.StartRoom = this;
                    return null;
                }),
            };
        }

        private Problem CreateExit(List<string> args)
        {
            return new Problem(ProblemType.Warning, "Key `exit` not yet implemented");
        }
    }
}
