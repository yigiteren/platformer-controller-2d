# Yern's Platformer Controller 2D

A fully customizable platformer controller for Unity. A platformer controller that doesn't feel like you are floating in the air, and feels good to play instead.

## Setup

There is a prefab that you can just drag and drop into your project. If you would like to create your character from scratch, follow the steps below.

- Attach the **PlatformController2D.cs** script to your character. It will automatically setup the **Rigidbody2D** and **BoxCollider**.
- Setup the ground layer masks at sections **Ground Check Settings**, **Ceiling Check Settings** and **Horizontal Check Settings**.
- If you are using one way colliders, make sure to add a tag to them and add their tag to the **Ceiling Check Ignore Tags** list as well, otherwise your one way collision won't work.
- Finally not necessary, but for more predictable results, I suggest you to add a Physics Material to both player and ground and set the friction to 0. You can use **No Friction Material** provided in the example assets.

You are done! Start the game and enjoy your 2D controller! :)

## License

Attribution is not required, but is welcomed. You are **allowed** to use this controller in your commercial/non-commercial projects. You are **not allowed** to redistribute/resell the controller.
