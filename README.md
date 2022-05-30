# AsteroidsSandbox
 The Sanbox Developer Test

 Project restrictions
 This project was developed using a pure ECS approach, without any monobehaviours or conventional GameObjects on runtime.

 Features
 The game is inspired by the classic asteroids with some modifications, the scales and looks have been modified and power ups have been added. Besides these modifications the basic concept is the same, the player has to avoid colliding with enemy bullets and asteroids to survive, and destroy enemies and asteroids by shooting them to get the maximum score possible.

 The player has three lives and when they run out the score resets to start a new game.

 The power ups implemented are a shield and faster bullets. When picked up they affect the player for a certain time and when this time runs out the power up is removed.

 The shield power up makes the player indestructible and destroys asteroids on contact.
 The faster bullets power up allows the player to shoot more bullets per second.

 Controls
 We have rotation controls with left and right arrows key, forward thrust with up arrow key, teleport with down arrow key and shoot bullets with space key.

 Score System
 The score increments with different values, according to the next table:

 Event                       Score Increment
 Player Destroyed Asteroid    +5
 Player Destroyed Enemy       +30
 Enemy Crashes with Asteroid  +65
 Enemy Destroyed Asteroid     +5



 Future Improvements
 Due to this project being a prototype there are necessary improvements to make it a Release Ready Game, these include: animation system, modify player sprite when a power up is picked, menu, when teleporting and spawning player check that there aren't asteroids at that location, save all time maximum score, controller and touch support, multiplayer.
