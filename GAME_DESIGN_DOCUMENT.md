# Alien Probe - Game Design Document

## 🎮 Game Overview

**Genre**: Puzzle Game with Comedy/Narrative Elements  
**Platform**: Mobile (iOS/Android)  
**Engine**: Unity 2D with Live2D Cubism SDK  
**Target Audience**: Casual puzzle gamers who enjoy humor and story progression  

**Core Concept**: Aliens Zorp and Xylar study human intelligence through rebus puzzles to determine if Earth should be destroyed. Players must prove human worth by solving visual word puzzles while experiencing hilarious alien commentary.

## 📅 Development Status (Updated: June 2025)

### ✅ **Implemented & Working:**
- **Core Puzzle Mechanics**: Multi-stage puzzle presentation (Standby → Silhouette → Rebus)
- **Character Animation System**: Zorp (penguin sprite) and Xylar (Live2D) with synchronized dialogue
- **Level Manager System**: Automatic progression, destruction meter, alien relationship tracking
- **Game Status UI**: Real-time destruction meter and alien relationship displays
- **Save/Load System**: Persistent progress between game sessions
- **Multiple Dialogue Responses**: Varied character reactions with different animations
- **Debug Tools**: Level Manager Editor for testing and progression control

### 🚧 **In Development:**
- **Additional Puzzle Content**: More rebus puzzles for each category
- **Audio System**: Character voice acting and sound effects
- **Advanced UI Polish**: Enhanced visual feedback and animations
- **Multiple Endings**: Different conclusion scenarios based on performance

## 🌍 Core Game Loop

1. **Puzzle Presentation**: Multi-stage reveal (Standby → Silhouette → Rebus)
2. **Player Interaction**: Select letters to spell solution
3. **Alien Response**: Character reactions with animations based on correct/incorrect answers
4. **Progression**: Destruction meter changes, story advances
5. **Next Level**: New puzzle with evolving alien relationships

## 🚀 Level Progression System

### Core Motivation: **Save Earth Through Proving Human Intelligence**

### Level Categories (Alien Research Departments):

#### **1. "Basic Cognitive Functions" (Levels 1-5)**
- **Concepts**: TIME, MONEY, FOOD, HOME, WORK
- **Alien Reactions**:
  - Zorp: "Even bacteria can understand this!"
  - Xylar: "Fascinating baseline data..."
- **Purpose**: Establish baseline human intelligence

#### **2. "Emotional Intelligence" (Levels 6-10)** 
- **Concepts**: LOVE, ANGER, HOPE, FEAR, JOY
- **Alien Reactions**:
  - Zorp: "Humans leak water from their face-holes?"
  - Xylar: "These chemical reactions are... illogical."
- **Purpose**: Test human emotional complexity

#### **3. "Cultural Behaviors" (Levels 11-15)**
- **Concepts**: PARTY, MUSIC, DANCE, GIFT, HOLIDAY
- **Alien Reactions**:
  - Zorp: "They waste energy moving rhythmically!"
  - Xylar: "Cultural rituals defy all efficiency protocols."
- **Purpose**: Examine human social constructs

#### **4. "Human Logic Paradoxes" (Levels 16-20)**
- **Concepts**: IRONY, SARCASM, HUMOR, TRADITION
- **Alien Reactions**:
  - Zorp: "My circuits are overheating!"
  - Xylar: "They say opposite of what they mean?!"
- **Purpose**: Challenge alien understanding of human complexity

#### **5. "Advanced Human Concepts" (Secret Levels 21+)**
- **Concepts**: ART, PHILOSOPHY, DREAMS, CREATIVITY
- **Unlocked**: Only through perfect performance
- **Purpose**: Prove humans transcend basic intelligence

## 🎭 Character System

### **Main Characters**

#### **Zorp (The Penguin Alien)**
- **Role**: Hostile researcher, wants to destroy Earth
- **Personality**: Impatient, aggressive, easily frustrated
- **Character Arc**: Hostile → Grudgingly Impressed → Protective
- **Animations**: idle, atack, jump, walk, slide, preslide
- **Dialogue Style**: Sarcastic, insulting, gradually more respectful

#### **Xylar (The Analytical Alien)**
- **Role**: Scientific researcher, objectively studying humans
- **Personality**: Clinical, curious, methodical
- **Character Arc**: Detached → Fascinated → Empathetic
- **Animations**: face01, face02, body (Live2D)
- **Dialogue Style**: Academic, increasingly emotional

### **Future Characters**
- **Dr. Bleebok**: Overly dramatic alien researcher
- **Commander Zyx**: Military efficiency-obsessed alien
- **The Narrator**: Omniscient observer providing context

## 🏆 Progression & Addiction Mechanics

