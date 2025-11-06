# Setup UI and Audio in Unity Solitaire

This guide will help you set up the UI canvas with score, moves, timer displays, and the three buttons (New Game, Restart, Undo).

## What We've Added

### ✅ Code Complete

1. **AudioManager.cs** - Handles all sound effects
2. **SolitaireUIManager.cs** - Manages UI elements and button clicks
3. **Audio Integration** - Flip and place sounds in Card.cs and SolitaireGameManager.cs
4. **Undo System** - Full undo functionality in SolitaireGameManager.cs

### 🎨 Unity Setup Needed

You need to create the UI elements in the Unity Editor.

---

## Step 1: Create Audio Manager GameObject

1. **In Unity Scene (Solitaire.unity):**
   - Right-click in Hierarchy → Create Empty
   - Name it: `AudioManager`

2. **Add AudioManager Component:**
   - Select `AudioManager` object
   - In Inspector → Add Component
   - Search for "AudioManager"
   - Add the AudioManager script

3. **Verify Sounds Load:**
   - Click Play
   - Check Console for: `[AudioManager] Loaded 3/3 flip sounds, 3/3 place sounds`
   - If you see 0/3, the audio files aren't in Resources/Sounds properly

---

## Step 2: Create UI Canvas

### 2.1 Create Canvas

1. Right-click in Hierarchy → UI → Canvas
2. Name it: `GameUI`
3. In Canvas component:
   - Render Mode: **Screen Space - Overlay**
   - Pixel Perfect: ✅ Checked

### 2.2 Create Panel for Top UI

1. Right-click `GameUI` → UI → Panel
2. Name it: `TopPanel`
3. In Rect Transform:
   - Anchor: Top stretch (top-left corner preset + hold Alt)
   - Height: 80
   - Position Y: 0

4. Change Panel color (optional):
   - Image component → Color: Semi-transparent dark color (R:0, G:0, B:0, A:180)

---

## Step 3: Add Score, Moves, Timer Text

### 3.1 Install TextMeshPro (If Not Installed)

1. Click on TopPanel → Add Component → TextMeshPro
2. Unity will prompt "Import TMP Essentials" → Click **Import TMP Essentials**
3. Wait for import to complete

### 3.2 Create Score Text

1. Right-click `TopPanel` → UI → Text - TextMeshPro
2. Name it: `ScoreText`
3. In Rect Transform:
   - Anchor: Middle Left
   - Pos X: 20, Pos Y: 0
   - Width: 200, Height: 60

4. In TextMeshPro component:
   - Text: "Score: 0"
   - Font Size: 24
   - Color: White
   - Alignment: Middle Left

### 3.3 Create Moves Text

1. Right-click `TopPanel` → UI → Text - TextMeshPro
2. Name it: `MovesText`
3. In Rect Transform:
   - Anchor: Top Center
   - Pos X: 0, Pos Y: 0
   - Width: 200, Height: 60

4. In TextMeshPro component:
   - Text: "Moves: 0"
   - Font Size: 24
   - Color: White
   - Alignment: Middle Center

### 3.4 Create Timer Text

1. Right-click `TopPanel` → UI → Text - TextMeshPro
2. Name it: `TimerText`
3. In Rect Transform:
   - Anchor: Middle Right
   - Pos X: -20, Pos Y: 0
   - Width: 200, Height: 60

4. In TextMeshPro component:
   - Text: "Time: 00:00"
   - Font Size: 24
   - Color: White
   - Alignment: Middle Right

---

## Step 4: Create Button Panel

### 4.1 Create Bottom Panel

1. Right-click `GameUI` → UI → Panel
2. Name it: `BottomPanel`
3. In Rect Transform:
   - Anchor: Bottom stretch (bottom-left + hold Alt)
   - Height: 80
   - Position Y: 0

4. Change Panel color:
   - Image component → Color: Semi-transparent dark (R:0, G:0, B:0, A:180)

---

## Step 5: Create Buttons

### 5.1 New Game Button

1. Right-click `BottomPanel` → UI → Button - TextMeshPro
2. Name it: `NewGameButton`
3. In Rect Transform:
   - Anchor: Middle Left
   - Pos X: 100, Pos Y: 0
   - Width: 150, Height: 50

4. Expand `NewGameButton` → Select `Text (TMP)` child
5. In TextMeshPro:
   - Text: "New Game"
   - Font Size: 18
   - Alignment: Center

### 5.2 Restart Button

1. Right-click `BottomPanel` → UI → Button - TextMeshPro
2. Name it: `RestartButton`
3. In Rect Transform:
   - Anchor: Center
   - Pos X: 0, Pos Y: 0
   - Width: 150, Height: 50

4. Select `Text (TMP)` child:
   - Text: "Restart"
   - Font Size: 18

### 5.3 Undo Button

1. Right-click `BottomPanel` → UI → Button - TextMeshPro
2. Name it: `UndoButton`
3. In Rect Transform:
   - Anchor: Middle Right
   - Pos X: -100, Pos Y: 0
   - Width: 150, Height: 50

4. Select `Text (TMP)` child:
   - Text: "Undo"
   - Font Size: 18

---

