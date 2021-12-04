using System;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    public class Room : ScriptedEntity<Room>
    {
        static List<Room> rooms = new List<Room>();

        public enum Direction
        {
            East, North, Northeast, Northwest, South, Southeast, Southwest,
            West, Down, Up, Count
        }

        public string Desc => desc.Value;

        #region Script Fields
        private readonly ScriptField<string> desc = new("desc", 0);
        #endregion

        private readonly Room[] exits = new Room[(int)Direction.Count];
        private readonly List<Item> items = new();

        public Room(List<ScriptLine> lines) : base(lines, "room")
        {
            rooms.Add(this);
        }

        public void AddItem(Item item)
        {
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
            if (exits[dirint] is not null)
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
            if (!items.Remove(item))
                throw new Exception($"Failed to remove item {item} (unknown error)");
        }

        public void WriteAll()
        {
            Display.WriteLine($"--= {Name} =--");
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
                desc,
                new ScriptCommand("exit", 1, args => CreateExit(args)),
                new ScriptCommand("start", 0, args => {
                    if(ShiftParser.StartRoom is not null)
                        return new Problem(ProblemType.Warning, $"Multiple start rooms. Using last defined ({Name}).");
                    ShiftParser.StartRoom = this;
                    return null;
                }),
            };

            base.BindScriptKeys();
        }

        private static Problem CreateExit(List<string> args)
        {
            if (args is null)
                throw new ArgumentNullException(nameof(args));

            return new Problem(ProblemType.Warning, "Key `exit` not yet implemented");
        }
    }
}
