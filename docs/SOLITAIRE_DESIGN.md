# Solitaire Game Design

Classic Klondike Solitaire implementation in Unity.

## Game Rules

### Objective
Move all cards to the four foundation piles, sorted by suit from Ace to King.

### Layout

```
┌─────────────────────────────────────────────────────┐
│  [Stock] [Waste]              [F♠] [F♥] [F♦] [F♣]  │  Foundation (4 piles)
│                                                      │
│  [T1]    [T2]    [T3]    [T4]    [T5]    [T6]  [T7]│  Tableau (7 piles)
│  │ │     │ │     │ │     │ │     │ │     │ │   │ │ │
│  └─┘     └─┘     └─┘     └─┘     └─┘     └─┘   └─┘ │
└─────────────────────────────────────────────────────┘
```

### Setup
- **Stock:** 24 cards face-down
- **Tableau:** 7 piles
  - Pile 1: 1 card (top face-up)
  - Pile 2: 2 cards (top face-up)
  - Pile 3: 3 cards (top face-up)
  - ...
  - Pile 7: 7 cards (top face-up)
- **Foundation:** Empty (4 piles for each suit)
- **Waste:** Empty

### Valid Moves

1. **Stock → Waste:**
   - Draw 1 or 3 cards from stock to waste

2. **Waste → Foundation:**
   - Aces start foundation piles
   - Build up by suit (A, 2, 3, ..., K)

3. **Waste → Tableau:**
   - Build down by alternating colors
   - Example: Red 7 on Black 8

4. **Tableau → Foundation:**
   - Same rules as Waste → Foundation

5. **Tableau → Tableau:**
   - Move single cards or sequences
   - Build down by alternating colors
   - Only Kings can move to empty tableau piles

6. **Flip Cards:**
   - When top card is moved, flip next card face-up

### Scoring

- Move to foundation: +10 points
- Move from waste to tableau: +5 points
- Flip tableau card: +5 points
- Complete game: +bonus based on time
- Move from foundation to tableau: -15 points (penalty)

### Winning
All 52 cards moved to foundation piles in correct order.

## Unity Implementation

### Core Classes

#### Card.cs
```csharp
public class Card : MonoBehaviour {
    public enum Suit { Spades, Hearts, Diamonds, Clubs }
    public enum Rank { Ace = 1, Two, Three, ..., King = 13 }

    public Suit suit;
    public Rank rank;
    public bool isFaceUp;
    public Color color; // Red or Black

    public void Flip();
    public bool CanStackOn(Card other);
    public bool IsNextInSequence(Card other);
}
```

#### Pile.cs
```csharp
public class Pile : MonoBehaviour {
    public enum PileType { Stock, Waste, Tableau, Foundation }

    public PileType type;
    public List<Card> cards;

    public bool CanAccept(Card card);
    public void AddCard(Card card);
    public Card RemoveTopCard();
    public void FlipTopCard();
}
```

#### GameManager.cs
```csharp
public class GameManager : MonoBehaviour {
    private List<Card> deck;
    private Pile[] tableauPiles;
    private Pile[] foundationPiles;
    private Pile stockPile;
    private Pile wastePile;

    private int score;
    private int moves;
    private float gameTime;

    public void StartGame();
    public void MoveCard(Card card, Pile destination);
    public void DrawFromStock();
    public void CheckWinCondition();
    public void SendResultToRN(bool won);
}
```

#### InputManager.cs
```csharp
public class InputManager : MonoBehaviour {
    private Card selectedCard;
    private Pile selectedPile;

    void Update() {
        HandleTouch();
        HandleMouse();
    }

    private void SelectCard(Card card);
    private void TryMoveCard(Card card, Pile pile);
    private void ShowValidMoves();
}
```

### Card Movement System

#### Drag & Drop
```csharp
public class DraggableCard : MonoBehaviour {
    private Vector3 offset;
    private bool isDragging;

    void OnMouseDown() {
        // Start drag
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag() {
        // Update position
        if (isDragging) {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    void OnMouseUp() {
        // Try to place on pile
        isDragging = false;
        Pile targetPile = GetPileUnderMouse();

        if (targetPile != null && targetPile.CanAccept(this.card)) {
            MoveToP ile(targetPile);
        } else {
            ReturnToOriginalPosition();
        }
    }
}
```

### Animation System

