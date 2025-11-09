# Hybrid Architecture Migration Plan
## React Native UI + Unity Game Engine

**Goal:** Separate UI layer (React Native) from game rendering (Unity) for better scalability across the game collection.

---

## 🎯 Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                  React Native Layer                      │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐     │
│  │   Header    │  │   Buttons   │  │   Controls  │     │
│  │ Score/Timer │  │  Actions    │  │  Sliders    │     │
│  └─────────────┘  └─────────────┘  └─────────────┘     │
│         ↕ PostMessage API (JSON events)                 │
│  ┌───────────────────────────────────────────────────┐  │
│  │              Unity WebGL Game                      │  │
│  │  - Card rendering & animations                     │  │
│  │  - Game logic & state                              │  │
│  │  - Drag/drop interactions                          │  │
│  │  - NO UI elements (pure game)                      │  │
│  └───────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

---

## 📋 Phase 1: Unity Cleanup (2-3 hours)

### 1.1 Remove Unity UI Elements
**File:** `Solitaire.unity` scene

- [ ] Delete Canvas GameObject (GameUI)
- [ ] Delete TopPanel (score, timer, moves text)
- [ ] Delete BottomPanel (buttons)
- [ ] Delete WinPanel
- [ ] Delete UIPositioner GameObject
- [ ] Keep only: Game Manager, Card objects, Piles, Camera

**Expected Result:** Unity scene only contains game objects (cards, piles), no UI.

---

### 1.2 Create Unity-to-React Communication Layer
**New File:** `Assets/Scripts/Core/ReactBridge.cs`

```csharp
using UnityEngine;
using System.Runtime.InteropServices;

namespace CardGames.Core
{
    /// <summary>
    /// Bridge for communicating between Unity and React Native WebView
    /// Sends game events to React, receives actions from React
    /// </summary>
    public class ReactBridge : MonoBehaviour
    {
        private static ReactBridge instance;
        public static ReactBridge Instance => instance;

        [DllImport("__Internal")]
        private static extern void SendMessageToReact(string message);

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Send JSON message to React Native
        /// </summary>
        public void SendToReact(string eventType, string payload)
        {
            string json = $"{{\"type\":\"{eventType}\",\"payload\":{payload}}}";
            Debug.Log($"[ReactBridge] Sending to React: {json}");

            #if UNITY_WEBGL && !UNITY_EDITOR
                SendMessageToReact(json);
            #else
                Debug.Log("[ReactBridge] (WebGL only, simulated in editor)");
            #endif
        }

        /// <summary>
        /// Called by React Native via WebView (window.unityInstance.SendMessage)
        /// </summary>
        public void ReceiveFromReact(string json)
        {
            Debug.Log($"[ReactBridge] Received from React: {json}");

            // Parse and broadcast to game managers
            BroadcastMessage("OnReactMessage", json, SendMessageOptions.DontRequireReceiver);
        }
    }

    // Event payload classes
    [System.Serializable]
    public class GameStatePayload
    {
        public int score;
        public int moves;
        public string time;
    }

    [System.Serializable]
    public class GameEndPayload
    {
        public bool won;
        public int finalScore;
        public string finalTime;
    }
}
```

**Actions:**
- [ ] Create ReactBridge.cs
- [ ] Add ReactBridge GameObject to scene
- [ ] Test communication in editor (logs only)

---

### 1.3 Update SolitaireGameManager to Send Events
**File:** `Assets/Scripts/Solitaire/SolitaireGameManager.cs`

**Changes needed:**

