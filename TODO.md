- more linq
- item info
        quantity (for ammo etc. gets combined with same-named, default 1 - this works because items never exist in multiple in a single area, and are combined on pickup)
        target item (null for no target, basic use)
        destroy self (default false)
        destroy target (default false)
        self state set (null for no change)
        target state set (null for no change)
        unlock target (door, null for no change)
- use (inventory)
- use messages based on state
        "[if state A]state A response[else if state B]state B response[else]other response[end]"
- combine (inventory)
        combine target
        new item created
        (auto destroy both, place new item in inventory)
- help per command
    str->str dict?
- item aliases
- item name plurals
- multi word item names (disambiguation)
- disallow item names that match loaded commands or aliases
- intro text
- passages between rooms
- doors (closed, broken, locked)
- autokeys?
- room/item scripting
- command scripting
- CLEAR
- SAVE/LOAD
- TRANCSRIPT
- DEBUG (extra messages - or as arg to shift)
- pretty presentation
        distinguish between output and input regions
        "title bar" (can we change console name? at the very least, draw as first line)
        mouse driven menus?
        clickable item names?
        ...or are those better not in the console?
        could make web version with ASP interface (maybe overkill for prototyping)