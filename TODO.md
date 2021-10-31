- autocomplete: continue searching from initial typed letters with multiple tab presses
- item info
        quantity (for ammo etc. gets combined with same-named, default 1)
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
- help (list commands - per command help?)
    command = action + help text + more?
- item aliases
- disallow item names that match loaded commands or aliases
- disallow identical item names
- intro text
- passages between rooms
- doors (closed, broken, locked)
- autokeys?
- room/item scripting
- command scripting
- command completion with tab
- CLEAR
- SAVE/LOAD
- TRANCSRIPT
- DEBUG (extra messages - or as arg to shift)