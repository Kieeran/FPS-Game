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
1. Open the project in Unity (version 2022.3+ recommended)
2. Install all dependencies via Unity Package Manager
3. Install dependencies via Unity Package Manager
4. Press `Play` to start

### From Build (Standalone)
1. Ensure the project is linked to Unity Services (Relay + Authentication enabled)
2. Build the project via `File > Build Settings`
3. Launch the game executable

**Note:** Internet connection is required for Unity Relay to work properly.

## Sample results
### Scene 1: Sign in

![Capture](https://github.com/user-attachments/assets/6e5ddd10-bec2-48b8-b628-9fed57acc3fa)

### Scene 2: Lobby List

![Capture2](https://github.com/user-attachments/assets/9d250f5b-9df3-42fa-8dfe-6662c9f49135)

#### Create lobby with info:

![Capture5](https://github.com/user-attachments/assets/08b2822d-a152-4423-a9a3-d12f35c0be3c)

### Scene 3: Lobby Room

![Capture3](https://github.com/user-attachments/assets/4519d947-df7a-45c1-97d4-e530de97eaed)

### Scene 4: Play Scene

![Capture4](https://github.com/user-attachments/assets/7a03938e-69b4-4b23-bdec-6d78d944d6aa)

### Video demo: [Link video youtube](https://www.youtube.com/watch?v=Jn3rT69-qWA)
### Flow chart: [Link draw.io](https://drive.google.com/file/d/1SavhWu40WVZ4tlYFkBoMBUBUQ3jDOio6/view?usp=sharing)

## Current Limitations and Future Development
### Current Limitations
- UI/UX is still basic and under development
- Player character animations are not natural; movement lacks polish and proper animation logic
- The game still experiences noticeable lag
- High latency difference between host and clients causes gameplay imbalance

### Future Development (Graduation Thesis Phase)
- Add AI bots for training or offline play
- Improve UI/UX (main menu, HUD, scoreboard, lobby navigation)
- Add game modes (Team Deathmatch, Bomb Defusal)
- Polish character animation, effects, and network optimization
- Add save/load, progression, or stats system (optional for polish)

## Credits
Developed by [Kieeran](https://github.com/Kieeran) and [Haiseus](https://github.com/Haiseus) as part of the graduation project<br>
Special thanks to:<br>
- [Haiseus](https://github.com/Haiseus) - for your help with this projec

And thanks to everyone who have helped with suggestions and feedback! 
