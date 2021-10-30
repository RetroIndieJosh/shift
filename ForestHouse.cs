namespace csif
{
    public class ForestHouse : Game
    {
        public ForestHouse() : base("The Forest House", "Joshua McLean")
        {
        }

        protected override void LoadRooms()
        {
            var bedroom = new Room("Bedroom", "It's a bedroom.");
            bedroom.AddItems(new Item[] {
                new Item("lamp", "Doesn't seem to turn on."),
                new Item("dresser", "Contains clothes."),
                new Item("flashlight",
                    "A standard battery-powered portable light.",
                    "",
                    "You toggle the flashlight."
                ),
            });

            var hall = new Room("Hall", "It's a hall.");
            bedroom.SetExit(Room.Direction.West, hall);

            CurRoom = bedroom;
        }
    }
}