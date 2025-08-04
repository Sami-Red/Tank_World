# Tank World  
**Unity | Multiplayer Top-Down Shooter | Final Year University Project**

**Tank World** is a top-down multiplayer shooter built in Unity as part of a final-year university module **Games Networking and Security**. The project explores core multiplayer concepts such as client-server communication, sync of movement and actions, and network authority.

## Project Goal

The objective was to design a simple but complete multiplayer experience using Unity's networking framework (Photon/Netcode/etc.), with a focus on:

- Smooth, responsive player movement over the network  
- Real-time shooting mechanics and hit detection  
- Handling latency, authority, and sync between clients  
- Creating a minimal but fun core gameplay loop  

## Features

- **Top-down twin-stick controls** with shooting and dodging  
- **Online multiplayer support** (tested in LAN and public rooms)  
- **Health, death, and respawn system**  
- **Basic lobby system** for joining/creating rooms  
- **Syncing movement, rotation, bullets, and health across players**  
- Designed using [Photon Unity Networking (PUN2)]

## Preview

[Play it on Itch.io](https://saaami.itch.io/tank-world)  

## Tech Stack

- Unity (URP)  
- C#  
- [Photon PUN2] or [Unity Netcode for GameObjects] *(whichever applies)*  
- Designed for Windows PC

## Folder Structure (Simplified)
Assets/
- Scripts/ # Player control, shooting, networking
- Prefabs/ # Player tanks, bullets, UI
- Scenes/ # Main menu, lobby, game arena
- Resources/ # Networked prefabs, assets
- UI/ # Health bars, join/create menu
ProjectSettings/
Packages/


## Lessons Learned

This project helped me understand:

- Managing real-time multiplayer logic and sync  
- Handling player authority and prediction  
- Networked prefab spawning and state updates  
- Debugging and testing multiplayer issues in Unity

## Project Status

This project was completed for assessment and is no longer in active development, but it remains a portfolio piece showcasing my work with networked gameplay in Unity.

## 👤 Developer

Solo project by [Sami](https://github.com/Sami-Red) as part of a final-year module at [The University of Westminster].

---

