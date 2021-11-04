# SHIFT Scripting Specification

## Basics

SHIFT is case sensitive. Unless otherwise specified, keywords are expected to be in lowercase.

Each line consists of a keyword followed by an optional value that runs until the end of the line. Keywords may not contain spaces, but arguments may.

A line with no arguments:

```
keyword
```

A line with one argument:

```
keyword [value]
```

A line with multiple arguments:

```
keyword [value 1], [value 2], [value 3]
```

Extended strings (i.e. descriptions) are always the final argument and may contain commas, i.e.:

```
exit west, closed, Living Room, A description of the living room, which can include commas, like this.
```

will split to:

- `exit` (keyword)
- `west` (arg 1)
- `closed` (arg 2)
- `Living Room` (arg 3)
- `A description of the living room, which can include commas, like this.` (description)

### Blocks and Indentation

Three levels are indicated using indentation:

- game block
    - room block
        - item block
        - OR combine block
        - OR use on block

Blocks begin with special keywords (`room`, `item`, `combine`, or `useon`) and end when the whitespace returns to its previous position.

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

### Comments

Comments begin with a `#` and run to the end of the line. These can be started after the beginning of the line.

```python
# This is a full line comment
room Kitchen # this is a comment about the kitchen
```

There are no multiline comments.

## Game Block

The top level of the script defines game properties:

- `author` the game author
- `room [name]` start a room block with given `name`
- `title` the game title

## Room Block

A room block started by `room [name]` from the game block can have the following properties:

- `desc [description]` description for the room
    - empty `desc` is meaningless but not an error
- `combine [item1], [item2]` start a combine block with specified items
- `exit [direction], [type], [room], [move description]` define an exit
    - create a reciprocal exit with same description unless already defined (can also be later overwritten by script)
    - see Directions and Exits below 
- `item [name]` start an item block with given `name`
- `start` indiciate the game should start in the current room
    - multiple `start` definitions is an error
- `useon [item1], [item2]` start a use on block with specified items

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
    - an exit definition may omit `room` when broken: `exit west, broken, , The lock is broken. You can't open it.`

When defined in an `exit`, the `description` is used as follows:

- **free** or **closed**: print when player goes through exit
- **locked**: print when player moves in exit direction without key
    - see Item Block -> `key` for the unlock description
- **broken**: print when player moves in exit direction

## Item Block

An item block started by `item [name]` from the game block can have the following properties:

- `ex [description]` defines the EXAMINE command description for the item
- `give [item]` defines a special GET/TAKE that gives `item` instead of this item
    - uses `take` description from `item`
    - flags `item` as canTake if not already
- `key [room], [direction], [description]` indicates the item can unlock the door in `room` going `direction`
    - if a description is provided, this is printed when walking through the door carrying this item (unlocking, opening, and stepping through the door)
- `state [state1], [state2], ...` defines a new state machine for the item
    - by default, the state of this machine is the first listed state
    - all states in an item must be unique
- `take [description]` defines the GET/TAKE command 
    - flags this item as canTake
- `use [new state], [description]` defines the USE command
    - flags this item as canUse
    - when used, the item state is changed to `new state` and `description` is printed

## Combine Block

A combine block started by `combine [item1], [item2]` defines a COMBINE command for one item on another:

- `combinedesc [item], [description]` description for combining `item` with the other item
    - if only one defined, applies to reciprocal combination
    - if no `combinedesc` defined, fall back to default description
- `result [item]` the item resulting from the combination
    - other items are destroyed (removed from the game)
    - if no result defined, combining simply destroys both items

## Use On Block

A use on block started by `useon [item0], [item1]` defines a USE command for one item ON another:

- `destroy [#]` flag item `#` to be destroyed on the USE ON action
- `state [#] [state]` set item `#` to given `state` on the USE ON action
- `usedesc [#] [description]` description for using `item#` on other item
    - `#` must be 0 or 1
    - if only one defined, applies to reciprocal combination
    - if no `usedesc` defined, fall back to default description