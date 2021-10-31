namespace shift
{
    public class ForestHouse : Game
    {
        public ForestHouse() : base("The Forest House", "Joshua McLean")
        {
        }

        protected override void LoadRooms()
        {
            var bedroom = new Room("Bedroom", "It's a bedroom.");

            var flashlight = new Item("flashlight",
                "A standard battery-powered portable light.",
                "",
                "You toggle the flashlight."
            );
            flashlight.AddState(new string[] { "off", "on" });

            bedroom.AddItems(new Item[] {
                new Item("lamp", "Doesn't seem to turn on."),
                new Item("dresser", "Contains clothes."),
                flashlight
            });

            var hall = new Room("Hall", "It's a hall.");
            bedroom.SetExit(Room.Direction.West, hall);

            CurRoom = bedroom;
        }
    }
}