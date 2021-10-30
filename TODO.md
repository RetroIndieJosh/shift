- use (inventory)
        target item (null for self)
        destroy self?
        destroy target?
        self state set (null for no change)
        target state set (null for no change)
        unlock target (door, null for no change)
- use messages based on state
        "[if state a]state a response[else if state b]state b response[else]other states response[end]"
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
- command history with up
- command completion with tab
- CLEAR
- SAVE/LOAD
- TRANCSRIPT
- DEBUG (extra messages - or as arg to shift)