```csharp
// Add field
private ReactBridge reactBridge;

// In Start()
void Start()
{
    reactBridge = ReactBridge.Instance;
    // ... existing code
}

// Update score changed event
private void OnScoreChanged(int newScore)
{
    score = newScore;

    // Send to React
    if (reactBridge != null)
    {
        string payload = $"{{\"score\":{score},\"moves\":{moves},\"time\":\"{GetFormattedTime()}\"}}";
        reactBridge.SendToReact("gameState", payload);
    }
}

// Update moves changed event
private void OnMovesChanged(int newMoves)
{
    moves = newMoves;

    // Send to React
    if (reactBridge != null)
    {
        string payload = $"{{\"score\":{score},\"moves\":{moves},\"time\":\"{GetFormattedTime()}\"}}";
        reactBridge.SendToReact("gameState", payload);
    }
}

// Update game end event
private void OnGameEnd(bool won)
{
    if (reactBridge != null)
    {
        string payload = $"{{\"won\":{won.ToString().ToLower()},\"finalScore\":{score},\"finalTime\":\"{GetFormattedTime()}\"}}";
        reactBridge.SendToReact("gameEnd", payload);
    }
}

// Add method to receive React commands
private void OnReactMessage(string json)
{
    // Parse JSON and handle actions
    if (json.Contains("newGame"))
    {
        InitializeGame();
    }
    else if (json.Contains("restart"))
    {
        RestartGame();
    }
    else if (json.Contains("undo"))
    {
        UndoLastMove();
    }
}

// Helper to format time
private string GetFormattedTime()
{
    int elapsed = Mathf.FloorToInt(Time.time - gameStartTime);
    int minutes = elapsed / 60;
    int seconds = elapsed % 60;
    return $"{minutes:00}:{seconds:00}";
}
```

**Actions:**
- [ ] Add ReactBridge reference
- [ ] Send gameState updates (score, moves, time)
- [ ] Send gameEnd event
- [ ] Implement OnReactMessage handler
- [ ] Remove SolitaireUIManager.cs (no longer needed)
- [ ] Remove UIPositioner.cs (no longer needed)

---

## 📋 Phase 2: React Native UI Framework (3-4 hours)

### 2.1 Create Reusable UI Components

**File:** `mobile-app/src/components/game/GameHeader.tsx`

```typescript
import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

interface GameHeaderProps {
  score: number;
  moves: number;
  time: string;
}

export const GameHeader: React.FC<GameHeaderProps> = ({ score, moves, time }) => {
  return (
    <View style={styles.container}>
      <View style={styles.stat}>
        <Text style={styles.label}>Score</Text>
        <Text style={styles.value}>{score}</Text>
      </View>
      <View style={styles.stat}>
        <Text style={styles.label}>Moves</Text>
        <Text style={styles.value}>{moves}</Text>
      </View>
      <View style={styles.stat}>
        <Text style={styles.label}>Time</Text>
        <Text style={styles.value}>{time}</Text>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    padding: 16,
    backgroundColor: '#2c3e50',
  },
  stat: {
    alignItems: 'center',
  },
  label: {
    color: '#95a5a6',
    fontSize: 12,
    marginBottom: 4,
  },
  value: {
    color: '#ecf0f1',
    fontSize: 20,
    fontWeight: 'bold',
  },
});
```

**File:** `mobile-app/src/components/game/GameActions.tsx`

```typescript
import React from 'react';
import { View, TouchableOpacity, Text, StyleSheet } from 'react-native';

interface GameActionsProps {
  onNewGame: () => void;
  onRestart: () => void;
  onUndo: () => void;
  orientation: 'portrait' | 'landscape';
}

export const GameActions: React.FC<GameActionsProps> = ({
  onNewGame,
  onRestart,
  onUndo,
  orientation,
}) => {
  const isLandscape = orientation === 'landscape';

  return (
    <View style={[styles.container, isLandscape && styles.containerLandscape]}>
      <TouchableOpacity style={styles.button} onPress={onNewGame}>
        <Text style={styles.buttonText}>New Game</Text>
      </TouchableOpacity>
      <TouchableOpacity style={styles.button} onPress={onRestart}>
        <Text style={styles.buttonText}>Restart</Text>
      </TouchableOpacity>
      <TouchableOpacity style={styles.button} onPress={onUndo}>
        <Text style={styles.buttonText}>Undo</Text>
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    padding: 12,
    backgroundColor: '#34495e',
  },
  containerLandscape: {
    flexDirection: 'column',
    padding: 8,
  },
  button: {
    backgroundColor: '#3498db',
    paddingHorizontal: 20,
    paddingVertical: 10,
    borderRadius: 8,
  },
  buttonText: {
    color: '#fff',
    fontWeight: '600',
    fontSize: 14,
  },
});
```

**File:** `mobile-app/src/components/game/WinModal.tsx`

