# SHIFT To Do

## Current

- Parser: handle room created after item that references it (two passes?)

## Critical / Major

- disambiguate between held quantity and in-room quantity
    - i.e. player carries 7 bullets and room contains 4 bullets, what does "bullet" refer to? we must ask: "held bullets or inventory bullets?"
    - what does this do to find? each grouping of bullets must have a unique identifier, perhaps a hidden ID in the name
- BUG: Display.Flush() seems to be missing a final newline when the text ends on a blank line
- BUG: Display.Flush() doesn't count lines correctly when one write with multiple \n's (i.e. "three\nlines\nlong" counts as one)
- Game: use (inventory)
- Items: aliases
- Items: plurals
- Text: messages based on state
        - `[if item name = state]response[else if item name = other state]other response[else]yet another response[end]`
- Text: messages based on location
        - `[if in room name]response[else if in other room name]other response[else]yet another response[end]`
- Items: combine (inventory)
- Meta: add stuff from [here](https://github.com/RetroIndieJosh/shift/community)
- Parser: more test games to test parser errors and features
- Parser: Room `exit`
- Parser: Item `ex`
- Parser: Item `give`
- Parser: Item `take`
- Parser: Item `alias`
- Parser: Item `key`
- Parser: Item `state`
- Parser: Item `use`
- Parser: Game `itemtype`
- Parser: Room `item #` (dupes of `itemtype`)
- Parser: combine block
- Parser: use on block

- Parser: Item (name, desc, use desc, take desc, states)
- Rooms: doors (closed, broken, locked)

## Quick / Minor

- Game: help per command
    str->str dict?
- Parser: warn for empty or missing room descriptions
- Parser: warn for empty or missing item descriptions
- Parser: disallow item names matching commands or aliases
- Parser: disallow multiple items with the same name
- Parser: disallow multiple rooms with the same name
- Display: move verbose mode (in ShiftParser) here
- Display: implement Log/Warn/Error here (to allow runtime warnings/errors and logging)
- Display: log to file
- Game: TRANCSRIPT (enable log to file)
- Items: default to "nothing interesting" message when no description

## Cleanup

- Display: clean up code (alphabetize methods, split methods?, cleaner loops)
- remove repetition in Find between Room and Item
- should parser code be in its own namespace?

## Optional for Minimum Viable

- Script: include other script files to allow organization by region (`include [filename]` ?)
- Display: customizable prompt (at least a few options)
- Display: scrollable buffer (pgup/pgdown)
- Display: ctrl+backspace to clear words
- Display: rename to something that suggests it's also handling input
- Display: pretty presentation
        distinguish between output and input regions
        "title bar" (can we change console name? at the very least, draw as first line)
        mouse driven menus?
        clickable item names?
        ...or are those better not in the console?
        could make web version with ASP interface (maybe overkill for prototyping)
- Game: CLEAR
- Game: DEBUG (extra messages - or as arg to shift)
- Game: SAVE/LOAD
- more LINQ
- Parser: variables
- Parser: flexible indentation
- Parser: command scripting
- Rooms: autokeys?
