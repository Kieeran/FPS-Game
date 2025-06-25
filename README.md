# FPS Game
![IntroGameFPS](https://github.com/user-attachments/assets/a4015f2c-a13e-4d2c-b6ff-c8f0f4c08a54)


## Description
This project is a course assignment focused on building a multiplayer game using Unity 3D and Unity's official services. The game leverages Relay to establish serverless communication between players, Netcode for GameObject to handle real-time networking for game objects, and Lobby for player matchmaking and room creation. The entire project is developed in C# and showcases the implementation of multiplayer functionalities in a seamless and engaging gaming experience.

## Built with

- [![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [![Unity](https://img.shields.io/badge/Unity-%23000000.svg?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/)
- [![.NET Framework](https://img.shields.io/badge/.NET_Framework-%235C2D91.svg?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/en-us/)

- [NGO (Netcode for GameObjects)](https://docs-multiplayer.unity3d.com/netcode/current/about/)
![501ec0daf62aa78f5744b0c6e973f4b373dd24d1-800x450](https://github.com/user-attachments/assets/93553ec0-aed1-4ccb-8a6f-77a7c54e79f5)

  Netcode for GameObjects (Netcode) is a high-level networking library built for Unity for you to abstract networking logic. It enables you to send GameObjects and world data across a networking session to many players at once. With Netcode, you can focus on building your game instead of low-level protocols and networking frameworks.

- [Unity Relay](https://docs.unity.com/ugs/manual/relay/manual/introduction)
![image](https://github.com/user-attachments/assets/f0758b91-1a1f-403a-b43b-79ce49d11983)

  Unity Relay exposes a way for game developers to securely offer increased connectivity between players by using a join code style workflow without needing to invest in a third-party solution, maintain dedicated game servers (DGS), or worry about the network complexities of a peer-to-peer game. Instead of using DGS, the Relay service provides connectivity through a universal Relay server acting as a proxy.

- [Unity Lobby](https://docs.unity.com/ugs/manual/lobby/manual/unity-lobby-service)
![image](https://github.com/user-attachments/assets/7f8c9cde-6f60-4409-b2a5-0998f084625a)

  The Lobby service provides a way for players to discover and connect to each other to accomplish a variety of multiplayer gaming scenarios. The Lobby can persist for the duration of the game session to provide a mechanism for users to re-join an existing game session or facilitate host-migration after an unexpected disconnect.
  
## Status
This project has been completed up to Phase 2 (Project 2). The next step will be the final phase: the Graduation Thesis.

## Features
- Full basic movement sync (idle, walk, run, jump)
- Complete weapon system including:
    + Rifle, Sniper, Pistol, Melee, and Grenade
    + Shooting, aiming, reloading mechanics
    + Damage system with hit effects and death effects
- Character model: players now use a full humanoid model instead of capsule (from Phase 1)
- Fully playable match loop:
    + Scoreboard system
    + Win/Loss (Victory / Defeat) conditions

## How to Run
### In Unity Editor
- Open the project in Unity (version 2022.3+ recommended)
- Install all dependencies via Unity Package Manager
- Install dependencies via Unity Package Manager
- Press `Play` to start

### From Build (Standalone)
- Ensure the project is linked to Unity Services (Relay + Authentication enabled)
- Build the project via `File > Build Settings`
- Launch the game executable

**Note:** Internet connection is required for Unity Relay to work properly.

## Acknowledgements
- Based on Unity's Boss Room sample
- Uses Unity Netcode for GameObjects and Relay SDK

## Sample results
Scene 1:
![Scene 1](https://github.com/user-attachments/assets/66cb1a18-83ca-4015-ac5a-7134d6c2704b)

Scene 2:
![Scene 2](https://github.com/user-attachments/assets/4d248274-8e35-410b-835f-303913d639d5)

Create lobby with info:
![image](https://github.com/user-attachments/assets/99bb063a-5b30-4841-8be5-73b362495f84)

Scene 3: 
![image](https://github.com/user-attachments/assets/586c66cb-3c32-4cf9-8899-f3f9f6ad6c42)

Scene 4:
![image](https://github.com/user-attachments/assets/745ed64a-4d97-4e91-a5ef-9c83fef7c036)

Video demo: [Link video youtube](https://www.youtube.com/watch?v=3TDs-37pTak)

Flow chart: [Link draw.io](https://drive.google.com/file/d/1SavhWu40WVZ4tlYFkBoMBUBUQ3jDOio6/view?usp=sharing)

# Todo
As I mention above, this project is on going there is a lot more left that needs to be worked on.
Some of the things left to do:
- Add more UI
- Add more types of guns
- Add model for the character
- Add game modes
- ...

# Credits
Specials thanks to: 
- [Haiseus](https://github.com/Haiseus) - for your help with this project
And thanks to many more who have helped with suggestions and feedback! 
