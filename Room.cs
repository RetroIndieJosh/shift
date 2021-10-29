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

        public Room GetExit(Direction direction)
        {
            return exits[(int)direction];
        }

        public void SetExit(Direction direction, Room room)
        {
            int dirint = (int)direction;
            if (exits[dirint] != null)
                Console.WriteLine($"WARNING: overwriting {direction} exit from "
                    + $"{this} (orignally to {exits[dirint]} but now to {room})");
            exits[dirint] = room;
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
