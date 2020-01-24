# Point Blank

# Video Game Design Project
- Game PointBlank:. Created fully three dimensional game with real time character/game-object control, Sci-fi themed third person shooter game. Player navigates an abandoned futuristic city and fights off robots and ninjas using different types of weapons and bombs.
- Tools: Unity Game engine, Visual Studio, C#, VFX and Assets Pack

How to Play: 

Move the player around the screen using W,A,S,D or arrow keys. 
The goal is to fight the attacking enemies using your weapon. You can also navigate the virtual world by sprinting and jumping (climbing coming soon). 

Technical Components: 

-Player Movement: Sprint mode can be triggered by the "shift" key. The player can also jump further in this mode. 

-Weapons: Click the mouse button to fire the weapon. Use "c" to switch weapons. Each weapon is implemented using a separate script for effects and IK. Additionally, bullets have IK to ensure they move toward the target. 

-Player Airborne Ability: If the player falls from a high level, he will make harder impact with the ground and have a stiff landing, improving game feel. If the player sprints and falls, he will do a summersault. 

-AI: Humanoid enemies (cyborg ninjas) are implemented using AI. These characters patrol two waypoints and the patrol spawns every 10 seconds. The movement is based on a state machine with 4 states. The patrol state is the default state. When the player approaches, the enemy switches to the Chase state. When the player moves farther away, the enemy switches back to Patrol state. If the player gets extra close, the enemy will switch to melee state which involves combat animations and will ultimately impact the player's health. 
