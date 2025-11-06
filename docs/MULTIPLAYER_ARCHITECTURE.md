# Multiplayer Architecture

## Overview

For multiplayer card games (Poker, Blackjack), we use a **backend-authoritative** architecture where the server holds the source of truth.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                   Firebase Backend                      │
│  ┌──────────────────────────────────────────────────┐  │
│  │          Cloud Firestore (Real-time DB)          │  │
│  │  ┌────────────────────────────────────────────┐  │  │
│  │  │  Game State Collection                     │  │  │
│  │  │  {                                         │  │  │
│  │  │    gameId: "abc123",                       │  │  │
│  │  │    players: [...],                         │  │  │
│  │  │    deck: [...],                            │  │  │
│  │  │    currentPlayer: "player1",               │  │  │
│  │  │    pot: 100,                               │  │  │
│  │  │    communityCards: [...]                   │  │  │
│  │  │  }                                         │  │  │
│  │  └────────────────────────────────────────────┘  │  │
│  └──────────────────────────────────────────────────┘  │
│                                                         │
│  ┌──────────────────────────────────────────────────┐  │
│  │          Cloud Functions (Game Logic)            │  │
│  │  - validateMove()                                │  │
│  │  - dealCards()                                   │  │
│  │  - calculateWinner()                             │  │
│  │  - handleBet()                                   │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
                      ↕                  ↕
        ┌─────────────────────┐  ┌─────────────────────┐
        │    Player 1 Device  │  │    Player 2 Device  │
        │  ┌───────────────┐  │  │  ┌───────────────┐  │
        │  │ React Native  │  │  │  │ React Native  │  │
        │  │  - Auth       │  │  │  │  - Auth       │  │
        │  │  - Firebase   │  │  │  │  - Firebase   │  │
        │  │  - Websocket  │  │  │  │  - Websocket  │  │
        │  └───────┬───────┘  │  │  └───────┬───────┘  │
        │          ↕          │  │          ↕          │
        │  ┌───────────────┐  │  │  ┌───────────────┐  │
        │  │  Unity Game   │  │  │  │  Unity Game   │  │
        │  │  (Renderer)   │  │  │  │  (Renderer)   │  │
        │  └───────────────┘  │  │  └───────────────┘  │
        └─────────────────────┘  └─────────────────────┘
```

## Game Flow Example: Poker

### 1. Game Creation

```typescript
// React Native - HomeScreen
const createPokerGame = async () => {
  const gameRef = await firestore.collection('games').add({
    type: 'poker',
    host: currentUserId,
    players: [
      { userId: currentUserId, chips: 1000, status: 'ready' }
    ],
    status: 'waiting', // waiting, playing, finished
    maxPlayers: 6,
    smallBlind: 5,
    bigBlind: 10,
    createdAt: serverTimestamp()
  });

  // Navigate to lobby
  navigation.navigate('GameLobby', { gameId: gameRef.id });
};
```

### 2. Players Join

```typescript
// React Native - GameLobby
const joinGame = async (gameId: string) => {
  await firestore.collection('games').doc(gameId).update({
    players: arrayUnion({
      userId: currentUserId,
      chips: 1000,
      status: 'ready'
    })
  });
};

// Listen for player updates
useEffect(() => {
  const unsubscribe = firestore
    .collection('games')
    .doc(gameId)
    .onSnapshot((doc) => {
      const game = doc.data();
      setPlayers(game.players);

      // Start game when enough players
      if (game.players.length >= 2 && game.status === 'waiting') {
        startGame();
      }
    });

  return unsubscribe;
}, [gameId]);
```

### 3. Game Start (Cloud Function)

```javascript
// Firebase Cloud Function
exports.startPokerGame = functions.firestore
  .document('games/{gameId}')
  .onUpdate(async (change, context) => {
    const game = change.after.data();

    if (game.status === 'starting') {
      // Create shuffled deck
      const deck = createAndShuffleDeck();

      // Deal cards to players (2 hole cards each)
      const players = game.players.map((player, index) => ({
        ...player,
        holeCards: [deck[index * 2], deck[index * 2 + 1]]
      }));

      // Update game state
      await change.after.ref.update({
        status: 'playing',
        deck: deck.slice(players.length * 2), // Remaining cards
        players: players,
        currentPlayer: game.players[0].userId,
        currentRound: 'preflop',
        pot: game.smallBlind + game.bigBlind,
        communityCards: []
      });
    }
  });