```typescript
import React from 'react';
import { Modal, View, Text, TouchableOpacity, StyleSheet } from 'react-native';

interface WinModalProps {
  visible: boolean;
  finalScore: number;
  finalTime: string;
  onNewGame: () => void;
  onClose: () => void;
}

export const WinModal: React.FC<WinModalProps> = ({
  visible,
  finalScore,
  finalTime,
  onNewGame,
  onClose,
}) => {
  return (
    <Modal
      visible={visible}
      transparent
      animationType="fade"
      onRequestClose={onClose}
    >
      <View style={styles.overlay}>
        <View style={styles.modal}>
          <Text style={styles.title}>🎉 You Won! 🎉</Text>
          <Text style={styles.stat}>Final Score: {finalScore}</Text>
          <Text style={styles.stat}>Time: {finalTime}</Text>

          <View style={styles.buttons}>
            <TouchableOpacity style={styles.button} onPress={onNewGame}>
              <Text style={styles.buttonText}>New Game</Text>
            </TouchableOpacity>
            <TouchableOpacity style={[styles.button, styles.buttonSecondary]} onPress={onClose}>
              <Text style={styles.buttonText}>Close</Text>
            </TouchableOpacity>
          </View>
        </View>
      </View>
    </Modal>
  );
};

const styles = StyleSheet.create({
  overlay: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.7)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  modal: {
    backgroundColor: '#fff',
    borderRadius: 16,
    padding: 24,
    width: '80%',
    maxWidth: 400,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    textAlign: 'center',
    marginBottom: 20,
  },
  stat: {
    fontSize: 18,
    textAlign: 'center',
    marginBottom: 8,
  },
  buttons: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    marginTop: 20,
  },
  button: {
    backgroundColor: '#3498db',
    paddingHorizontal: 24,
    paddingVertical: 12,
    borderRadius: 8,
  },
  buttonSecondary: {
    backgroundColor: '#95a5a6',
  },
  buttonText: {
    color: '#fff',
    fontWeight: '600',
  },
});
```

**Actions:**
- [ ] Create components/game/ folder
- [ ] Implement GameHeader component
- [ ] Implement GameActions component
- [ ] Implement WinModal component

---

### 2.2 Create Unity WebView Hook

**File:** `mobile-app/src/hooks/useUnityGame.ts`

```typescript
import { useRef, useState, useCallback } from 'react';
import { WebView } from 'react-native-webview';

interface GameState {
  score: number;
  moves: number;
  time: string;
}

interface GameEndData {
  won: boolean;
  finalScore: number;
  finalTime: string;
}

interface UnityMessage {
  type: string;
  payload: any;
}

export const useUnityGame = () => {
  const webViewRef = useRef<WebView>(null);
  const [gameState, setGameState] = useState<GameState>({
    score: 0,
    moves: 0,
    time: '00:00',
  });
  const [gameEndData, setGameEndData] = useState<GameEndData | null>(null);

  // Handle messages from Unity
  const handleUnityMessage = useCallback((event: any) => {
    try {
      const message: UnityMessage = JSON.parse(event.nativeEvent.data);

      console.log('[Unity Message]', message);

      switch (message.type) {
        case 'gameState':
          setGameState(message.payload);
          break;

        case 'gameEnd':
          setGameEndData(message.payload);
          break;

        default:
          console.warn('[Unity] Unknown message type:', message.type);
      }
    } catch (error) {
      console.error('[Unity] Failed to parse message:', error);
    }
  }, []);

  // Send action to Unity
  const sendToUnity = useCallback((action: string, data?: any) => {
    const message = JSON.stringify({ action, data });
    const script = `
      if (window.unityInstance) {
        window.unityInstance.SendMessage('ReactBridge', 'ReceiveFromReact', '${message}');
      }
    `;
    webViewRef.current?.injectJavaScript(script);
  }, []);

  // Game actions
  const newGame = useCallback(() => {
    sendToUnity('newGame');
    setGameEndData(null);
  }, [sendToUnity]);

  const restart = useCallback(() => {
    sendToUnity('restart');
    setGameEndData(null);
  }, [sendToUnity]);

  const undo = useCallback(() => {
    sendToUnity('undo');
  }, [sendToUnity]);

  return {
    webViewRef,
    gameState,
    gameEndData,
    handleUnityMessage,
    newGame,
    restart,
    undo,
  };
};
```

