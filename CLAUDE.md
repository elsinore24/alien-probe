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
- Answer validation
- TV screen presentation

### Dialogue System (CharacterDialogueManager.cs)
- Multiple speakers: Xylar, Zorp, Narrator, Human
- Live2D animation triggers
- ScriptableObject-based conversations

### Data Architecture
- **RebusPuzzleData**: Puzzles, solutions, letter banks
- **DialogueLineData**: Character dialogue, animations

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
- Puzzle Data: Assets/_ScriptableObjects/RebusPuzzles/
- Dialogue Data: Assets/_ScriptableObjects/DialogueLines/

## Recent Changes
- Created new main scene: MmainGameScene
- Cleaned up troubleshooting tools and scripts
- Resolved Live2D gizmo display issues
- Fixed TextMeshPro rendering problems

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