## Step 6: Create UI Manager GameObject

### 6.1 Create GameObject

1. Right-click in Hierarchy → Create Empty
2. Name it: `UIManager`

### 6.2 Add SolitaireUIManager Component

1. Select `UIManager`
2. Inspector → Add Component
3. Search for "SolitaireUIManager"
4. Add the script

### 6.3 Assign References

In SolitaireUIManager component, drag and drop:

**UI Text Elements:**
- Score Text → `ScoreText` (from TopPanel)
- Moves Text → `MovesText` (from TopPanel)
- Timer Text → `TimerText` (from TopPanel)

**Buttons:**
- New Game Button → `NewGameButton` (from BottomPanel)
- Restart Button → `RestartButton` (from BottomPanel)
- Undo Button → `UndoButton` (from BottomPanel)

**Game Manager Reference:**
- Game Manager → `SolitaireGameManager` (from scene)

---

## Step 7: Test Everything

### 7.1 Play in Editor

1. Click Play
2. Check Console for:
   ```
   [AudioManager] Loaded 3/3 flip sounds, 3/3 place sounds
   ```

### 7.2 Test Audio

- **Flip Sound:** Should play when cards flip face-up
- **Place Sound:** Should play when:
  - Drawing from stock pile
  - Placing cards on tableau/foundation

### 7.3 Test UI

- **Score:** Updates when moving cards
- **Moves:** Increments with each move
- **Timer:** Counts up from 00:00

### 7.4 Test Buttons

- **New Game:** Starts fresh game with reshuffled cards
- **Restart:** Restarts with same initial setup
- **Undo:** Reverses last move (try making a few moves then undo)

---

## Troubleshooting

### Issue: No Sounds Playing

**Check:**
1. Audio files in correct location: `Assets/Resources/Sounds/`
2. Files named correctly: `cardFlip1.ogg`, `cardPlace1.ogg`, etc.
3. AudioManager exists in scene
4. Console shows sounds loaded

**Fix:**
- Reimport audio files: Right-click → Reimport
- Check Audio Import Settings → Load Type: Decompress on Load

### Issue: Buttons Don't Work

**Check:**
1. EventSystem exists in scene (created automatically with Canvas)
2. Button components have "Interactable" checked
3. SolitaireUIManager references are assigned

**Fix:**
- If no EventSystem: Right-click Hierarchy → UI → Event System

### Issue: UI Not Showing

**Check:**
1. Canvas Render Mode is "Screen Space - Overlay"
2. UI elements are children of Canvas
3. Camera sees UI layer (should by default)

### Issue: Text Not Updating

**Check:**
1. SolitaireUIManager has all TextMeshPro references assigned
2. SolitaireGameManager events are being triggered (check Console logs)

**Fix:**
- Make sure all Inspector fields in UIManager are filled

### Issue: Undo Not Working

**Check Console for:**
```
[Undo] Recorded move: X card(s) from Tableau to Foundation
[Undo] Undoing move: X card(s) from Foundation back to Tableau
```

If you see "No moves to undo", make sure you're making valid moves first.

---

## Optional Improvements

### 1. Better Button Styling

- Select Button → Image component
- Change colors for Normal, Highlighted, Pressed, Disabled states
- Add icons (download button icons and add to Sprite field)

### 2. Win Popup

Create a popup panel that shows when you win:

1. Right-click `GameUI` → UI → Panel
2. Name: `WinPanel`
3. Add "YOU WON!" text
4. Add "Score: XXX" and "Time: XX:XX"
5. Initially disable panel (uncheck in Inspector)
6. In SolitaireUIManager.OnGameEnd(), enable and show the panel

### 3. Sound Volume Control

Add a slider:
1. Right-click TopPanel → UI → Slider
2. Position in corner
3. In SolitaireUIManager, add OnValueChanged listener
4. Call `AudioManager.Instance.SetVolume(value)`

### 4. Undo Button State

The Undo button automatically disables when no moves available:
- This is handled by `SolitaireUIManager.SetUndoButtonEnabled()`
- You could add visual feedback (grey out, etc.)

---

## Final Checklist

Before building:

- [ ] AudioManager exists in scene with script attached
- [ ] All 6 audio files load successfully
- [ ] UIManager exists with SolitaireUIManager script
- [ ] All UI text fields assigned in UIManager
- [ ] All 3 buttons assigned in UIManager
- [ ] GameManager reference assigned in UIManager
- [ ] Play mode test: audio works
- [ ] Play mode test: UI updates correctly
- [ ] Play mode test: all buttons work
- [ ] Play mode test: undo functionality works
- [ ] No errors in Console

---

## Ready to Build!

Once UI is set up and tested:

1. **Save Scene:** Ctrl+S (Windows) or Cmd+S (Mac)
2. **Build:** File → Build Settings → Build
3. **Deploy:** Run `./deploy.sh` in terminal
4. **Test in Browser:** Hard refresh (Cmd+Shift+R)

Your Solitaire game now has:
- ✅ Sound effects
- ✅ Score/Moves/Timer display
- ✅ New Game/Restart/Undo buttons
- ✅ Full undo functionality

Enjoy! 🎮
