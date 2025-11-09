# Unity Mobile Responsive Layout Guide

## 🎯 Goal
Make the Solitaire game (and all future Unity games) work seamlessly on:
- Desktop (current 960x600)
- Mobile Portrait (9:16, 9:18, 9:21 aspect ratios)
- Mobile Landscape (16:9, 18:9, 21:9 aspect ratios)
- Tablets (various sizes)
- Seamless rotation during gameplay (no interruption)

## 📋 Implementation Checklist

### Phase 1: Canvas Setup (Unity Editor)
- [ ] Update Canvas Scaler settings
- [ ] Set reference resolution
- [ ] Configure match width/height

### Phase 2: Dynamic Positioning Script
- [ ] Create ResponsiveLayout.cs
- [ ] Calculate screen dimensions
- [ ] Position piles for landscape
- [ ] Position piles for portrait
- [ ] Add smooth transitions on rotation

### Phase 3: Camera Adjustment
- [ ] Make camera orthographic size responsive
- [ ] Adjust based on screen aspect ratio

### Phase 4: UI Panel Adjustments
- [ ] Make top panel responsive
- [ ] Position buttons relative to screen
- [ ] Scale win panel for all screens

### Phase 5: Testing
- [ ] Test portrait (9:16)
- [ ] Test landscape (16:9)
- [ ] Test rotation mid-game
- [ ] Test on real mobile device

## 🛠️ Technical Implementation

### Step 1: Canvas Scaler Settings

**Location:** GameUI (Canvas) → Inspector → Canvas Scaler

**Current Settings:**
```
UI Scale Mode: Constant Pixel Size (WRONG for mobile)
```

**New Settings:**
```
UI Scale Mode: Scale With Screen Size
Reference Resolution: 1080 x 1920 (portrait base)
Screen Match Mode: Match Width Or Height
Match: 0.5 (blend between width and height)
```

**Why?** This makes UI elements scale proportionally on any screen size.

---

### Step 2: Create ResponsiveLayout Script

**File:** `Assets/Scripts/ResponsiveLayout.cs`

**Purpose:**
- Detect screen size changes
- Calculate pile positions dynamically
- Support both portrait and landscape
- Smooth transitions when rotating

**Key Features:**
- Landscape: 7 tableau piles in single row
- Portrait: 7 tableau piles in 2 rows (4 top, 3 bottom)
- Foundation/Stock/Waste positioned intelligently
- Cards scale based on available space

---

### Step 3: Screen Layouts

#### Landscape Layout (width > height)
```
┌─────────────────────────────────────────────┐
│ [Stock] [Waste]           [🏆][🏆][🏆][🏆] │ ← Top bar
│                                             │
│   [▯]    [▯]    [▯]    [▯]    [▯]    [▯]   │ ← Tableau
│   [▯]    [▯]    [▯]    [▯]    [▯]    [▯]   │   (7 columns)
│   [▯]    [▯]    [▯]    [▯]    [▯]    [▯]   │
│                                             │
└─────────────────────────────────────────────┘
```

#### Portrait Layout (height > width)
```
┌───────────────────┐
│  [Stock] [Waste]  │ ← Top
│  [🏆][🏆][🏆][🏆] │ ← Foundation
├───────────────────┤
│                   │
│ [▯]  [▯]  [▯] [▯]│ ← Row 1 (4 piles)
│ [▯]  [▯]  [▯] [▯]│
│ [▯]  [▯]  [▯] [▯]│
│                   │
│  [▯]  [▯]  [▯]   │ ← Row 2 (3 piles)
│  [▯]  [▯]  [▯]   │
│                   │
└───────────────────┘
```

---

### Step 4: Rotation Handling

**When user rotates device:**
1. Unity detects screen size change via `Screen.width` and `Screen.height`
2. Script recalculates pile positions
3. Piles smoothly animate to new positions (0.3s)
4. Game continues without interruption
5. Dragged cards stay with cursor/finger

**Edge Cases:**
- Dragging during rotation: Pause drag → reposition → resume
- Animation during rotation: Let finish → then reposition
- Win panel open: Reposition panel to fit new screen

---

## 📐 Position Calculation Logic

### Landscape Positioning:
```csharp
// Calculate usable screen width
float screenWidth = Camera.main.orthographicSize * Camera.main.aspect * 2;
float usableWidth = screenWidth * 0.9f; // 90% to leave margins

// Space out 7 tableau piles evenly
float spacing = usableWidth / 8; // 8 gaps for 7 piles
float startX = -usableWidth / 2 + spacing;

for (int i = 0; i < 7; i++) {
    float x = startX + (i * spacing);
    tableauPiles[i].position = new Vector3(x, 0, 0);
}
```