### **Destruction Meter System**
- **Core Mechanic**: Earth's fate hangs in the balance
- **Correct Answer**: Reduces meter (-0.05 per puzzle)
- **Incorrect Answer**: Increases meter (+0.02 per puzzle)
- **Visual Feedback**: Earth getting redder/more threatened
- **Failure State**: Meter reaches 100% = Earth destroyed

### **Alien Relationship System**
- **Zorp Respect Meter**: Tracks from hostile to impressed
  - 0-25%: Actively insulting
  - 26-50%: Grudging acknowledgment
  - 51-75%: Surprised respect
  - 76-100%: Protective of humans
- **Xylar Curiosity Meter**: Tracks scientific fascination
  - 0-25%: Clinical detachment
  - 26-50%: Growing interest
  - 51-75%: Genuine fascination
  - 76-100%: Emotional investment

### **Earth Status Visualization**
- **Alien Fleet**: Ships leave as you prove human worth
- **Human Cities**: Lights dim as destruction meter rises
- **Environmental Changes**: Atmosphere, color shifts

## 🎯 What Players Hope to Achieve

### **Immediate Rewards (Each Level)**
- New alien reactions and animations
- Funnier dialogue as relationships evolve
- More complex, satisfying puzzles
- Visible progression toward saving Earth

### **Long-term Goals**
- **Prove Human Worth**: Show aliens we're not "intellectual voids"
- **Character Development**: Watch alien personalities evolve
- **Multiple Endings**: Different outcomes based on performance
- **Collection Elements**: Unlock alien research reports, concept art

## 🎮 Advanced Game Mechanics

### **Human Specimen Variety**
Each level tests different humans (different silhouettes):
- **Chef** → Food-related puzzles
- **Artist** → Creative concept puzzles
- **Engineer** → Logic and technology puzzles
- **Child** → Simple but profound concept puzzles
- **Elderly Person** → Wisdom and experience puzzles

