# SHIFT Scripting Specification

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

- [Basics](#basics)
- [Names and References](#names-and-references)
- [Text](#text)
- [Commands and Arguments](#commands-and-arguments)
  - [Standards](#standards)
  - [Blocks and Indentation](#blocks-and-indentation)
  - [Built In Variables](#built-in-variables)
  - [Comments](#comments)
- [Game Block](#game-block)
- [Room Block](#room-block)
  - [Directions](#directions)
  - [Exits](#exits)
- [Item Block](#item-block)
- [Item Type Block](#item-type-block)
- [Use Block](#use-block)
- [Combine Block](#combine-block)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Basics

A SHIFT script must be in a file ending with `.shift` and must contain at least one room with exactly one room flagged as the start room.

The minimum viable SHIFT script is as follows:

```
room a
    start
```

This creates a descriptionless room called `a` in a game with no title, author, intro, or items.

## Names and References

Commands, items, item types, rooms, and variables are uniquely named objects. Names may also not clash with any commands in the parser. Spaces and underscores are interchangeable in object names, so `blue book` and `blue_book` refer to the same object.

State machines and states are uniquely named within their containing object. For instance, in an item called `test`, a state machine `status` with states `passed` and `failed` would define `test.status`, `test.passed`, and `test.failed`. No other states or state machines in `test` can use the named `status`, `passed`, or `failed`.

Names are not case sensitive, but will be printed the way they are defined. So an `item Blue Book` will print as `Blue Book` but can be referred to as `blue book` or `Blue book` in the script.

References can only be made to previously defined objects. For instance, if an item is placed in a room called `Kitchen`, then `room Kitchen` must exist in the script *before* the item definition.

## Text

No symbols denote text. Rather, text is recognized on context.

Text may include the following escape sequences:

- `\n` for newline (text cannot include literal newlines, as a newline ends a command line)
- `\p` for pause/page, which stops with a `[Press down]` message and waits for user input to continue
- `\t` for tab (width determined by the output console)

Underscores are printed in the game as spaces. To write a literal underscore, write two consecutive underscores:

- `hello_world` will render as `hello world`
- `hello__world` will render as `hello_world`

Spaces in identifiers (for rooms, items, and variables) convert to underscores at runtime. So, a variable called `bullet count` in the script will be redefined as `bullet_count`. Thus, you cannot have both a `bullet count` and `bullet_count` as they will become the same in the resulting game.

Variables can be placed in text using square brackets.

Script 1:
```
room/Kitchen
    desc/You are in [CURROOM].
```

Output 1: `You are in Kitchen.`

Script 2:
```
item/Light
    desc "The light is [Light.toggled]."
    statemach/toggled/on/off
```

Output 2: `The light is on.`

These replacements are made at runtime, so an item or state machine may be referenced before it is defined in the script.

## Commands and Arguments

Each line consists of a command followed by an optional value that runs until the end of the line. Keywords may not contain spaces, but arguments may.

A line with no arguments:

```
command
```

A line with one argument uses a slash to separate the argument:

```
command / [value]
```

A line with multiple arguments separates arguments with a forward slash:

```
command / [value 1] / [value 2] / [value 3]
```

Leading and trailing whitespace is ignored. So the above could be written:

```
command  /  [value 1]/[value 2]   /     [value 3]
```

More technically, any non-comment line matchs the regex:

```regex
\s*[^\/#]+\s*(\s*\/[^\/#]*\s*)*
```

### Standards

Current (experimental) standard is to put one space around a slash when the right hand side is a list, but otherwise put no spaces around slashes.

```
item/No Space
    ex/No space with one arg.
    statemach/ space before first state / and around slashes / between other states
```

### Blocks and Indentation

Two levels are indicated using indentation:

- game block
    - combine block
    - OR item block
    - OR itemtype block
    - OR room block
    - OR use block 

Blocks begin with special commands and end when the whitespace returns to its previous position.

An indentation is four spaces. (In future, this will be more flexible.)

```
game block
    room block
        item block 1
        item block 2
    room block
        item block
            property
        item block
    combine block
        property
    use on block
        property
```

### Built In Variables 

The following variables exist in all scripts and their names cannot be used. Remember names are *not* case sensitive but by convention these are written in SCREAMINGCAPS. These names are designed to be unlikely for the script writer to use.

- `CURROOM` name of the current room
- `HELDCOUNT` number of items currently held
- `HELD` a pseudo-room representing the player's inventory which can be used in `item` block `loc` commands
- `PLAYER` a psedo-item representing the player as an item
- `TARGITEM` name of the currently targeted item or "nothing" if null
- `[item name].ONFLOOR` count of the given `item name` in the current room
- `[item name].NUMHELD` count of the given `item name` carried by the player
- `[room name].ITEMCOUNT` the number of items in `room name`

### Comments

Comments begin with `#` and run to the end of the line. These can be placed after commands.

```python
# This is a full line comment
room Kitchen # this is a comment about the kitchen
```

Multiline comments are not supported.

## Game Block

The script's top level defines game properties:

- `author/[name]` the game author
- `intro/[text]` text to display when the game begins
- `item/[name]` start an item block with given `name`
- `itemtype [name]` define an item type (an item that can be manipulated in quantity)
- `room/[name]` start a room block with given `name`
- `title/[name]` the game title
- `use/[item]` or `use/[item]/[target]` define a USE command for the given item (optionally with the given target)
- `var/[name]/[value]` define a global integer variable with initial value `value`

## Room Block

A room block started by `room [name]` from the game block can have the following properties:

- `desc/[description]` description for the room
    - empty `desc` is meaningless but not an error
- `exit/[direction]/[type]/[room]/[move description]` define an exit
    - create a reciprocal exit with same description unless already defined (can also be later overwritten by script)
    - `move description`: if blank, use the default description; otherwise this is written when moving in that direction
    - see Directions and Exits below for more information
- `items/[item type]/[number]` place `number` of `item type` in this room
    - `item type` must be defined before this room
    - if `number` omitted, place one of the item type in this room
    - multiple ddeclarations with the same type in a room block is an error
- `roomvar/[name]/[#]` create a variable with the given `name` and an initial value of `#`
    - `name` cannot contain the period character
    - attached to the room through name mangling (`room name.var name`)
    - only integer variables supported
- `start` game starts in this room
    - multiple `start` definitions is an error

### Directions

A direction can be any cardinal or ordinal direction, up, or down. Directions can be abbreviated as follows:

- down / d
- east / e
- north / n
- northeast / ne
- northwest / nw
- south / s
- southeast / se
- southwest / sw
- up / u
- west / w

### Exits

An exit can be:

- **free**: passage from one area to another, such as one side of a field to the other, or to simulate an open doorway
- **closed**: a door blocks the exit (default message suggests opening, walking through, and closing the door behind)
- **locked**: a door blocks the exit and requires a key
- **broken**: a dead end that cannot be opened
    - an exit definition may omit `room` when broken: `exit west / broken /  / The lock is broken. You can't open it.`

When defined in an `exit`, the `description` is used as follows:

- **free** or **closed**: print when player goes through exit
- **locked**: print when player moves in exit direction without key
    - see Item Block -> `key` for the unlock description
- **broken**: print when player moves in exit direction

## Item Block

Started by `item/[name]`. A special item `item/PLAYER` defines player properties. An item block can include the following properties:

- `alias/[name]` define an alias for the item
    - all aliases and item names must be unique
    - multiple aliases are allowed, one per `alias` command
- `ex/[description]` define the EXAMINE command description for the item
- `give/[item]` define a special GET/TAKE that gives `item` instead of this item
    - uses `take` description from `item`
    - flags `item` as canTake if not already
- `key/[room]/[direction]/[description]` indicate item can unlock exit in `room` going `direction`
    - if description is provided, print when walking through the door carrying this item (unlocking, opening, and stepping through the door)
- `loc/[room]` specify the item's location
    - if this entry is missing, the item starts "out of world"
    - special statement `loc/INVENTORY` starts the item in the player's inventory
- `statemach/[name]/[state1]/[state2]/...` define a new state machine for the item
    - by default, state is the first listed state
    - all states must be unique per item
- `take/[description]` defines the GET/TAKE command 
    - flag this item as canTake
- `itemvar/[name]/[value]` create a variable with the given `name` and an initial value of `#`
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

## Use Block

Define what happens when the player USEs an item, started with `use/[item]/[target]`.

- `add/[var]/[value]` add `value` to variable `var`
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
- `set/[var]/[value]` set the `var` to `value`
- `sub/[var]/[value]` subtract `value` from variable `var`
- `state/[state]` set the item state to `state`
    - use multiple times for different state machines
    - two or more `state` commands from the same state machine is an error
- `statetarget/[state]` as `state` but on `target`
    - error if no `target` defined

## Combine Block

Started in the game block with `combine [item1] / [item2]` to define a COMBINE command for one item on another:

- `combinedesc/[item]/[description]` description for combining `item` with the other item
    - if only one defined, applies to reciprocal combination
    - if no `combinedesc` defined, fall back to default description
- `replace/[item]` the item replacing the combination
    - other items are removed from the game
    - this can be `item1` or `item2` which will be returned to the player after combination
    - if no result defined, combining simply destroys both items

If `item1` or `item2` is an `itemtype`, this applies to COMBINE with any item of that type and destroys one from the collection for each COMBINE command.
