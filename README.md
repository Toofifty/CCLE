CCLE
====

C# Command-Line (Gaming) Engine
+ ATARI Breakout-like demo

## Features

* Map loading from plaintext
* Collision checking
* Per-character fore/back colours
* Easy sprite movement
* Sprite groups (rects) acting as one sprite
* Depth buffer
* Semi-automatic dirty frame cleaning
* Collision-movement blocking
* Custom window size / buffer size
* Char array sprites
* Sounds!

### Planned features

* Network/multiplayer integration
* Simple shape drawing / movement
* 3D applications

## Bug list

* Sprites do not always render fully
* Dirty pixels aren't always cleared
* Input is taken from Windows rather than the current frame
* DEMO: Input loop breaks for unknown reason
* DEMO: After a restart, the ball does not collide with the paddle
* DEMO: After a restart, the ball launches automatically and at the wrong angle

# Documentation

## Creating a game

To begin, create a new C# file in the namespace `CLEngine` and create your main class.

You can set basic information about your game through `Engine.SetGameSize(width, height)`, `Engine.SetWindowTitle(title)` (more coming soon).

To initiate the engine and renderer, use `Engine.Init(level, [fps])`. NOTE: If no level is provided, the renderer will not be created (useful if you want to extend the renderer). Level must be a _Pixel_ array, and fps is optional. You can load a new level from a `.cll` (plaintext) file with the method `Pixel.LoadTextLevel(file)`.

Create your main sprites in this class, and ensure they are declared earlier so they can be referenced in the main loop. Create a sprite with the constructor `new Sprite(charArray, consoleColorArray, rect, z-depth)` (has some overloads). If your sprite only consists of one colour or one symbol (repeated) make use of the `CharArray(length, char)` and `ConsoleColorArray(length, consoleColor)` methods to create a filled out array.

All sprites will be added to the sprite render queue, and will be rendered _once_. To provoke a new render of a sprite, set it's `Sprite.dirty` to `true`.

Collidable sprites can then be added to your sprite, through the `Sprite.AddCollider(Sprite)` method. **These colliders will prevent the sprite from moving if using the `Sprite.MoveCollision(dir, am)` method (which will return false).**

From here, use any means necessary to create a GameLoop and an InputLoop (recommended on different threads). For a _smooth_ input, with no pause after the initial press, use `Keyboard.IsKeyDown(Key.<key>)` this is useful for movement or physics based games. For a _non-smooth_ input, use `Console.ReadKey()` to return the key byte pressed. This is more practical for simple movement, like RPG or similar.

To move a sprite, use either `Sprite.Move(dir, am)` or `Sprite.MoveCollision(dir, am)`. The MoveCollision method will return false and will not move if blocked by one of it's colliders (sprites), whereas Move will not care. Alternatively, `Sprite.SetPosition(position)` may be used, in conjunction with `Sprite.GetPosition()`.
Sprites will not leave the boundaries of the screen (set with the game size) and will stop at these boundaries if moved too far.

Enjoy! Have fun making your CCLE games!