# Yern's Platformer Controller 2D

### Update: This archive is now archived, and will no longer receive any updates.

A fully customizable platformer controller for Unity. A platformer controller that doesn't feel like you are floating in the air, and feels good to play instead.

<p align="center">
    <img src="https://github.com/yigiteren/platformer-controller-2d/blob/master/Example%20Assets/Sprites/Thumbnail.png?" alt="Thumbnail">
</p>


## Setup

There is a prefab that you can just drag and drop into your project. If you would like to create your character from scratch, follow the steps below.

- Attach the **PlatformController2D.cs** script to your character. It will automatically setup the **Rigidbody2D** and **BoxCollider**.
- Setup the ground layer masks at sections **Ground Check Settings**, **Ceiling Check Settings** and **Horizontal Check Settings**.
- If you are using one way colliders, make sure to add a tag to them and add their tag to the **Ceiling Check Ignore Tags** list as well, otherwise your one way collision won't work.
- Finally not necessary, but for more predictable results, I suggest you to add a Physics Material to both player and ground and set the friction to 0. You can use **No Friction Material** provided in the example assets.

You are done! Start the game and enjoy your 2D controller! :)

## Features

Here is a list of features that I've implemented, and some that I haven't implemented yet. Feel free to contribute towards creation of any of these features.

### Implemented Features

- **Acceleration Based Movement**: All the movement is based on acceleration.
- **Variable Jump**: The jump ends as soon as the player releases the jump key.
- **Jump Buffering**: If player presses the jump key before landing, the jump still executes after the land.
- **Coyote Time**: The controller can jump after leaving grounded state for a short period of time.
- **Clamped Falling Speed**: Fall speed gets clamped to give player more control mid-fall.
- **One Way Collision Support**: Supports one way collision for platforms. (Can jump to platforms from below, but cannot fall down yet.)
- **Dash Support**: Fully customizable dash support.
- **Fixed Jump Height**: No matter the gravity, the player jumps as high as the _jumpHeight_ variable is.

### Missing Features

- **Bumped Head Correction**: Automatically fix the controllers position if it hits its head to close to a corner of the above platform.
- **Anti Gravity Apex Point**: Currently there is no gravity helper at apex point of the jump.
- **Step/Small Gap Detection**: There is no check for step detection yet.

## License

Attribution is not required, but is welcomed. You are **allowed** to use this controller in your commercial/non-commercial projects. You are **not allowed** to redistribute/resell the controller.
