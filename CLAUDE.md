# Alien Probe - Project Documentation for Claude

## 🚀 STARTUP CHECKLIST
When starting a new conversation, always:
1. Read this CLAUDE.md file first
2. Check git status and recent commits (`git status` and `git log --oneline -10`)
3. Scan MainGameScene.unity for current setup
4. Review any TODO markers in recent code
5. Check current branch name for context
6. Check screenshots folder for latest Unity Editor state: `/mnt/c/Users/Daniel/Desktop/screenshots/`

## 🖥️ ENVIRONMENT NOTES
- Running in WSL (Windows Subsystem for Linux) environment
- Use Linux commands and paths (e.g., `/mnt/c/` for Windows C: drive)
- Cannot directly capture Windows screenshots - check the screenshots folder instead
- Unity MCP Server runs on Windows, not accessible from WSL sessions

## Project Overview
**Game**: Alien Probe - A comedic mobile puzzle game
**Core Concept**: Aliens Zorp and Xylar study human intelligence through rebus puzzles
**Platform**: Mobile (iOS/Android)
**Engine**: Unity 2D with Live2D Cubism SDK

## Game Vision
- **Comedy**: Aliens overthinking simple human concepts through rebus puzzles
- **Gameplay**: Solve visual word puzzles while experiencing alien commentary
- **Tone**: Parody of alien abduction tropes with incompetent alien researchers

## Current Development State
- ✅ Core puzzle mechanics implemented
- ✅ Dialogue system with Live2D integration
- ✅ Multi-stage puzzle presentation (silhouette → glitch → rebus)
- 🚧 Destruction meter system
- 🚧 Audio implementation
- 🚧 Additional puzzle content

## Key Systems

### Puzzle System (PuzzleController.cs)
- Multi-stage introduction sequence
- Letter tile selection mechanics
- Answer validation with retry on incorrect
- TV screen presentation
- Clear answer slots after incorrect attempt (3s delay)

### Dialogue System (CharacterDialogueManager.cs)
- Multiple speakers: Xylar, Zorp, Narrator, Human
- Animation triggers for character expressions
- ScriptableObject-based conversations
- Auto-advance dialogue (configurable delay, default 2.5s)
- Auto-discovery of character animators via GameObject.Find()
- Character-specific animators:
  - Xylar: 6 expressions (Neutral, Happy, Surprised, Thinking, Concerned, Excited)
  - Zorp: penguin animations (jump, walk, slide, atack)

### Level Manager (LevelManager.cs)
- Manages puzzle progression with two modes:
  - **Randomized Pool Mode**: Uses SimplePuzzlePoolManager for PersonPuzzleSet system
  - **Linear Mode**: Traditional sequential puzzle progression
- Tracks destruction meter (0-100%)
- Only advances on correct answers
- Game over when destruction reaches 100%
- Manages alien relationship meters (Zorp respect, Xylar curiosity)

### Data Architecture
- **RebusPuzzleData**: Individual puzzles with solutions, letter banks, dialogue
- **DialogueLineData**: Character dialogue, animations
- **PersonPuzzleSet**: Groups 3-5 puzzles per mystery person with silhouette
- **SimplePuzzlePoolManager**: Manages randomized PersonPuzzleSet selection

## Code Standards
- No comments unless requested
- Follow existing conventions
- Use ScriptableObjects for data
- Maintain prefab-based UI

## Testing Commands
```bash
# Unity testing
# Add Unity test runner commands when discovered

# Linting/Type checking  
# Add when lint/typecheck commands are identified
```

## Important File Locations
- Main Scene: Assets/_Scenes/MainGameScene.unity
- Puzzle Controller: Assets/_Scripts/PuzzleController.cs
- Dialogue Manager: Assets/_Scripts/CharacterDialogueManager.cs
- Level Manager: Assets/_Scripts/Managers/LevelManager.cs
- Pool Manager: Assets/_Scripts/Managers/SimplePuzzlePoolManager.cs
- Puzzle Data: Assets/_ScriptableObjects/RebusPuzzles/
- Person Sets: Assets/_ScriptableObjects/PersonSets/
- Dialogue Data: Assets/_ScriptableObjects/DialogueLines/

## Recent Changes
- Created new main scene: MainGameScene
- Cleaned up troubleshooting tools and scripts
- Resolved Live2D gizmo display issues
- Fixed TextMeshPro rendering problems
- Implemented retry mechanism for incorrect puzzle answers
- Added Xylar character with 6 expression animations
- Fixed animator controllers (set layer weight from 0 to 1)
- Added auto-advance dialogue system (2.5s delay)
- Adjusted camera orthographic size to ~540 for UI scale
- Fixed animation exit times (3-5 seconds) for visible expressions
- Implemented LevelManager with dual modes: randomized pool & linear progression
- Created PersonPuzzleSet system for grouped puzzles per character
- Fixed PersonPuzzleSet inspector visibility (removed HideInInspector)
- Created puzzle generation tools for Arnold, Homer, Tom Hanks themes
- Added letter bank shuffling utility to prevent obvious solutions
- **FIXED XYLAR ANIMATIONS**: Updated trigger names from TriggerExpression1-6 to descriptive names (Happy, Surprised, etc.)
- **FIXED AUTO-DISCOVERY**: CharacterDialogueManager now auto-finds animators via GameObject.Find() for both Xylar and Zorp
- **ANIMATION SETUP**: Assigned animation clips to Xylar states and adjusted exit times for proper expression duration
- **REMOVED LIVE2D**: Switched from Live2D to PNG sequence animations for all characters
- **IDLE ANIMATION SYSTEM**: Created XylarIdleController for periodic idle animations during puzzle gameplay
  - Plays random idle animations every 45-75 seconds
  - Includes: IdleGunSpin, IdleLookAround, IdleStretch
  - Auto-stops during dialogue, resumes after
  - Component must be added to Xylar_Character GameObject

## TV Screen Effects System
- **Shader**: Assets/_Art/Shaders/TVScreenEffects.shader
  - Static noise, scanlines, glitch, glow effects
  - Mobile-optimized with blend control
- **Controller**: Assets/_Scripts/UI/TVScreenEffectsController.cs
  - Preset system: Standby, Glitch, Normal
  - Smooth transitions between effects
  - Runtime control integration

## Notes
- Project uses new Input System
- Live2D models: Koharu (test), Mao (test)
- TV screen uses mask system for content display
- Effects overlay uses custom shader material
- Unity MCP Bridge shows "Could not find Python directory" warning but works via Claude Desktop
- Python installation path: C:\Python313\python.exe
- Camera orthographic size: ~540 (to match UI scale)
- Animator layer weights must be 1 (not 0) for animations to play
- Animation exit times control how long expressions are held
- DialogueManager auto-discovers character animators if not manually assigned
- Animation trigger names must match between dialogue assets and animator parameters
- Zorp works via auto-discovery of "Zorp_Character_Penguin", Xylar via "Xylar_Character"