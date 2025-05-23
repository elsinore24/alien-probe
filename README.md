# Alien Probe

A Unity 2D mobile game featuring aliens Zorp and Xylar conducting rebus puzzles on humans.

## Project Overview

Alien Probe is a comedic puzzle game where two alien researchers attempt to understand human intelligence through rebus puzzles. Players must solve visual word puzzles while experiencing humorous banter between the alien characters.

### Key Features
- Rebus puzzle gameplay mechanics
- Live2D character animations for Zorp and Xylar
- Dynamic dialogue system with voice-over support
- Destruction meter that responds to player performance
- Mobile-optimized 2D graphics

## Technical Stack
- **Engine**: Unity (2D Mobile template)
- **Platform**: Mobile (iOS/Android)
- **Animation**: Live2D Cubism SDK
- **Version Control**: Git

## Project Structure
```
Assets/
├── _Art/
│   ├── Characters/      # Zorp, Xylar, and human silhouette art
│   ├── Environments/    # Spaceship interior backgrounds
│   ├── UI/             # Interface elements, buttons, meters
│   └── RebusImages/    # Puzzle image assets
├── _Audio/             # Sound effects and voice-overs
├── _Prefabs/           # Reusable game objects
├── _Scenes/            # Game scenes
├── _ScriptableObjects/ # Data containers for puzzles and dialogue
│   ├── RebusPuzzles/
│   └── DialogueLines/
└── _Scripts/           # C# game scripts
    ├── Data/           # ScriptableObject definitions
    ├── Gameplay/       # Core game mechanics
    ├── UI/             # User interface logic
    ├── Live2D/         # Character animation controllers
    └── Managers/       # Game state management
```

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/elsinore24/alien-probe.git
   ```

2. Open the project in Unity (2D Mobile template recommended)

3. Open the main scene: `Assets/_Scenes/MainGameScene.unity`

## Development Status

🚧 **Early Development** - Core architecture and data structures are being established.

## License

[License information to be added]

---

*"Humans are fascinating specimens... if only we could understand their strange picture-words!"* - Zorp