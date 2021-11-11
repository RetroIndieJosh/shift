# SHIFT To Do

## Current

- warn on `room` block empty `desc`

### Variables

- `PLAYER` the player (treated as an item for use, combine, etc.)
- `[item type].FLOOR` count of the given `item name` in the current room
- `[item type].HELD` count of the given `item name` carried by the player
- `[room name].ITEMCOUNT` the number of items in `room name`

## Reserved Words

The following cannot be used as names. Remember names are *not* case sensitive.

- `HELD` is a pseudo-room representing the player's inventory which can be used in `item` block `loc` commands
- `PLAYER` is a psedo-item representing the player as an item

### Game

- `itemtype/[name]` define an item type (an item that can be manipulated in quantity)
- `use/[item]` or `use/[item]/[target]` define a USE command for the given item (optionally with the given target)
- `playerdesc/[desc]` the examine description for the special `me` object (referring to the player)
- `var/[name]/[#]` define a global integer variable with default value `#`

### Room

- `exit [direction] / [type] / [room] / [move description]` define an exit
    - create a reciprocal exit with same description unless already defined (can also be later overwritten by script)
    - `move description`: if blank, use the default description; otherwise this is written when moving in that direction
    - see Directions and Exits in spec
- `items/[item type]/[number]` place `number` of `item type` in this room
    - `item type` must be defined before this room
    - if `number` omitted, place one of the item type in this room
    - multiple ddeclarations with the same type in a room block is an error
- `roomvar/[name]/[#]` create a variable with the given `name` and an initial value of `#`
    - `name` cannot contain the period character
    - attached to the room through name mangling (`room name.var name`)
    - only integer variables supported
    - move description: if blank, use the default description; otherwise this is written when moving in that direction

### Item

- `alias/[name]` define an alias for the item
    - all aliases and item names must be unique
- `give/[item]` define a special GET/TAKE that gives `item` instead of this item
    - uses `take` description from `item`
    - flags `item` as canTake if not already
- `key/[room] / [direction] / [description]` indicate item can unlock exit in `room` going `direction`
    - if description is provided, print when walking through the door carrying this item (unlocking, opening, and stepping through the door)
- special statement `loc/HELD` starts this item in player's inventory
- `statemach/[name]/[state1]/[state2]/...` define a new state machine for the item
    - by default, state is the first listed state
    - all states must be unique per item
    - states must be all lowercase
- `itemvar/[name]/[#]` create a variable with the given `name` and an initial value of `#`
    - `name` cannot contain the period character
    - attached to the item through name mangling (`item name.var name`)
    - only integer variables supported

## Item Type Block

Identical to `item` except:

- `state` and `loc` are invalid
- `take` is defined automatically (default null)
- `use` applies to the entire collection as a single item
- new key `plural` define name when referring to quantity > 1 of this itemtype 
- all instances combine in inventory, i.e. taking 5 bullets when carrying 7 results in one item `bullets (12)`
- all description entries apply to any collection of the `itemtype`

### Use 

`use/[item]/[target]` (target optional)

- `add/[var]/[#]` add `#` to variable `var`
- `dec/[var]` shortcut for `sub [var] 1`
- `destroy` destroy the used item (remove from the game)
- `destroytarget` as `destroy` but on `target`
    - error if no `target` defined
- `endgame/[message]` end the game with the given message
- `give/[item]` place the named item in the player's inventory
- `inc/[var]` shortcut for `add [var] 1`
- `ifnot/[var]/[value]/[message]` or `ifnot/[state]/[message]` print the given message instead of executing the USE command if the given condition is false
- `say/[message]` print the given message
    - multiple messages will be printed in sequence, separated by a newline
- `set/[var]/[#]` set the `var` to the value `#`
- `sub/[var]/[#]` subtract `#` from variable `var`
- `state/[state]` set the item state to `state`
    - use multiple times for different state machines
    - two or more `state` commands from the same state machine is an error
- `statetarget/[state]` as `state` but on `target`
    - error if no `target` defined

### Combine 

- `combinedesc/[item] / [description]` description for combining `item` with the other item
    - if only one defined, applies to reciprocal combination
    - if no `combinedesc` defined, fall back to default description
- `replace/[item]` the item replacing the combination
    - other items are removed from the game
    - this can be `item1` or `item2` which will be returned to the player after combination
    - if no result defined, combining simply destroys both items

If `item1` or `item2` is an `itemtype`, this applies to COMBINE with any item of that type and destroys one from the collection for each COMBINE command.

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

## Quick / Minor

- fail message on taking item which can't be taken
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
