# Table of Contents
<a href="#introduction">Introduction</a><br/>
<a href="#built-with">Built with</a><br/>
<a href="#status">Status</a><br/>
<a href="#features">Features</a><br/>
<a href="#how-to-run">How to Run</a><br/>
&nbsp;&nbsp;&nbsp;&nbsp;<a href="#in-unity-editor">In Unity Editor</a><br/>
&nbsp;&nbsp;&nbsp;&nbsp;<a href="#from-build-standalone">From Build (Standalone)</a><br/>
<a href="#sample-results">Sample Results</a><br/>
&nbsp;&nbsp;&nbsp;&nbsp;<a href="#scene-1-sign-in">Scene 1: Sign in</a><br/>
&nbsp;&nbsp;&nbsp;&nbsp;<a href="#scene-2-lobby-list">Scene 2: Lobby List</a><br/>
&nbsp;&nbsp;&nbsp;&nbsp;<a href="#scene-3-lobby-room">Scene 3: Lobby Room</a><br/>
&nbsp;&nbsp;&nbsp;&nbsp;<a href="#scene-4-play-scene">Scene 4: Play Scene</a><br/>
<a href="#current-limitations-and-future-development">
  Current Limitations and Future Development
</a><br/>
&nbsp;&nbsp;&nbsp;&nbsp;<a href="#current-limitations">Current Limitations</a><br/>
&nbsp;&nbsp;&nbsp;&nbsp;<a href="#future-development-graduation-thesis-phase">Future Development</a><br/>
<a href="#credits">Credits</a>

# Introduction
![IntroGameFPS](https://github.com/user-attachments/assets/a4015f2c-a13e-4d2c-b6ff-c8f0f4c08a54)
This project is a graduation thesis presents the development of a server-authoritative 3D multiplayer FPS game built using Unity and Unity Gaming Services. The system utilizes Unity Relay for serverless connectivity, Netcode for GameObjects (NGO) for real-time synchronization of networked entities, and Unity Lobby for matchmaking and session management.

Beyond basic multiplayer functionality, the project focuses on the integration of AI-controlled bots operating in a networked environment. These bots are fully controlled by the host and synchronized across all clients, ensuring consistent behavior and fairness in gameplay. A hybrid Finite State Machine – Behavior Tree (FSM–BT) architecture is employed to manage bot decision-making, balancing structured state control with flexible behavior execution.

The goal of this thesis is to design, implement, and evaluate a scalable multiplayer FPS architecture that supports both human players and network-synchronized AI agents using modern Unity networking services.


# Built with

- [![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [![Unity](https://img.shields.io/badge/Unity-%23000000.svg?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/)
- [![.NET Framework](https://img.shields.io/badge/.NET_Framework-%235C2D91.svg?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/en-us/)
- [NGO (Netcode for GameObjects)](https://docs-multiplayer.unity3d.com/netcode/current/about/)
- [Unity Relay](https://docs.unity.com/ugs/manual/relay/manual/introduction)
- [Unity Lobby](https://docs.unity.com/ugs/manual/lobby/manual/unity-lobby-service)
  
# Status
Completed and closed after successful Graduation Thesis defense. Only minor refinements and bug fixes remain.

# Features
- Map game: the *Italy* map inspired by Counter-Strike
- Character model: fully humanoid player model
- Full basic movement sync (idle, walk, run, jump)
- Complete weapon system including:
    + Weapon: Rifle, Sniper, Pistol, Melee, and Grenade
    + Core mechanics: shooting, aiming, reloading, and weapon switching
    + Server-authoritative damage system with hit and death effects to prevent cheating
- Fully playable match loop:
    + Scoreboard system
    + Win/Loss (Victory / Defeat) conditions
- AI Bots with a hybrid FSM–Behavior Tree architecture:
    + High-level states: Idle, Patrol, Combat
    + Detailed behaviors implemented as BT tasks: LookAround, ScanArea, Seek, AimAtPlayer, Attack
- Zone-based spatial reasoning system:
    + ZoneData and InfoPoint structures support area scanning, tactical positioning, and target pursuit
    + Enables bots to evaluate visible areas, select navigation targets, and perform contextual pathfinding

# How to Run
## In Unity Editor
1. Open the project in Unity (version 2022.3+ recommended)
2. Install all dependencies via Unity Package Manager
3. Install dependencies via Unity Package Manager
4. Press `Play` to start

## From Build (Standalone)
1. Ensure the project is linked to Unity Services (Relay + Authentication enabled)
2. Build the project via `File > Build Settings`
3. Launch the game executable

**Note:** Internet connection is required for Unity Relay to work properly.

# Sample results
## Scene 1: Sign in

![Capture](https://github.com/user-attachments/assets/6e5ddd10-bec2-48b8-b628-9fed57acc3fa)

## Scene 2: Lobby List

![Capture2](https://github.com/user-attachments/assets/9d250f5b-9df3-42fa-8dfe-6662c9f49135)

### Create lobby with info:

![Capture5](https://github.com/user-attachments/assets/08b2822d-a152-4423-a9a3-d12f35c0be3c)

## Scene 3: Lobby Room

![Capture3](https://github.com/user-attachments/assets/4519d947-df7a-45c1-97d4-e530de97eaed)

## Scene 4: Play Scene

![Capture4](https://github.com/user-attachments/assets/7a03938e-69b4-4b23-bdec-6d78d944d6aa)

## Video demo: [Link video youtube](https://www.youtube.com/watch?v=Jn3rT69-qWA)
## Flow chart: [Link draw.io](https://drive.google.com/file/d/1SavhWu40WVZ4tlYFkBoMBUBUQ3jDOio6/view?usp=sharing)

# Current Limitations and Future Development
## Current Limitations
- UI/UX is still basic and under development
- Player character animations are not natural; movement lacks polish and proper animation logic
- The game still experiences noticeable lag
- High latency difference between host and clients causes gameplay imbalance

## Future Development (Graduation Thesis Phase)
- Add AI bots for training or offline play
- Improve UI/UX (main menu, HUD, scoreboard, lobby navigation)
- Add game modes (Team Deathmatch, Bomb Defusal)
- Polish character animation, effects, and network optimization
- Add save/load, progression, or stats system (optional for polish)

# Credits
Developed by [Kieeran](https://github.com/Kieeran) and [Haiseus](https://github.com/Haiseus) as part of the graduation project<br>
Special thanks to:<br>
- [Haiseus](https://github.com/Haiseus) - for your help with this project

And thanks to everyone who have helped with suggestions and feedback! 
