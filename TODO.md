# SHIFT To Do

For a minimum viable prototype

Asterisk denotes items that may be delayed for minimum viable

## Display

- customizable prompt (at least a few options)*
- scrollable buffer (pgup/pgdown)*
- ctrl+backspace to clear words*
- rename to something that suggests it's also handling input*
- pretty presentation*
        distinguish between output and input regions
        "title bar" (can we change console name? at the very least, draw as first line)
        mouse driven menus?
        clickable item names?
        ...or are those better not in the console?
        could make web version with ASP interface (maybe overkill for prototyping)

## Game

- CLEAR*
- DEBUG (extra messages - or as arg to shift)*
- SAVE/LOAD*
- TRANCSRIPT*
- complain and fail if extra args to input (i.e. `get flashlight lamp dresser` or `flashlight lamp`)
- intro text
- use (inventory)
- help per command
    str->str dict?

## General

- more LINQ*

## Items

- aliases
- default to "nothing interesting" message when no description
- disallow item names that match loaded commands or aliases*
- plurals
- info
        quantity (for ammo etc. gets combined with same-named, default 1 - this works because items never exist in multiple in a single area, and are combined on pickup)
        target item (null for no target, basic use)
        destroy self (default false)
        destroy target (default false)
        self state set (null for no change)
        target state set (null for no change)
        unlock target (door, null for no change)
- use messages based on state
        "[if state A]state A response[else if state B]state B response[else]other response[end]"
- combine (inventory)
        combine target
        new item created
        (auto destroy both, place new item in inventory)

## Parser

- command scripting*
- comments (start with // - multiline?)*
- documentation (keywords, take and use empty means "use default")
- exits
- items (name, desc, use desc, take desc, states)
- require room description in .shift files

## Rooms

- autokeys?*
- doors (closed, broken, locked)