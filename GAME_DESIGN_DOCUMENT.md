# Alien Probe - Game Design Document

## 🎮 Game Overview

**Genre**: Puzzle Game with Comedy/Narrative Elements  
**Platform**: Mobile (iOS/Android)  
**Engine**: Unity 2D with Live2D Cubism SDK  
**Target Audience**: Casual puzzle gamers who enjoy humor and story progression  

**Core Concept**: Aliens Zorp and Xylar study human intelligence through rebus puzzles to determine if Earth should be destroyed. Players must prove human worth by solving visual word puzzles while experiencing hilarious alien commentary.

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

### **Puzzle Data Structure**
```
RebusPuzzleData:
- puzzleID: Unique identifier
- rebusImage: Visual puzzle sprite
- solution: Correct answer string
- letterBank: Available letters (solution + distractors)
- CorrectDialogue: List of positive response dialogues
- IncorrectDialogue: List of negative response dialogues
- humanSilhouetteForPuzzle: Human silhouette sprite
- destructionMeterChangeOnCorrect: Meter adjustment value
- destructionMeterChangeOnIncorrect: Meter adjustment value
```

### **Dialogue System**
```
DialogueLineData:
- characterSpeaking: Which alien responds
- dialogueText: What they say
- live2DAnimationTrigger: Animation to play
- audioClip: Voice acting (future)
```

### **Level Progression**
```
LevelManager:
- currentLevel: Track progress
- puzzleQueue: Ordered list of puzzles
- destructionMeter: Current Earth threat level
- alienRelationships: Zorp/Xylar relationship values
- unlockedContent: Bonus materials, secret levels
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

---

*This document serves as the comprehensive design blueprint for Alien Probe, capturing both the current implementation and future development roadmap.*