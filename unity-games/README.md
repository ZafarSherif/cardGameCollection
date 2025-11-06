# Unity Card Games

Unity game projects for the Card Game Collection.

## Project Structure

```
unity-games/
├── CardGames/                    # Main Unity project
│   └── Assets/
│       ├── Scripts/
│       │   ├── Core/            # Core card system
│       │   │   ├── Card.cs      ✅ Card data and logic
│       │   │   └── Pile.cs      ✅ Pile management
│       │   ├── Solitaire/       # Solitaire-specific
│       │   │   ├── SolitaireGameManager.cs  ✅ Game manager
│       │   │   └── DraggableCard.cs         ✅ Drag & drop
│       │   └── Bridge/          # React Native integration
│       │       └── ReactNativeBridge.cs     ✅ RN communication
│       ├── Prefabs/             # Card and pile prefabs
│       ├── Sprites/             # Card sprites (to be added)
│       └── Scenes/              # Game scenes
├── UNITY_PROJECT_SETUP.md       # Step-by-step setup guide
└── README.md                    # This file
```

## What's Been Created

### ✅ Core Systems

**Card.cs** - Complete card system with:
- Suit and Rank enums
- Face up/down state
- Stacking rules for tableau
- Foundation placement rules
- Visual management

**Pile.cs** - Pile management with:
- 4 pile types (Stock, Waste, Tableau, Foundation)
- Card acceptance validation
- Automatic card positioning
- Face-up/down handling
- Card fanning for tableau

### ✅ Solitaire Game

**SolitaireGameManager.cs** - Full game logic:
- Deck creation and shuffling
- Card dealing (7 tableau piles + stock)
- Drawing from stock (1 or 3 cards)
- Move validation
- Scoring system
- Win condition detection
- Game state management

**DraggableCard.cs** - Player interaction:
- Drag and drop cards
- Multi-card dragging (for sequences)
- Valid drop zone detection
- Return to original position on invalid move
- Double-click to auto-move to foundation

### ✅ React Native Bridge

**ReactNativeBridge.cs** - Communication layer:
- Receives messages from React Native
- Sends game events to React Native
- Platform-specific implementations (WebGL, iOS, Android)
- Score, moves, and game completion events

## Getting Started

### Prerequisites

- Unity 2022 LTS or later
- Unity Hub

### Quick Start

1. **Follow the setup guide:**
   ```bash
   See UNITY_PROJECT_SETUP.md for detailed instructions
   ```

2. **Open Unity Hub** → **Open** → Select `CardGames` folder

3. **Import card sprites** (or use placeholders for testing)

4. **Follow Step-by-Step Setup Guide** in UNITY_PROJECT_SETUP.md

## Game Features

### Implemented ✅
- Standard 52-card deck
- Klondike Solitaire rules
- Drag and drop interaction
- Stock pile (draw 1 or 3 cards)
- 7 Tableau piles
- 4 Foundation piles
- Automatic card positioning
- Move validation
- Scoring system
- Win detection

### To Be Added 🚧
- Card sprites (currently using placeholders)
- Animations (card flip, movement)
- Sound effects
- Particle effects on win
- Undo/Redo system
- Hint system
- Auto-complete
- UI polish (win screen, pause menu)

## Building for Platforms

### WebGL (For Web Testing)
```
1. File → Build Settings
2. Select WebGL → Switch Platform
3. Build → Choose output folder
4. Test in browser
```

### iOS
```
1. File → Build Settings
2. Select iOS → Switch Platform
3. Build → Opens Xcode project
4. Configure signing in Xcode
5. Build and run on device
```

### Android
```
1. File → Build Settings
2. Select Android → Switch Platform
3. Build → Generates APK/AAB
4. Install on device
```

## React Native Integration

The bridge is ready for integration with your React Native app.

### Messages FROM React Native → Unity

```javascript
// Start game
unityView.postMessage('StartGame', JSON.stringify({ difficulty: 'easy' }));

// Pause game
unityView.postMessage('PauseGame', '{}');

// Resume game
unityView.postMessage('ResumeGame', '{}');

// Quit game
unityView.postMessage('QuitGame', '{}');
```

### Messages FROM Unity → React Native

```javascript
// Game ready
{ type: "GameReady" }

// Score update
{ type: "ScoreUpdate", data: { score: 150 } }

// Moves update
{ type: "MovesUpdate", data: { moves: 42 } }

// Game complete
{
  type: "GameComplete",
  data: {
    won: true,
    score: 250,
    timeSeconds: 180,
    coinsEarned: 150
  }
}
```

## Development Tips

### Testing in Unity Editor

- Press **Play** to test the game
- Check Console for errors/logs
- Use Scene view to adjust pile positions
- Test drag and drop on different piles

### Debugging

- Enable debug logs in ReactNativeBridge
- Use Debug.Log() statements
- Check script compilation errors in Console
- Verify all prefab references are assigned

### Performance

- Use object pooling for cards (if spawning/destroying frequently)
- Optimize sprite atlases
- Profile using Unity Profiler
- Target 60 FPS on mobile

## File Overview

| File | Purpose | Lines |
|------|---------|-------|
| Card.cs | Card data model | ~200 |
| Pile.cs | Pile management | ~250 |
| SolitaireGameManager.cs | Game orchestration | ~350 |
| DraggableCard.cs | Input handling | ~200 |
| ReactNativeBridge.cs | RN communication | ~230 |

Total: ~1,230 lines of clean, documented C# code

## Architecture

```
GameManager (SolitaireGameManager)
├── Creates Deck (52 cards)
├── Manages Piles
│   ├── Stock Pile (24 cards initially)
│   ├── Waste Pile (empty initially)
│   ├── Tableau Piles × 7 (28 cards total)
│   └── Foundation Piles × 4 (empty initially)
├── Handles Game Logic
│   ├── Move Validation
│   ├── Scoring
│   └── Win Detection
└── Communicates with React Native (Bridge)
```

## Next Steps

1. **Complete Unity Setup** (see UNITY_PROJECT_SETUP.md)
2. **Add Card Sprites** - Import or create 53 sprites
3. **Test Gameplay** - Play through a full game
4. **Polish** - Add animations and effects
5. **Build WebGL** - Test in browser
6. **Integrate with RN** - Connect to mobile app

## Resources

- [Unity Manual](https://docs.unity3d.com/Manual/)
- [C# Scripting Reference](https://docs.unity3d.com/ScriptReference/)
- [2D Game Development](https://learn.unity.com/tutorial/2d-game-kit)
- [Unity Asset Store](https://assetstore.unity.com/) - For card sprites

## Credits

Built with Unity 2022 LTS and C# for the Card Game Collection project.