```

### 4. Player Action

```typescript
// React Native - PokerScreen
const handlePlayerAction = async (action: 'fold' | 'call' | 'raise', amount?: number) => {
  // Send action to Cloud Function
  const result = await functions.httpsCallable('pokerAction')({
    gameId: gameId,
    playerId: currentUserId,
    action: action,
    amount: amount
  });

  if (!result.data.success) {
    alert(result.data.error);
  }
};

// Unity receives command to show animation
const sendActionToUnity = (action: string) => {
  // For iframe/webview
  unityIframe.contentWindow.postMessage({
    type: 'PlayerAction',
    action: action
  }, '*');
};
```

### 5. Cloud Function Validates & Updates

```javascript
// Firebase Cloud Function
exports.pokerAction = functions.https.onCall(async (data, context) => {
  const { gameId, playerId, action, amount } = data;
  const userId = context.auth.uid;

  // Security: Verify it's the current player's turn
  const gameDoc = await firestore.collection('games').doc(gameId).get();
  const game = gameDoc.data();

  if (game.currentPlayer !== userId) {
    return { success: false, error: 'Not your turn!' };
  }

  // Validate action
  switch (action) {
    case 'fold':
      // Remove player from round
      break;
    case 'call':
      // Match current bet
      break;
    case 'raise':
      // Increase bet
      if (amount < game.currentBet * 2) {
        return { success: false, error: 'Raise must be at least 2x current bet' };
      }
      break;
  }

  // Update game state
  const updatedGame = applyAction(game, action, amount);
  await gameDoc.ref.update(updatedGame);

  return { success: true };
});
```

### 6. All Clients Receive Update

```typescript
// React Native - PokerScreen
useEffect(() => {
  // Listen for game state changes
  const unsubscribe = firestore
    .collection('games')
    .doc(gameId)
    .onSnapshot((doc) => {
      const gameState = doc.data();

      // Update local React state
      setPot(gameState.pot);
      setCurrentPlayer(gameState.currentPlayer);
      setCommunityCards(gameState.communityCards);

      // Send to Unity for visual update
      sendGameStateToUnity(gameState);
    });

  return unsubscribe;
}, [gameId]);

const sendGameStateToUnity = (gameState: any) => {
  const message = {
    type: 'UpdateGameState',
    data: JSON.stringify({
      pot: gameState.pot,
      communityCards: gameState.communityCards,
      players: gameState.players.map(p => ({
        id: p.userId,
        chips: p.chips,
        status: p.status,
        // Don't send hole cards of other players!
        holeCards: p.userId === currentUserId ? p.holeCards : null
      }))
    })
  };

  // Send to Unity
  unityIframe?.contentWindow?.postMessage(message, '*');
};
```

### 7. Unity Renders State

```csharp
// Unity - MultiplayerGameManager.cs
public void ReceiveMessageFromRN(string message) {
    var data = JsonUtility.FromJson<MessageData>(message);

    if (data.type == "UpdateGameState") {
        var gameState = JsonUtility.FromJson<GameState>(data.data);

        // Update visuals
        UpdatePot(gameState.pot);
        UpdateCommunityCards(gameState.communityCards);
        UpdatePlayerChips(gameState.players);

        // Highlight current player
        HighlightCurrentPlayer(gameState.currentPlayer);
    }
}
```

## Security Considerations

### 1. **Server-Side Validation**
```javascript
// ❌ NEVER trust client
// Unity should NEVER know other players' cards

// ✅ Server validates everything
exports.pokerAction = functions.https.onCall(async (data, context) => {
  // Verify authentication
  if (!context.auth) {
    throw new functions.https.HttpsError('unauthenticated', 'Must be logged in');
  }

  // Verify it's player's turn
  if (game.currentPlayer !== context.auth.uid) {
    throw new functions.https.HttpsError('permission-denied', 'Not your turn');
  }

  // Validate bet amount
  if (data.amount > player.chips) {
    throw new functions.https.HttpsError('invalid-argument', 'Insufficient chips');
  }

  // All checks passed, proceed
});
```

### 2. **Firestore Security Rules**
```javascript
// firestore.rules
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    match /games/{gameId} {
      // Anyone can read public game info
      allow read: if true;

      // Only players can update their own actions
      allow update: if request.auth != null
        && request.auth.uid in resource.data.players.map(p => p.userId);

      // Game state can only be updated by Cloud Functions
      // (using admin SDK)
    }

    match /games/{gameId}/players/{playerId}/privateData {
      // Players can only see their own cards
      allow read: if request.auth.uid == playerId;
    }
  }
}
```

### 3. **Separate Private Data**
```
games/
  {gameId}/
    - pot: 100
    - communityCards: [...]
    - players: [...]
    - currentPlayer: "player1"

    players/ (subcollection)
      {playerId}/
        privateData/
          - holeCards: ["Ah", "Kd"]  // Only this player can read!
