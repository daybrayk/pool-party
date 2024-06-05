Pool Party (Temp Title) is a project I created to learn some of the basics of networked multiplayer games and get experience with Unity's Netcode For GameObjects package. It is a top down 2D networked multiplayer party shooter based around having a backyard watergun fight.

Unity Version: 2020.3.28

## Controls:
Move - WASD
Aim - Mouse
Shoot - Left Click
Recharge Weapon - R (Water guns lose pressure as you shoot, causing projectile speed and distance to be reduced unless recharged)
Reload Weapon - R while near a water source
Shield - Left Shift
Switch Weapon -  Scroll wheel up/down

## How to play/test:
Open two versions of the game, using either the Unity Editor and a build instance, or two running instances of a build. Have one of the versions click the "Host" button and fill in the provided spaces for Room Name (use a unique Room Name, or leave as temp if you want) and Display Name. Then have the other click the "Client" button and use the same Room Name that was used by the Host and fill in the Display Name. You should now both be in the same lobby. The Host can then change the game mode. When all players, except the Host, **Ready Up** the Host should then be able to start the game. Now you will be in the same world and can run around and shoot each other.

## Known Issues:
Due to updates in the Netcode For Gameobjects package the project is currently broken and clients cannot connect to a Host lobby in new project builds. However the old build found in the **PoolPartyPrototype** zip file still works if you want to checkout the game.
