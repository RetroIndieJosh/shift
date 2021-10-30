# SHIFT / Survival Horror Interactive Fiction Tool

(c)2021 Joshua McLean aka [Retro Indie Josh](https://retroindiejosh.itch.io)

Released under a permissive GPL v3 license. See LICENSE.txt for more information.

A simple interactive fiction system modeled after behaviors in 3D survival horror games to serve as an outlining and prototyping tool for that style of game.

Currently under development, so the following description may include unimplemented or incomplete features.

Commands are simplified to movement, `LOOK`, `EXAMINE`, and `USE`:

- `EXAMINE` or `USE` an item in the scene
    - If the item is takeable, prompt whether the user wants to take it (ammo, medkit, etc.)
    - Otherwise if the item is usable, prompt for whether the user wants to use it (light switches, wheels, etc.)
    - Otherwise describe the item and set it as the current target 
- `EXAMINE` an item in the inventory
    - Prints a description of the item including its state
- `USE` an item in the inventory
    - If the item requires a target:
        - If there is no target, prints a "can't use" message
        - Otherwise uses the item on the target
    - Otherwise, uses the item
- `LOOK` describes the room, including what item the user is targeting
- The user can also type the name of an item
    - Out of inventory, this will take a takeable or otherwise examine it and set it as the current target
    - In inventory, this will ask if the user wants to `EXAMINE` or `USE` the item
- Targeting is lost upon movement to a new room
- Traditional interactive fiction movement (cardinal and ordinal directions plus up and down)

Additional meta commands include CREDITS, HELP, and QUIT.