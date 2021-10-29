using System;
using System.Collections.Generic;

namespace csif
{
    public class Room : Item
    {
        public enum Direction
        {
            East, North, Northeast, Northwest, South, Southeast, Southwest,
            West, Down, Up, Count
        }

        private Room[] exits = new Room[(int)Direction.Count];
        private List<Item> items = new List<Item>();

        public Room(string name, string desc) : base(name, desc)
        {
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public void AddItems(Item[] item)
        {
            this.items.AddRange(items);
        }

        // TODO ambiguity handling (multiple matching items)
        public Item FindItem(string[] args)
        {
            foreach (var item in items)
            {
                if (item.Matches(args))
                    return item;
            }
            return null;
        }

        public Room GetExit(Direction direction)
        {
            return exits[(int)direction];
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
                Console.WriteLine($"WARNING: overwriting {direction} exit from "
                    + $"{this} (orignally to {exits[dirint]} but now to {targetRoom})");
            exits[dirint] = targetRoom;

            if (oneway)
                return;

            var oppositeDirection = GetOppositeDirection(direction);
            targetRoom.SetExit(oppositeDirection, this, true);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public void WriteAll()
        {
            Console.Write("--= ");
            WriteName();
            Console.WriteLine(" =-- ");
            WriteDesc();
            Console.WriteLine();

            if (items.Count == 0)
                return;

            var itemStr = string.Join(", ", items);
            Console.Write("You also see {0}.", itemStr);
        }
    }
}
