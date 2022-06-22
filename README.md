# SHIFT / Survival Horror Interactive Fiction Tool

(c)2022 Joshua McLean aka [Retro Indie Josh](https://retroindiejosh.itch.io)

Released under a permissive GPL v3 license. See LICENSE.txt for more information.

A simple interactive fiction system modeled after behaviors in 3D survival horror games to serve as an outlining and prototyping tool for that style of game.

Although traditional IF commands are supported (`LOOK`, `EXAMINE`, and `USE`, along with navigation), the preferred interaction with SHIFT games is through naming objects and directions only. Player input is context-sensitive and prompts with a menu when multiple options are available instead of requiring a specific verb for actions.

A developer specifies their game through a brief, readable, and writable scripting language also called SHIFT, with the `.shift` extension.

See the [SHIFT Scripting Specification](doc/ShiftSpec.md) for more detailed information on scripting.


## Why another IF system?

SHIFT applies Occam's Razor to interactive fiction. With a focus on site-based adventures and classic object-inventory puzzles, we want to minimize the required effort for the player and make it as close to a streamlined video game experience as possible. Rather than coming up with a verb, the user simply states the item they wish to interact with. In most cases, this interaction will be obvious in context. In the few instances where it's ambiguous, the player is presented a menu so they don't need to invent actions to take.

This system is *not* meant for more narrative- or conversation-driven interactive fiction, or beautiful generated prose. It goes back to the basics of game implementation, allowing its use not only as a parser text game engine, but as a way to prototype similar video games such as the titual survival horror genre, steeped in a historical connection to point and click adventure games.