```csharp
public class CardAnimator : MonoBehaviour {
    public void AnimateMove(Card card, Vector3 target, float duration) {
        StartCoroutine(MoveCoroutine(card, target, duration));
    }

    private IEnumerator MoveCoroutine(Card card, Vector3 target, float duration) {
        Vector3 start = card.transform.position;
        float elapsed = 0;

        while (elapsed < duration) {
            card.transform.position = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        card.transform.position = target;
    }

    public void AnimateFlip(Card card) {
        StartCoroutine(FlipCoroutine(card));
    }

    private IEnumerator FlipCoroutine(Card card) {
        // 3D flip animation
        float elapsed = 0;
        float duration = 0.3f;

        while (elapsed < duration) {
            float angle = Mathf.Lerp(0, 180, elapsed / duration);
            card.transform.rotation = Quaternion.Euler(0, angle, 0);

            if (elapsed > duration / 2 && !card.isFaceUp) {
                card.Flip();
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
```

### Auto-Complete Feature

```csharp
public class AutoComplete : MonoBehaviour {
    public bool CanAutoComplete(GameState state) {
        // Check if all tableau cards are face-up
        // and can be moved to foundation
        return AllTableauCardsFaceUp() && NoMovesInTableau();
    }

    public IEnumerator AutoCompleteSequence() {
        while (cardsRemaining > 0) {
            Card card = FindNextCardForFoundation();
            yield return MoveCardToFoundation(card);
            yield return new WaitForSeconds(0.2f);
        }

        GameWon();
    }
}
```

## UI/UX Features

### Visual Feedback
- Highlight valid drop zones when dragging
- Glow effect on selected card
- Particle effects on foundation completion
- Smooth animations for all moves

### Controls
- **Tap/Click:** Select card
- **Drag:** Move card
- **Double-tap:** Auto-move to foundation
- **Undo button:** Reverse last move
- **Hint button:** Show possible move
- **Menu button:** Pause/quit

### HUD Elements
```
┌─────────────────────────────┐
│  Score: 150    Moves: 23    │
│  Time: 02:45   [Undo] [Hint]│
└─────────────────────────────┘
```

## Bridge Communication

### Messages to React Native

```csharp
// Game started
SendMessageToRN("GameStarted", "{}");

// Game complete
var result = new {
    won = true,
    score = 250,
    moves = 145,
    timeSeconds = 234,
    coinsEarned = 100
};
SendMessageToRN("GameComplete", JsonUtility.ToJson(result));

// Game paused
SendMessageToRN("GamePaused", "{}");
```

### Messages from React Native

```csharp
public void ReceiveMessage(string message) {
    var data = JsonUtility.FromJson<RNMessage>(message);

    switch (data.type) {
        case "StartGame":
            StartNewGame();
            break;
        case "PauseGame":
            PauseGame();
            break;
        case "ResumeGame":
            ResumeGame();
            break;
        case "QuitGame":
            QuitToMenu();
            break;
    }
}
```

## Development Phases

### Phase 1: Basic Setup ✓
- [x] Unity project created
- [x] Project structure
- [x] Card sprites imported

### Phase 2: Core Mechanics
- [ ] Card class implementation
- [ ] Pile system
- [ ] Deck creation and shuffling
- [ ] Initial deal

### Phase 3: Game Logic
- [ ] Movement validation
- [ ] Foundation rules
- [ ] Tableau rules
- [ ] Stock/waste cycling

### Phase 4: Input & Interaction
- [ ] Drag and drop
- [ ] Click/tap selection
- [ ] Double-tap auto-move
- [ ] Visual feedback

### Phase 5: Features
- [ ] Scoring system
- [ ] Timer
- [ ] Undo system
- [ ] Hint system
- [ ] Auto-complete

### Phase 6: Polish
- [ ] Animations
- [ ] Sound effects
- [ ] Particle effects
- [ ] Win/lose screens

### Phase 7: Integration
- [ ] React Native bridge
- [ ] Message passing
- [ ] Build exports (iOS/Android/WebGL)
- [ ] Testing

## Testing Checklist

- [ ] All cards can be moved correctly
- [ ] Win condition triggers properly
- [ ] Score calculates correctly
- [ ] Undo works for all moves
- [ ] No invalid moves allowed
- [ ] Animations smooth on all devices
- [ ] Bridge communication works
- [ ] Performance is good (60 FPS)

## Performance Targets

- **Frame Rate:** 60 FPS
- **Load Time:** < 3 seconds
- **Memory:** < 150 MB
- **Build Size:**
  - Android: < 50 MB
  - iOS: < 50 MB
  - WebGL: < 15 MB
