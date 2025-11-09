# Win Panel Setup Guide

Quick guide to set up the win panel UI elements in Unity and connect them to the UIManager.

## What You Need

You already created a WinPanel. Now you need to add text and buttons inside it.

---

## Step 1: Position Win Panel

1. Select `WinPanel` in Hierarchy
2. Rect Transform:
   - Anchor: **Stretch All** (bottom-right preset + hold Alt+Shift)
   - Left: 0, Right: 0, Top: 0, Bottom: 0
   - This makes it cover the full screen

3. Image component:
   - Color: Semi-transparent black (R:0, G:0, B:0, A:200)
   - This creates a dark overlay

---

## Step 2: Create Content Panel

1. Right-click `WinPanel` → UI → Panel
2. Rename to `WinContent`
3. Rect Transform:
   - Anchor: Center
   - Width: 400
   - Height: 300
4. Image:
   - Color: White or light gray (R:240, G:240, B:240, A:255)

---

## Step 3: Add Win Message Text

1. Right-click `WinContent` → UI → Text - TextMeshPro
2. Rename to `WinMessageText`
3. Rect Transform:
   - Anchor: Top Center
   - Pos X: 0, Pos Y: -50
   - Width: 350, Height: 60
4. TextMeshPro:
   - Text: "🎉 YOU WON! 🎉"
   - Font Size: 36
   - Color: Gold or Green
   - Alignment: Center
   - Font Style: Bold

---

## Step 4: Add Score Text

1. Right-click `WinContent` → UI → Text - TextMeshPro
2. Rename to `WinScoreText`
3. Rect Transform:
   - Anchor: Center
   - Pos X: 0, Pos Y: 20
   - Width: 300, Height: 50
4. TextMeshPro:
   - Text: "Final Score: 0"
   - Font Size: 24
   - Color: Black
   - Alignment: Center

---

## Step 5: Add Time Text

1. Right-click `WinContent` → UI → Text - TextMeshPro
2. Rename to `WinTimeText`
3. Rect Transform:
   - Anchor: Center
   - Pos X: 0, Pos Y: -30
   - Width: 300, Height: 50
4. TextMeshPro:
   - Text: "Time: 00:00"
   - Font Size: 24
   - Color: Black
   - Alignment: Center

---

## Step 6: Add New Game Button

1. Right-click `WinContent` → UI → Button - TextMeshPro
2. Rename to `WinNewGameButton`
3. Rect Transform:
   - Anchor: Bottom Center
   - Pos X: -80, Pos Y: 40
   - Width: 140, Height: 50
4. Select `Text (TMP)` child:
   - Text: "New Game"
   - Font Size: 18

---

## Step 7: Add Close Button

1. Right-click `WinContent` → UI → Button - TextMeshPro
2. Rename to `WinCloseButton`
3. Rect Transform:
   - Anchor: Bottom Center
   - Pos X: 80, Pos Y: 40
   - Width: 140, Height: 50
4. Select `Text (TMP)` child:
   - Text: "Close"
   - Font Size: 18

---

## Step 8: Connect to UIManager

1. Select `UIManager` in Hierarchy
2. In Inspector, find **Win Popup** section
3. Drag and drop:
   - **Win Panel** → `WinPanel` (parent GameObject)
   - **Win Message Text** → `WinMessageText`
   - **Win Score Text** → `WinScoreText`
   - **Win Time Text** → `WinTimeText`
   - **Win New Game Button** → `WinNewGameButton`
   - **Win Close Button** → `WinCloseButton`

---

## Step 9: Hide Win Panel Initially

**Important:** The panel should be hidden at game start.

1. Select `WinPanel` in Hierarchy
2. In Inspector, **uncheck the box** next to the GameObject name (top-left)
3. This disables the panel (it will be shown by code when you win)

---

## Visual Layout

Your hierarchy should look like:

```
GameUI (Canvas)
├── TopPanel
│   ├── ScoreText
│   ├── MovesText
│   └── TimerText
├── BottomPanel
│   ├── NewGameButton
│   ├── RestartButton
│   └── UndoButton
└── WinPanel (DISABLED initially)
    └── WinContent
        ├── WinMessageText
        ├── WinScoreText
        ├── WinTimeText
        ├── WinNewGameButton
        └── WinCloseButton
```

---

## Testing

1. **Play the game in Unity Editor**
2. **Win the game** (move all cards to foundations)
3. **Win panel should appear** with:
   - Congratulations message
   - Your final score
   - Time taken
   - New Game button (starts fresh game)
   - Close button (hides panel)

---

## Behavior

### When You Win:
- Timer stops
- Win panel appears with score and time
- Background overlay dims the game

### New Game Button (on win panel):
- Hides win panel
- Creates fresh shuffled game
- Resets timer

### Close Button (on win panel):
- Hides win panel
- Game stays won
- Can continue looking at the completed game

### New Game Button (bottom panel):
- Creates fresh shuffled game
- Hides win panel if showing

### Restart Button (bottom panel):
- Restarts with **same deck order**
- Hides win panel if showing
- Good for practicing a specific game

---

## Styling Tips (Optional)

### Make it Prettier:

**Win Content Panel:**
- Add rounded corners with a sprite
- Add shadow/outline

**Button Colors:**
- Normal: Light blue
- Highlighted: Brighter blue
- Pressed: Dark blue

**Win Message:**
- Add animation (scale up when appearing)
- Try different emojis: 🏆, ⭐, 🎊

**Background:**
- Try different transparency values
- Add blur effect (needs shader)

---

## Common Issues

### Win panel doesn't appear:
- Check Console for "🎉 YOU WON!" message
- Make sure UIManager has all references assigned
- Check WinPanel is initially disabled

### Buttons don't work:
- EventSystem must exist in scene
- Check button OnClick has no old listeners
- Check SolitaireUIManager component is on UIManager

### Text doesn't update:
- Make sure UIManager references are correct
- Check you're dragging the TextMeshPro object, not its parent

---

## All Done!

Your win panel is now set up. Complete a game to see it in action!

### New Game vs Restart:
- **New Game** = Fresh shuffled deck (different game)
- **Restart** = Same initial deck (practice same game)

This distinction makes your game more useful for learning strategies! 🎮
