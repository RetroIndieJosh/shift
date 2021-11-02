using System;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    public class Room : Entity
    {
        static List<Room> rooms = new List<Room>();

        public enum Direction
        {
            East, North, Northeast, Northwest, South, Southeast, Southwest,
            West, Down, Up, Count
        }

        private Room[] exits = new Room[(int)Direction.Count];
        private List<Item> items = new List<Item>();

        public static Room Find(string name)
        {
            var matches = rooms.Where(room => room.Name == name).ToList();
            if (matches.Count == 0)
                return null;

            return matches[0];
        }

        public Room(string name, string desc) : base(name, desc)
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

        // TODO ambiguity handling (multiple matching items)
        public Item FindItem(string[] args)
        {
            return Item.Find(args, items);
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
            Display.Write("--= ");
            WriteName();
            Display.WriteLine(" =--");
            Display.WriteLine($"[{items.Count} items]");
            WriteDesc();
            Display.WriteLine();

            if (items.Count == 0)
                return;

            var itemStr = string.Join(", ", items);
            Display.WriteLine("You also see {0}.", itemStr);
        }
    }
}