### Portrait Positioning:
```csharp
// Top row: 4 piles
float topSpacing = usableWidth / 5;
for (int i = 0; i < 4; i++) {
    float x = startX + (i * topSpacing);
    tableauPiles[i].position = new Vector3(x, 1, 0);
}

// Bottom row: 3 piles (centered)
float bottomSpacing = usableWidth / 4;
for (int i = 4; i < 7; i++) {
    float x = startX + ((i - 4) * bottomSpacing);
    tableauPiles[i].position = new Vector3(x, -3, 0);
}
```

---

## 🎨 Card Scaling

Cards should scale based on available space:

```csharp
// Calculate ideal card width (should fit ~7 cards horizontally with gaps)
float cardWidth = usableWidth / 9; // 7 cards + 2 margins

// Unity card standard aspect ratio: 0.714 (2.5:3.5)
float cardHeight = cardWidth / 0.714f;

// Apply scale
float scale = cardWidth / originalCardWidth;
```

---

## 🧪 Testing Resolutions

Test these in Unity Game view:

**Mobile Portrait:**
- 1080 x 1920 (9:16) - Standard
- 1080 x 2160 (9:18) - Taller phones
- 1080 x 2340 (9:19.5) - iPhone 12/13/14

**Mobile Landscape:**
- 1920 x 1080 (16:9) - Standard
- 2160 x 1080 (18:9) - Modern Android
- 2340 x 1080 (19.5:9) - iPhone landscape

**Tablets:**
- 1536 x 2048 (3:4) - iPad
- 1600 x 2560 (10:16) - Android tablets

**Desktop:**
- 960 x 600 (current)
- 1280 x 720
- 1920 x 1080

---

## 🚀 Implementation Timeline

**Session 1: Canvas Setup (30 mins)**
- Update Canvas Scaler
- Test UI scaling in different resolutions

**Session 2: Create ResponsiveLayout Script (1-2 hours)**
- Write landscape positioning logic
- Write portrait positioning logic
- Add screen change detection

**Session 3: Smooth Transitions (1 hour)**
- Add position interpolation
- Handle rotation edge cases
- Test transitions

**Session 4: Testing & Polish (1-2 hours)**
- Test all resolutions
- Fix any layout bugs
- Optimize positioning

**Session 5: Deploy & Real Device Test (30 mins)**
- Build WebGL
- Deploy to GitHub Pages
- Test on actual phone

**Total: 4-6 hours**

---

## 📱 Real Device Testing

After deployment, test on:
- Your phone (portrait)
- Your phone (landscape)
- Friend's phone (different screen size)
- Tablet (if available)
- Rotate during active gameplay

---

## 🔄 Future Games

**Apply this pattern to all future Unity games:**
1. Use Canvas Scaler with Scale With Screen Size
2. Create ResponsiveLayout script
3. Calculate positions dynamically
4. Support portrait and landscape
5. Test on multiple devices

**Reusable Components:**
- ResponsiveLayout.cs (adapt for each game)
- Camera adjustment logic
- Card scaling logic
- Smooth transition system

---

## 📝 Notes

- Portrait mode uses 2-row layout because 7 cards won't fit in single row
- Landscape uses single row (like desktop)
- Foundation piles always at top (easier to reach on mobile)
- Stock/Waste at top left corner
- Win panel scales to screen size
- Buttons positioned relative to screen edges

---

## 🐛 Common Issues & Solutions

**Issue:** Cards overlap in portrait
**Solution:** Reduce card size or spacing

**Issue:** Top panel cuts off in landscape
**Solution:** Use Canvas anchors, not absolute positions

**Issue:** Rotation feels jerky
**Solution:** Increase transition duration (0.3s → 0.5s)

**Issue:** Drag breaks during rotation
**Solution:** Pause drag, reposition, resume with updated positions

**Issue:** Camera doesn't show all cards
**Solution:** Adjust orthographic size based on aspect ratio

---

## ✅ Success Criteria

- [ ] Works on mobile portrait
- [ ] Works on mobile landscape
- [ ] Seamless rotation mid-game
- [ ] Cards properly sized on all screens
- [ ] UI buttons accessible
- [ ] No cards cut off or overlapping
- [ ] Performance: 60 FPS on mobile
- [ ] Win panel displays correctly
