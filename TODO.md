# SHIFT To Do

<!-- START doctoc generated TOC please keep comment here to allow auto update -->

<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

- [Current](#current)
  - [Variables](#variables)
- [Reserved Words](#reserved-words)
  - [Game](#game)
  - [Room](#room)
  - [Item](#item)
- [Item Type Block](#item-type-block)
  - [Use](#use)
  - [Combine](#combine)
- [Critical / Major](#critical--major)
- [Quick / Minor](#quick--minor)
- [Cleanup](#cleanup)
- [Optional for Minimum Viable](#optional-for-minimum-viable)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Current

- clean up this file and migrate to freedcamp
  
- variables checked as keywords (PLAYER, CURROOM, etc.) on naming
  - store a list of built-in variable names for lookup
- update Item, Game, and Room to use ScriptFields and ScriptReferences where appropriate
  - check that ScriptReference works correctly
- better error system that doesn't rely on returns (some sort of error manager with statics methods?)

### Variables

(test these in `builtinvars.shift`)

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

- parser/compiler object constructors should read only name
  - then parse vars
  - then parse everything else
  - this allows cross-referencing for object references and variables
- --compile option to build `.shift` file into `.sb` binary file (string mangling + serilization)
  - still allow loading `.shift` scripts without compile step for testing (`.sb` is for release)
- --verify option to check if valid `.shift` script
- `ScriptReferenceList` for state lists
- `ItemState` as a `ScriptableObject` so it can use `ScriptReference<ItemState>` to parse
- automatic testing that runs through all scripts in `game/test` to ensure there are no parsing/scripting errors
- disambiguate between held quantity and in-room quantity
  - i.e. player carries 7 bullets and room contains 4 bullets, what does "bullet" refer to? we must ask: "held bullets or  bullets in room?"
    - or by context: 
        examine gives the same message for each
        use/combine has the same result whether held or not
        take refers to bullets on the ground (all of them? probably)
  - what does this do to find? each grouping of bullets must have a unique identifier, perhaps a hidden ID in the name
- BUG: Display.Flush() might miss final newline when text ends on blank line
- BUG: Display.Flush() counts incorrectly on write with multiple `\n`s (i.e. "three\nlines\nlong" counts as one line)
- Game: use (inventory)
- Items: plurals
- Text: messages based on state
        - `[if item name = state]response[else if item name = other state]other response[else]yet another response[end]`
- Text: messages based on location
        - `[if in room name]response[else if in other room name]other response[else]yet another response[end]`
- Text: messages based on held items
        - `[if has item name]response[else if has other item name]other response[else]yet another response[end]`
- Items: combine (inventory)

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
- map generation (ASCII) based on paths

## Ideas

Line endings to allow multiline text (semicolon would require escaping `\;` or could use different symbol like `)

```
desc/This is a really long room description.
    It goes on forever.
    But then it ends.;
```

Or use backticks for multiline (where backticks are optional for single lines):

```
desc/`This is a really long room description.
    It goes on forever.
    But then it ends.`
```

would print as (notice the indentation on each subsequent line collapsed to a single space)

`This is a really long room description.  It goes on forever. But then it ends.`