```

## Real-Time Synchronization

### Firebase Realtime Database vs Firestore

**For multiplayer, use Firestore:**
- Real-time listeners
- Offline support
- Better querying
- Easier security rules

```typescript
// Subscribe to game updates
const gameRef = firestore.collection('games').doc(gameId);

gameRef.onSnapshot((snapshot) => {
  const game = snapshot.data();
  // Update UI immediately when any player acts
  updateGameState(game);
});
```

## Latency & Lag Handling

### 1. **Optimistic Updates**
```typescript
// Show action immediately in UI
setMyAction('fold');

// Then send to server
await sendAction('fold');

// If server rejects, rollback
if (!success) {
  setMyAction(null);
  showError('Invalid action');
}
```

### 2. **Turn Timers**
```typescript
// Cloud Function: Auto-fold if player doesn't act
exports.checkTurnTimeout = functions.pubsub
  .schedule('every 1 minutes')
  .onRun(async () => {
    const games = await firestore.collection('games')
      .where('status', '==', 'playing')
      .get();

    games.forEach(async (doc) => {
      const game = doc.data();
      const turnStartTime = game.turnStartedAt.toDate();
      const now = new Date();

      if (now - turnStartTime > 30000) { // 30 seconds
        // Auto-fold current player
        await doc.ref.update({
          players: game.players.map(p =>
            p.userId === game.currentPlayer
              ? { ...p, status: 'folded' }
              : p
          ),
          currentPlayer: getNextPlayer(game)
        });
      }
    });
  });
```

## Unity's Role in Multiplayer

Unity is **purely a renderer** in multiplayer games:

✅ **Unity DOES:**
- Display cards, chips, table
- Animate card dealing, chip movements
- Show player avatars
- Highlight current player
- Show visual effects (winner celebration)
- Provide UI for actions (buttons: Fold, Call, Raise)

❌ **Unity DOES NOT:**
- Store game state
- Know other players' hole cards
- Validate moves
- Calculate winners
- Manage turn order
- Handle bet logic

## Example: Complete Poker Flow

```
1. Host creates game → Firebase
2. Players join → Firebase updates
3. Game starts → Cloud Function deals cards
4. Player 1's turn:
   ├─ Unity: Shows buttons (Fold, Call, Raise)
   ├─ Player clicks "Raise $20"
   ├─ Unity → RN: "PlayerAction: raise, 20"
   ├─ RN → Firebase Cloud Function
   ├─ Function validates (correct turn? has chips? valid raise?)
   ├─ Function updates Firestore
   ├─ Firestore → All RN clients (real-time)
   └─ All RN → Unity: Update display
5. Player 2's turn → repeat
6. Showdown:
   ├─ Cloud Function determines winner
   ├─ Updates chip counts
   ├─ Sends results to all players
   └─ Unity shows winner animation
```

## Cost Considerations

Firebase pricing for multiplayer:

- **Firestore reads:** ~$0.06 per 100K reads
- **Firestore writes:** ~$0.18 per 100K writes
- **Cloud Function invocations:** ~$0.40 per million
- **Real-time listeners:** Included in reads

**Estimated cost per game:**
- 1 poker game (10 minutes, 50 actions)
- ~200 Firestore operations
- ~50 Cloud Function calls
- **Cost: < $0.01 per game**

Very affordable!

## Alternative: Socket.IO + Custom Backend

If you need MORE real-time performance:

```typescript
// Node.js server with Socket.IO
io.on('connection', (socket) => {
  socket.on('playerAction', (data) => {
    // Validate
    // Update game state
    // Broadcast to all players
    io.to(gameRoom).emit('gameUpdate', newState);
  });
});
```

But Firebase is easier and sufficient for turn-based card games!

## Summary

**Multiplayer in our hybrid architecture:**

1. **React Native** handles:
   - Player connections
   - Firebase communication
   - UI outside game (lobby, chat)

2. **Firebase** handles:
   - Game state (source of truth)
   - Game logic (Cloud Functions)
   - Real-time synchronization
   - Security

3. **Unity** handles:
   - Visual representation only
   - Animations
   - Input (button clicks)
   - Reports actions to RN

This gives you:
- ✅ Secure multiplayer (no cheating)
- ✅ Real-time updates
- ✅ Scalable
- ✅ Affordable
- ✅ Easy to maintain

Want me to create a detailed implementation guide for Poker multiplayer? 🎮