### **Alien Department Rivalry**
- **Department of Destruction** (Zorp's team): Wants to destroy Earth
- **Department of Research** (Xylar's team): Wants to study humans
- **Department of Efficiency** (Future): Wants to optimize humans
- **Player Performance**: Affects which department gains influence

### **Dynamic Difficulty & Time Pressure**
- **Early Levels**: Aliens are patient, provide hints
- **Mid Levels**: Growing impatience, faster countdowns
- **Late Levels**: "JUST DESTROY THEM ALREADY!" - extreme time pressure
- **Adaptive**: Difficulty adjusts based on player performance

### **Meta-Commentary System**
- **Memory**: Aliens remember previous mistakes and successes
- **References**: Callback to earlier puzzles and performances
- **Running Jokes**: Develop over time through repeated interaction
- **Relationship Building**: Genuine character development through gameplay

## 🔧 Technical Implementation

### **Core Architecture (Implemented)**

#### **LevelManager.cs**
```csharp
// Singleton pattern for global game state management
public class LevelManager : MonoBehaviour
{
    // Puzzle progression
    public List<RebusPuzzleData> allPuzzles;
    public int currentLevelIndex = 0;
    
    // Game state tracking
    public float destructionMeter = 50f;
    public float zorpRespectMeter = 0f;
    public float xylarCuriosityMeter = 25f;
    
    // Automatic progression with 3-second delay
    public void OnPuzzleCompleted(bool wasCorrect) {
        // Update meters, check end conditions
        StartCoroutine(TransitionToNextLevel());
    }
}
```

#### **PuzzleController.cs**
```csharp
// Handles puzzle presentation and player interaction
public class PuzzleController : MonoBehaviour
{
    // Multi-stage puzzle sequence
    private IEnumerator SetupPuzzleUISequence(RebusPuzzleData puzzle)
    {
        // Stage 1: Standby (2s) → Stage 2: Silhouette (3s) → Stage 3: Rebus + UI
        yield return new WaitForSeconds(standbyDuration);
        // Show silhouette, then reveal puzzle
    }
    
    // Integration with LevelManager
    public void LoadPuzzleFromLevelManager(RebusPuzzleData puzzle) {
        StartCoroutine(SetupPuzzleUISequence(puzzle));
    }
}
```

#### **CharacterDialogueManager.cs**
```csharp
// Synchronized dialogue and animation system
public class CharacterDialogueManager : MonoBehaviour
{
    // Multi-character support
    public Animator zorpAnimator;   // Penguin sprite animations
    public Animator xylarAnimator;  // Live2D animations
    
    // Dynamic animation triggering
    Animator targetAnimator = GetAnimatorForSpeaker(dialogueLine.characterSpeaking);
    targetAnimator.SetTrigger(dialogueLine.live2DAnimationTrigger);
}
```

#### **GameStatusUI.cs**
```csharp
// Real-time game state visualization
public class GameStatusUI : MonoBehaviour
{
    // Animated meters with color-coding
    public Slider destructionMeterSlider;
    public Slider zorpRespectSlider;
    public Slider xylarCuriositySlider;
    
    // Smooth interpolation for visual appeal
    currentDestructionValue = Mathf.Lerp(currentValue, targetValue, deltaTime);
}
```

### **Data Structures (Implemented)**

#### **RebusPuzzleData.cs**
```csharp
[CreateAssetMenu(fileName = "NewPuzzle", menuName = "Alien Probe/Rebus Puzzle")]
public class RebusPuzzleData : ScriptableObject
{
    public string puzzleID;                    // "TP001", "TP002"
    public Sprite rebusImage;                  // Visual puzzle
    public string solution;                    // "UNITY", "TIME"
    public string letterBank;                  // "UNITYWFVD", "TIMEXYZAB"
    public List<DialogueLineData> CorrectDialogue;   // Multiple responses
    public List<DialogueLineData> IncorrectDialogue; // Multiple responses
    public Sprite humanSilhouetteForPuzzle;   // Pre-puzzle silhouette
    public float destructionMeterChangeOnCorrect = -0.05f;
    public float destructionMeterChangeOnIncorrect = 0.02f;
}
```

#### **DialogueLineData.cs**
```csharp
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Alien Probe/Dialogue Line")]
public class DialogueLineData : ScriptableObject
{
    public Speaker characterSpeaking;          // Zorp, Xylar, Narrator
    public string dialogueText;                // Character's response
    public string live2DAnimationTrigger;      // "atack", "jump", "face01"
    public AudioClip audioClip;                // Future voice acting
}
```

### **Testing & Debug Tools (Implemented)**

#### **LevelManagerEditor.cs**
```csharp
// Real-time testing interface
[MenuItem("Alien Probe/Level Manager Editor")]
public class LevelManagerEditor : EditorWindow
{
    // Runtime control panel
    - Manual level navigation (Previous/Next/Load specific)
    - Meter manipulation (destruction, alien relationships)
    - Answer simulation (correct/incorrect testing)
    - Progress reset and save state management
}
```

#### **PuzzleTestUtility.cs**
```csharp
// Animation and dialogue testing
[MenuItem("Alien Probe/Puzzle Test Utility")]
public class PuzzleTestUtility : EditorWindow
{
    // Individual animation testing
    - Zorp animations: atack, jump, walk, slide
    - Xylar animations: face01, face02, body
    - Dialogue sequence testing
}
```

### **Save System (Implemented)**
```csharp
// Persistent progress using PlayerPrefs
void SaveGameProgress()
{
    PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex);
    PlayerPrefs.SetFloat("DestructionMeter", destructionMeter);
    PlayerPrefs.SetFloat("ZorpRespect", zorpRespectMeter);
    PlayerPrefs.SetFloat("XylarCuriosity", xylarCuriosityMeter);
}
```

### **Animation Integration (Implemented)**
```csharp
// Zorp Penguin Animations (Sprite-based)
- penguin_idle.anim    → "idle" trigger
- penguin_atack.anim   → "atack" trigger  
- penguin_jump.anim    → "jump" trigger
- penguin_walk.anim    → "walk" trigger
- penguin_slide.anim   → "slide" trigger

// Xylar Live2D Animations
- face01, face02 → Facial expressions
- body → Body movement animations
```

## 🎨 Visual & Audio Design

### **Art Style**
- **2D Cartoon**: Bright, comedic alien aesthetic
- **Live2D Characters**: Smooth, expressive alien animations
- **Rebus Puzzles**: Clear, recognizable symbolic representations
- **UI**: Sci-fi themed with humor elements

### **Animation System**
- **Penguin Sprites**: Frame-based animation for Zorp
- **Live2D**: Smooth character animation for Xylar
- **TV Effects**: Custom shader for screen glitches and effects
- **Particle Systems**: Destruction meter visual feedback

### **Audio Design** (Future Implementation)
- **Voice Acting**: Unique alien voices for each character
- **Sound Effects**: Puzzle interactions, UI feedback
- **Music**: Atmospheric sci-fi with comedic undertones
- **Adaptive Audio**: Changes based on destruction meter level

## 🏁 Multiple Endings

### **Destruction Ending**
- **Trigger**: Destruction meter reaches 100%
- **Outcome**: Earth is destroyed
- **Alien Reactions**: Zorp satisfied, Xylar regretful

### **Salvation Ending**
- **Trigger**: Complete all levels with low destruction meter
- **Outcome**: Aliens leave Earth alone
- **Alien Reactions**: Both impressed with human potential

### **Conversion Ending**
- **Trigger**: Perfect or near-perfect performance
- **Outcome**: Aliens become Earth's protectors
- **Alien Reactions**: Character development complete

### **Academic Exchange Ending**
- **Trigger**: Unlock all secret levels
- **Outcome**: Aliens establish research partnership with Earth
- **Alien Reactions**: Mutual respect and cooperation

## 📱 Mobile Optimization

### **Touch Controls**
- **Large Touch Targets**: Easy letter selection
- **Gesture Support**: Swipe to clear, pinch for hints
- **Accessibility**: Color blind support, font size options

### **Performance**
- **Efficient Rendering**: Optimized for mobile GPUs
- **Battery Management**: Low power mode options
- **Storage**: Compressed assets, progressive download

### **Monetization** (Future Consideration)
- **Premium Game**: One-time purchase
- **Cosmetic DLC**: Additional alien characters, outfits
- **Hint System**: Optional IAP for struggling players

## 🔄 Future Expansion Ideas

### **Sequel Concepts**
- **Alien Probe 2**: Aliens return for advanced testing
- **Human Exchange**: Humans visit alien planet
- **Multispecies**: Test other alien species' intelligence

### **Additional Features**
- **Multiplayer**: Cooperative puzzle solving
- **Level Editor**: Player-created rebus puzzles
- **AR Mode**: Puzzles in real-world environments
- **Educational Mode**: Learn about actual human psychology

## 📊 Success Metrics

### **Engagement**
- **Session Length**: Average time per play session
- **Retention**: Day 1, 7, 30 retention rates
- **Completion Rate**: Percentage reaching final level

### **Monetization**
- **ARPU**: Average revenue per user
- **Conversion**: Free to paid user conversion
- **LTV**: Lifetime value calculations

### **Quality**
- **App Store Rating**: Target 4.5+ stars
- **User Reviews**: Positive feedback on humor and gameplay
- **Crash Rate**: Less than 1% crash rate

## 🔧 Development Notes & Known Issues

### **Fixed Issues (June 2025)**

#### **Animation System Fixes**
- ✅ **Fixed**: Animation trigger spelling mismatch - "attack" vs "atack" in penguin animations
- ✅ **Fixed**: Animator Controller not assigned to Zorp penguin character
- ✅ **Fixed**: Missing transitions between animation states

#### **Level Progression Fixes**
- ✅ **Fixed**: Game end condition triggered too early (after 1st level instead of last level)
- ✅ **Fixed**: Automatic level transition blocked by `isTransitioning` flag during `LoadCurrentLevel()`
- ✅ **Fixed**: Coroutine access issue - LevelManager calling PuzzleController coroutines incorrectly

#### **System Integration Fixes**
- ✅ **Fixed**: PuzzleController not receiving puzzles from LevelManager properly
- ✅ **Fixed**: Console showing confusing 0-based level numbers vs user-friendly 1-based display
- ✅ **Fixed**: Missing debug logging for transition troubleshooting

### **Current Workflow for Adding New Puzzles**

1. **Create RebusPuzzleData Asset**: `Assets → Create → Alien Probe → Rebus Puzzle`
2. **Configure Puzzle**: Set puzzleID, solution, letterBank, rebusImage, silhouette
3. **Create Dialogue Assets**: `Assets → Create → Alien Probe → Dialogue Line`
4. **Assign Dialogues**: Add correct/incorrect dialogue lists to puzzle
5. **Add to LevelManager**: Drag puzzle to `allPuzzles` array in LevelManager Inspector
6. **Test**: Use Level Manager Editor to test progression and dialogue

### **Debug Tools Usage**

#### **Level Manager Editor** (`Alien Probe → Level Manager Editor`)
- **Game State**: Monitor current level, destruction meter, alien relationships
- **Level Controls**: Navigate between levels, test specific puzzles
- **Meter Testing**: Simulate correct/incorrect answers and meter changes
- **Quick Actions**: Reset progress, reload levels, force UI updates

#### **Animation Testing**
- **Individual Triggers**: Test Zorp animations (atack, jump, walk, slide)
- **Dialogue Sequences**: Test multi-character conversations
- **UI Feedback**: Verify destruction meter and relationship changes

### **Performance Considerations**
- **Memory Management**: Puzzle assets loaded on-demand, cleared between levels
- **Animation Efficiency**: Sprite-based animations for Zorp, Live2D for Xylar
- **UI Updates**: Smooth interpolation for meter changes to avoid jarring updates
- **Save Frequency**: Progress saved after each level completion and on app pause

---

*This document serves as the comprehensive design blueprint for Alien Probe, capturing both the current implementation and future development roadmap.*