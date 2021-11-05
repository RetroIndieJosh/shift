# SHIFT Scripting Specification

## Basics

SHIFT is case sensitive. Commands, item states, user-defined variable names must be lowercase. Built-in variables and special keywords are in SHOUTING_CAPS. Otherwise, any case is allowed.

A SHIFT script must be in a file ending with `.shift` and must contain at least one room with exactly one room flagged as the start room.

The minimum viable SHIFT script is as follows:

```
room a
    start
```

This creates an empty, descriptionless room called `a` in a game with no title or author.

## Text

No symbols denote text. Rather, text is recognized on context.

Text may include the following escape sequences:

- `\n` for newline (text cannot include literal newlines, as a newline ends a command line)
- `\p` for pause/page, which stops with a `[Press down]` message and waits for user input to continue
- `\t` for tab (width determined by the output console)

Underscores are printed as spaces. To write a literal underscore, write two consecutive underscores:

- `hello_world` will render as `hello world`
- `hello__world` will render as `hello_world`

Spaces in identifiers (for rooms, items, and variables) convert to underscores at runtime. So, a variable called `bullet count` in the script will be redefined as `bullet_count`. Thus, you cannot have both a `bullet count` and `bullet_count` as they will become the same in the resulting game.

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

Three levels are indicated using indentation:

- game block
    - room block
        - item block
            - use block (optional)
    - OR combine block
    - OR itemtype block

Blocks begin with special commands (`room`, `item`, `combine`, `itemtype`, or `use`) and end when the whitespace returns to its previous position.

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

### Built In Variables and Constants

- `CURROOM` name of the current room
- `HELDCOUNT` number of items currently held
- `TARGET` name of the currently targeted item or "nothing" if null
- `[room name].ITEMCOUNT` the number of items in `room name`
- `[item name].FLOOR` count of the given `item name` in the current room
- `[item name].HELD` count of the given `item name` carried by the player

### Comments

Comments begin with `#` and run to the end of the line. These can be placed after commands.

```python
# This is a full line comment
room Kitchen # this is a comment about the kitchen
```

Multiline comments are not supported.

## Game Block

The script's top level defines game properties:

- `author` the game author
- `intro` text to display when the game begins
- `itemtype [name]` defines an item type, identical to item block except:
    - `state` is invalid
    - `take` is defined automatically (default null)
    - `use` applies to the entire collection as a single item
    - new key `plural` define name when referring to quantity > 1 of this itemtype 
    - all instances combine in inventory, i.e. taking 5 bullets when carrying 7 results in one item `bullets (12)`
    - all description entries apply to any collection of the `itemtype`
- `room [name]` start a room block with given `name`
- `title` the game title
- `var [name] [#]` define a global integer variable with default value `#`

## Room Block

A room block started by `room [name]` from the game block can have the following properties:

- `desc [description]` description for the room
    - empty `desc` is meaningless but not an error
- `combine [item1] / [item2]` start a combine block with specified items
- `exit [direction] / [type] / [room] / [move description]` define an exit
    - create a reciprocal exit with same description unless already defined (can also be later overwritten by script)
    - see Directions and Exits below 
- `item [name]` start an item block with given `name`
    - an item name cannot start with a number, otherwise it is interpreted as below
    - name must be unique among all items including `itemtype` names
- `item [number] [item name]` place `number` of `item name` in the room
    - `item name` must be defined as an `itemtype` in the game block
    - multiple of these declarations in a single room block is an error
- `roomvar [name] [#]` create a variable with the given `name` and an initial value of `#`
    - `name` cannot contain the period character
    - attached to the room through name mangling (`room name.var name`)
    - only integer variables supported
- `start` game starts in this room
    - multiple `start` definitions is an error
- `useon [item1] / [item2]` start a use on block with specified items

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
    - move description: if blank, use the default description; otherwise this is written when moving in that direction

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

Started by `item [name]` in a room block. Can have the following properties:

- `alias [name]` define an alias for the item
    - all aliases and item names must be unique
- `ex [description]` define the EXAMINE command description for the item
- `give [item]` define a special GET/TAKE that gives `item` instead of this item
    - uses `take` description from `item`
    - flags `item` as canTake if not already
- `key [room] / [direction] / [description]` indicate item can unlock exit in `room` going `direction`
    - if description is provided, print when walking through the door carrying this item (unlocking, opening, and stepping through the door)
- `statemach [state1] / [state2] / ...` define a new state machine for the item
    - by default, state is the first listed state
    - all states must be unique per item
    - states must be all lowercase
- `take [description]` defines the GET/TAKE command 
    - flag this item as canTake
- `use` creates a use block to define the USE command on this item
    - flag this item as canUse
- `itemvar [name] [#]` create a variable with the given `name` and an initial value of `#`
    - `name` cannot contain the period character
    - attached to the item through name mangling (`item name.var name`)
    - only integer variables supported

## Use Block

Define what happens when the player USEs an item:

- `add [var] [#]` add `#` to variable `var`
- `dec [var]` shortcut for `sub [var] 1`
- `destroy` destroy the used item (remove from the game)
- `give [item]` place the named item in the player's inventory
- `inc [var]` shortcut for `add [var] 1`
- `say [message]` print the given message
    - multiple messages will be printed in sequence, separated by a newline
- `set [var] [#]` set the `var` to the value `#`
- `sub [var] [#]` subtract `#` from variable `var`
- `state [state]` set the item state to `state`
    - use multiple times for different state machines
    - two or more `state` commands from the same state machine is an error
- `target [item name]` require the named item to be currently targeted for a valid USE
- `targetdestroy` as `destroy` but on `target`
    - error if no `target` defined
- `targetstate [state]` as `state` but on `target`
    - error if no `target` defined

## Combine Block

Started in the game block with `combine [item1] / [item2]` to define a COMBINE command for one item on another:

- `combinedesc [item] / [description]` description for combining `item` with the other item
    - if only one defined, applies to reciprocal combination
    - if no `combinedesc` defined, fall back to default description
- `replace [item]` the item replacing the combination
    - other items are removed from the game
    - this can be `item1` or `item2` which will be returned to the player after combination
    - if no result defined, combining simply destroys both items

If `item1` or `item2` is an `itemtype`, this applies to COMBINE with any item of that type and destroys one from the collection for each COMBINE command.
