# SHIFT To Do

## Current

## Critical / Major

- BUG: Display.Flush() is inconsistent on final newline when flushing; sometimes too many, sometimes missing
- Game: intro text
- Game: use (inventory)
- Game: help per command
    str->str dict?
- Items: aliases
- Items: plurals
- Items: info
- Items: quantity (for ammo etc. gets combined with same-named, default 1 - this works because items never exist in multiple in a single area, and are combined on pickup)
- Text: messages based on state
        - `[if item name = state]response[else if item name = other state]other response[else]yet another response[end]`
- Text: messages based on location
        - `[if in room name]response[else if in other room name]other response[else]yet another response[end]`
- Items: combine (inventory)
- Meta: add stuff from [here](https://github.com/RetroIndieJosh/shift/community)
- Parser: implement comma handling as per spec
- Parser: more test games to test parser errors and features
- Parser: exits
- Parser: items (name, desc, use desc, take desc, states)
- Parser: warn for empty or missing room descriptions
- Parser: warn for empty or missing item descriptions
- Rooms: doors (closed, broken, locked)

## Quick / Minor

- Game: complain and fail if extra args to input (i.e. `get flashlight lamp dresser` or `flashlight lamp`)
- Display: move verbose mode (in ShiftParser) here
- Display: implement Log/Warn/Error here (to allow runtime warnings/errors and logging)
- Display: log to file
- Game: TRANCSRIPT (enable log to file)
- Items: default to "nothing interesting" message when no description

## Optional for Minimum Viable

- Display: customizable prompt (at least a few options)*
- Display: scrollable buffer (pgup/pgdown)*
- Display: ctrl+backspace to clear words*
- Display: rename to something that suggests it's also handling input*
- Display: pretty presentation*
        distinguish between output and input regions
        "title bar" (can we change console name? at the very least, draw as first line)
        mouse driven menus?
        clickable item names?
        ...or are those better not in the console?
        could make web version with ASP interface (maybe overkill for prototyping)
- Game: CLEAR*
- Game: DEBUG (extra messages - or as arg to shift)*
- Game: SAVE/LOAD*
- more LINQ*
- Items: disallow item names that match loaded commands or aliases*
- Parser: variables?*
- Parser: flexible indentation*
- Parser: command scripting*
- Rooms: autokeys?*
