# Hybrid Architecture: Unity + React Native

## Overview

This project uses a hybrid approach combining Unity for game logic and React Native for UI/UX.

## Why Hybrid?

- **Unity:** Best for game logic, card animations, physics, and visual effects
- **React Native:** Best for modern UI, navigation, social features, and cross-platform app shell

## Architecture Diagram

```
┌─────────────────────────────────────────────────────┐
│          React Native App (TypeScript)              │
│                                                     │
│  ┌─────────────┐  ┌──────────────┐  ┌───────────┐ │
│  │   Lobby     │  │    Profile   │  │  Settings │ │
│  └─────────────┘  └──────────────┘  └───────────┘ │
│                                                     │
│  ┌─────────────────────────────────────────────┐  │
│  │         Game Selection Menu                 │  │
│  │  [Solitaire] [Poker] [Blackjack]           │  │
│  └─────────────────────────────────────────────┘  │
│                       │                            │
│                  User taps game                    │
│                       ▼                            │
│  ┌─────────────────────────────────────────────┐  │
│  │         Unity View Container                │  │
│  │  ┌───────────────────────────────────────┐ │  │
│  │  │       Unity Game Instance             │ │  │
│  │  │    (Solitaire/Poker/Blackjack)       │ │  │
│  │  └───────────────────────────────────────┘ │  │
│  └─────────────────────────────────────────────┘  │
│                       │                            │
│              Game sends results back               │
│                       ▼                            │
│            Update balance, stats, UI               │
└─────────────────────────────────────────────────────┘
```

## Communication Protocol

### React Native → Unity

```typescript
// From React Native
unityView.postMessage('StartGame', JSON.stringify({
  gameType: 'solitaire',
  difficulty: 'easy',
  playerBalance: 1000
}));
```

### Unity → React Native

```csharp
// From Unity
public void SendGameResult(int score, bool won) {
    var result = new {
        score = score,
        won = won,
        coinsEarned = score * 10
    };

    SendMessageToRN("GameComplete", JsonUtility.ToJson(result));
}
```

## Platform-Specific Integration

### Mobile (iOS/Android)

**Library:** `react-native-unity-view` or `@azesmway/react-native-unity`

**Setup:**
1. Build Unity project for iOS/Android
2. Export as native library
3. Integrate into React Native project
4. Use UnityView component

```typescript
import UnityView from '@azesmway/react-native-unity';

<UnityView
  style={{ flex: 1 }}
  onUnityMessage={(message) => {
    console.log('Message from Unity:', message);
  }}
/>
```

### Web

**Approach:** Unity WebGL embedded in React app

```typescript
// Load Unity WebGL build
useEffect(() => {
  const script = document.createElement('script');
  script.src = '/unity-build/Build.loader.js';
  script.onload = () => {
    createUnityInstance(canvas, config).then((instance) => {
      setUnityInstance(instance);
    });
  };
  document.body.appendChild(script);
}, []);
```

### Desktop (Windows/Mac)

**Options:**
1. Electron + Unity WebGL
2. Unity standalone with custom UI
3. Full React Native Desktop (Electron) + Unity subprocess

## Message Types

### From React Native to Unity

| Message | Data | Purpose |
|---------|------|---------|
| `StartGame` | `{ gameType, difficulty, balance }` | Initialize game |
| `PauseGame` | `{}` | Pause current game |
| `ResumeGame` | `{}` | Resume paused game |
| `QuitGame` | `{}` | Exit to lobby |

### From Unity to React Native

| Message | Data | Purpose |
|---------|------|---------|
| `GameReady` | `{}` | Game loaded and ready |
| `GameComplete` | `{ score, won, coinsEarned }` | Game finished |
| `GamePaused` | `{}` | Game paused |
| `AdRequest` | `{ type: 'rewarded' \| 'interstitial' }` | Request ad display |

## Data Flow

1. **User selects game in RN**
   - RN: Load Unity view
   - RN → Unity: Send `StartGame` message

2. **User plays game in Unity**
   - Unity: Handle all game logic
   - Unity: Render cards, animations
   - Unity: Track score, moves

3. **Game completes**
   - Unity → RN: Send `GameComplete` with results
   - RN: Update player balance
   - RN: Save progress to Firebase
   - RN: Show results screen
   - RN: Return to lobby

## State Management

### React Native (Global State)
- User profile
- Balance/coins
- Game history
- Settings
- Achievements

### Unity (Game State)
- Current game state
- Card positions
- Score
- Moves
- Timer

## Best Practices

1. **Keep Unity focused on gameplay only**
   - No user management in Unity
   - No navigation in Unity
   - No balance updates in Unity

2. **Let React Native handle app logic**
   - User authentication
   - Data persistence
   - Navigation
   - Social features

3. **Minimize message passing**
   - Send data only when needed
   - Batch updates when possible
   - Use simple JSON structures

4. **Handle errors gracefully**
   - Unity crashes shouldn't crash RN app
   - Timeout for Unity responses
   - Fallback UI if Unity fails to load

## Development Workflow

1. **Develop Unity game standalone first**
   - Build and test in Unity Editor
   - Export builds for testing

2. **Build RN shell**
   - Create lobby/menu
   - Set up navigation

3. **Integrate**
   - Add Unity view to RN
   - Set up message passing
   - Test communication

4. **Deploy**
   - Build Unity for each platform
   - Build RN app with Unity embedded
   - Test on devices