**Actions:**
- [ ] Create hooks/ folder
- [ ] Implement useUnityGame hook
- [ ] Add TypeScript interfaces

---

### 2.3 Create Solitaire Game Screen

**File:** `mobile-app/src/screens/SolitaireScreen.tsx`

```typescript
import React, { useState, useEffect } from 'react';
import { View, StyleSheet, Dimensions } from 'react-native';
import { WebView } from 'react-native-webview';
import { useUnityGame } from '../hooks/useUnityGame';
import { GameHeader } from '../components/game/GameHeader';
import { GameActions } from '../components/game/GameActions';
import { WinModal } from '../components/game/WinModal';

export const SolitaireScreen: React.FC = () => {
  const [orientation, setOrientation] = useState<'portrait' | 'landscape'>('portrait');
  const { webViewRef, gameState, gameEndData, handleUnityMessage, newGame, restart, undo } = useUnityGame();

  // Detect orientation changes
  useEffect(() => {
    const updateOrientation = () => {
      const { width, height } = Dimensions.get('window');
      setOrientation(width > height ? 'landscape' : 'portrait');
    };

    const subscription = Dimensions.addEventListener('change', updateOrientation);
    updateOrientation();

    return () => subscription?.remove();
  }, []);

  const isLandscape = orientation === 'landscape';

  return (
    <View style={styles.container}>
      {/* Header - Top in portrait, Left in landscape */}
      {!isLandscape && (
        <GameHeader
          score={gameState.score}
          moves={gameState.moves}
          time={gameState.time}
        />
      )}

      <View style={[styles.gameRow, isLandscape && styles.gameRowLandscape]}>
        {/* Left panel in landscape */}
        {isLandscape && (
          <View style={styles.sidePanel}>
            <GameHeader
              score={gameState.score}
              moves={gameState.moves}
              time={gameState.time}
            />
          </View>
        )}

        {/* Unity Game */}
        <View style={styles.gameContainer}>
          <WebView
            ref={webViewRef}
            source={{ uri: 'http://localhost:8081/unity/index.html' }}
            style={styles.webview}
            onMessage={handleUnityMessage}
            javaScriptEnabled
            domStorageEnabled
            allowsInlineMediaPlayback
          />
        </View>

        {/* Right panel in landscape */}
        {isLandscape && (
          <View style={styles.sidePanel}>
            <GameActions
              onNewGame={newGame}
              onRestart={restart}
              onUndo={undo}
              orientation={orientation}
            />
          </View>
        )}
      </View>

      {/* Actions - Bottom in portrait */}
      {!isLandscape && (
        <GameActions
          onNewGame={newGame}
          onRestart={restart}
          onUndo={undo}
          orientation={orientation}
        />
      )}

      {/* Win Modal */}
      {gameEndData?.won && (
        <WinModal
          visible={true}
          finalScore={gameEndData.finalScore}
          finalTime={gameEndData.finalTime}
          onNewGame={newGame}
          onClose={() => {}}
        />
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#1a1a1a',
  },
  gameRow: {
    flex: 1,
  },
  gameRowLandscape: {
    flexDirection: 'row',
  },
  gameContainer: {
    flex: 1,
  },
  sidePanel: {
    width: 120,
    backgroundColor: '#2c3e50',
    justifyContent: 'center',
  },
  webview: {
    flex: 1,
    backgroundColor: '#1a1a1a',
  },
});
```

**Actions:**
- [ ] Create screens/ folder
- [ ] Implement SolitaireScreen
- [ ] Add responsive layout logic
- [ ] Handle orientation changes

---

## 📋 Phase 3: Integration & Testing (2-3 hours)

### 3.1 Update Unity WebGL Template

**File:** `unity-games/CardGames/Assets/WebGLTemplates/Default/index.html`

Add this JavaScript to enable React communication:

```html
<script>
  // Store Unity instance globally
  window.unityInstance = null;

  // Called by Unity when ready
  function onUnityLoaded(instance) {
    window.unityInstance = instance;
    console.log('[Unity] Loaded and ready');
  }

  // Called by Unity to send messages to React
  window.SendMessageToReact = function(message) {
    if (window.ReactNativeWebView) {
      window.ReactNativeWebView.postMessage(message);
    } else {
      console.log('[Unity→React]', message);
    }
  };

  // Initialize Unity
  createUnityInstance(document.querySelector("#unity-canvas"), {
    dataUrl: "Build/Build.data",
    frameworkUrl: "Build/Build.framework.js",
    codeUrl: "Build/Build.wasm",
    streamingAssetsUrl: "StreamingAssets",
    companyName: "DefaultCompany",
    productName: "CardGames",
    productVersion: "1.0",
  }).then(onUnityLoaded);
</script>
```

**Actions:**
- [ ] Update WebGL template
- [ ] Add message passing scripts
- [ ] Test communication

---

### 3.2 Update Navigation

**File:** `mobile-app/app/(tabs)/index.tsx`

```typescript
import { SolitaireScreen } from '../../src/screens/SolitaireScreen';

export default function HomeScreen() {
  return <SolitaireScreen />;
}
```

**Actions:**
- [ ] Update home screen to use SolitaireScreen
- [ ] Remove old Unity iframe code

---

### 3.3 Testing Checklist

**Portrait Mode:**
- [ ] Header at top shows score/moves/time
- [ ] Unity game in center
- [ ] Actions at bottom (New Game, Restart, Undo)
- [ ] Win modal appears on game completion
- [ ] All buttons send commands to Unity

**Landscape Mode:**
- [ ] Header on left side
- [ ] Unity game in center
- [ ] Actions on right side
- [ ] Layout switches smoothly on rotation

**Unity Communication:**
- [ ] Score updates from Unity → React
- [ ] Moves updates from Unity → React
- [ ] Timer updates from Unity → React
- [ ] Win event triggers modal
- [ ] New Game button → Unity
- [ ] Restart button → Unity
- [ ] Undo button → Unity

---

## 📋 Phase 4: Build & Deploy (1 hour)

### 4.1 Build Unity WebGL

```bash
# In Unity Editor
# File → Build Settings → WebGL → Build
# Output to: unity-games/Build/
```

**Actions:**
- [ ] Build Unity project (no UI)
- [ ] Copy Build folder to mobile-app/public/unity/
- [ ] Test in browser first

---

### 4.2 Test & Deploy

```bash
cd mobile-app
npm run build:web
npm run deploy
```

**Actions:**
- [ ] Build React Native web version
- [ ] Deploy to GitHub Pages
- [ ] Test on mobile browser
- [ ] Test on real device

---

## 📋 Phase 5: Documentation (30 mins)

### 5.1 Create Architecture Docs

**File:** `GAME_ARCHITECTURE.md`

Document:
- [ ] How to add a new game
- [ ] Unity-React communication protocol
- [ ] Reusable component library
- [ ] Testing procedures

---

## 🎮 Future Games: Reusable Pattern

For each new game (Blackjack, Poker, etc.):

**Unity Side:**
1. Create game scene (no UI)
2. Implement game logic
3. Send events via ReactBridge
4. Receive actions via OnReactMessage

**React Native Side:**
1. Create {GameName}Screen.tsx
2. Reuse GameHeader, GameActions, WinModal
3. Add game-specific controls (e.g., bet slider)
4. Use useUnityGame hook

**Example: Adding Blackjack**
- Unity: Renders cards, table, handles game logic
- React: Bet slider, Balance display, Hit/Stand buttons
- Communication: Bet amounts, actions, balance updates

---

## ✅ Success Criteria

- [ ] Solitaire works with hybrid architecture
- [ ] Portrait and landscape layouts work perfectly
- [ ] Unity has ZERO UI elements
- [ ] React Native handles all UI rendering
- [ ] Communication is reliable and fast
- [ ] Architecture is documented
- [ ] Easy to add new games

---

## 📊 Estimated Timeline

**Total: 8-11 hours**

| Phase | Time | Description |
|-------|------|-------------|
| Phase 1 | 2-3h | Unity cleanup + ReactBridge |
| Phase 2 | 3-4h | React Native UI components |
| Phase 3 | 2-3h | Integration & testing |
| Phase 4 | 1h | Build & deploy |
| Phase 5 | 30m | Documentation |

---

## 🚀 Ready to Start?

Start with Phase 1.1: Strip Unity UI elements from the scene!
