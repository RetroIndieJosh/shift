# SHIFT Scripting Specification

## Basics

Each line consists of a keyword followed by an optional value that runs until the end of the line.

`keyword`

or

`keyword [value]`

Three levels are indicated using indentation:

- game block
    - room block
        - item block

## Game Block

The top level of the script defines game properties:

- `title` the game title
- `author` the game author
- `room [name]` starts a room block

## Room Block

A room block started by `room [name]` from the game block can have the following properties:

- `start` indiciates the current room is the start room
    - it is an error to define multiple start rooms