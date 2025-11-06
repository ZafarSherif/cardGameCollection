# Unity Solitaire Project Setup Guide

## Prerequisites

- Unity 2022 LTS or later
- Unity Hub installed
- Basic knowledge of Unity Editor

## Step 1: Create Unity Project

1. **Open Unity Hub**
2. **Click "New Project"**
3. **Settings:**
   - Template: **2D (Core)**
   - Project Name: **CardGames**
   - Location: Select `CardGame/unity-games/` folder
4. **Click "Create Project"**

## Step 2: Project Structure

The scripts have been created for you in:
```
CardGames/Assets/Scripts/
├── Core/
│   ├── Card.cs               ✅ Created
│   └── Pile.cs                ✅ Created
├── Solitaire/
│   ├── SolitaireGameManager.cs  ✅ Created
│   └── DraggableCard.cs          ✅ Created
└── Bridge/
    └── ReactNativeBridge.cs      ✅ Created
```

## Step 3: Import Scripts to Unity

1. In Unity Editor, go to **Project window** (bottom)
2. Right-click in `Assets` folder → **Show in Explorer** (Windows) or **Reveal in Finder** (Mac)
3. You should see the `Scripts` folder already there with all the C# files

If scripts aren't showing in Unity:
- Right-click in Assets → **Refresh**
- Or press `Ctrl+R` (Windows) / `Cmd+R` (Mac)

## Step 4: Create Card Prefab

### 4.1 Create Empty Card GameObject

1. In **Hierarchy**, right-click → **Create Empty**
2. Rename it to "Card"
3. Reset Transform: Position (0, 0, 0)

### 4.2 Add Components

**On the Card GameObject:**
1. Add Component → **Sprite Renderer**
   - This will be the card back (face-down view)
   - Set Sorting Layer to "Default", Order = 0

2. Add Component → **Box Collider 2D**
   - Check "Is Trigger" = OFF
   - This allows card to be clicked/dragged

3. Create child object for card front:
   - Right-click Card → **Create Empty**
   - Rename to "CardFront"
   - Add **Sprite Renderer**
   - Set Sorting Layer to "Default", Order = 1

### 4.3 Add Scripts

**On the Card GameObject:**
1. Drag `Card.cs` script onto Card
2. Drag `DraggableCard.cs` script onto Card

### 4.4 Configure Card Component

In Inspector, on the Card script:
- **Card Front Renderer**: Drag CardFront GameObject here
- **Card Back Renderer**: Drag Card GameObject's SpriteRenderer here

### 4.5 Create Prefab

1. Create folder: `Assets/Prefabs`
2. Drag the Card GameObject from Hierarchy to Prefabs folder
3. Delete Card from Hierarchy (we'll spawn them via code)

## Step 5: Create Pile Prefabs

### 5.1 Create Empty Pile GameObject

1. Hierarchy → Create Empty → Name it "Pile"
2. Add Component → **Box Collider 2D**
   - Size: (1.5, 2) for card-sized hit area
   - Check "Is Trigger" = ON
3. Add the `Pile.cs` script

### 5.2 Create Prefab

- Drag Pile to Prefabs folder
- Delete from Hierarchy

## Step 6: Set Up Solitaire Scene

### 6.1 Create GameManager

1. Hierarchy → Create Empty → Name: "GameManager"
2. Add `SolitaireGameManager.cs` script
3. Add `ReactNativeBridge.cs` script

### 6.2 Create Piles

You need to create:
- 1 Stock Pile
- 1 Waste Pile
- 7 Tableau Piles
- 4 Foundation Piles

**Quick way:**
1. Drag Pile prefab into scene 13 times
2. Rename them:
   - `StockPile`
   - `WastePile`
   - `TableauPile0` through `TableauPile6`
   - `FoundationPile0` through `FoundationPile3`

3. **Position them:**

```
Stock: (-4, 3)        Waste: (-2, 3)        [Foundation 0-3]: (2, 3) to (5, 3)

[Tableau 0-6]: (-4.5, 1) to (4.5, 1) with spacing of 1.5 units
```

### 6.3 Configure Each Pile

For each pile, set in Inspector:
- **Type**: Stock/Waste/Tableau/Foundation
- **Pile Index**:
  - Tableau: 0-6
  - Foundation: 0-3
  - Stock/Waste: 0

### 6.4 Link Piles to GameManager

Select GameManager, in Inspector:
- **Card Prefab**: Drag Card prefab here
- **Stock Pile**: Drag StockPile here
- **Waste Pile**: Drag WastePile here
- **Tableau Piles**: Set size to 7, drag all 7 TableauPiles
- **Foundation Piles**: Set size to 4, drag all 4 FoundationPiles

## Step 7: Camera Setup

1. Select Main Camera
2. Set **Projection** to **Orthographic**
3. Set **Size** to **5** (adjust to fit all piles)
4. Position: (0, 1.5, -10)

## Step 8: Card Sprites (Placeholder)

For now, use Unity's default sprite:
1. In Project, right-click → Create → **Sprites** → **Square**
2. Assign this to both Card Front and Card Back in the Card prefab

**Later, import real card sprites:**
- Get card sprites from asset store or create custom
- Standard size: 140x190 pixels
- 52 front sprites + 1 back sprite

## Step 9: Configure Build Settings

### For WebGL (Testing)

1. File → **Build Settings**
2. Click **WebGL** → **Switch Platform**
3. **Player Settings** → **WebGL** tab:
   - Compression Format: **Gzip**
   - Enable Exceptions: **Explicitly Thrown Exceptions Only**

### For Mobile (Later)

**Android:**
- Switch Platform to Android
- Minimum API Level: 24
- Target API Level: 33
- Scripting Backend: IL2CPP

**iOS:**
- Switch Platform to iOS
- Minimum iOS Version: 12.0
- Architecture: ARM64

## Step 10: Test the Game

1. Press **Play** in Unity Editor
2. You should see:
   - Cards dealt to tableau piles
   - Top cards face up
   - Remaining cards in stock
   - Empty foundation piles

3. **Test interactions:**
   - Drag cards between tableau piles
   - Click stock to draw cards
   - Try moving cards to foundation

## Step 11: Add UI (Optional but Recommended)

Create Canvas:
1. Hierarchy → UI → **Canvas**
2. Add TextMeshPro elements for:
   - Score display
   - Moves counter
   - Timer

Connect to GameManager events:
```csharp
gameManager.OnScoreChanged += (score) => scoreText.text = $"Score: {score}";
gameManager.OnMovesChanged += (moves) => movesText.text = $"Moves: {moves}";
```

## Step 12: Build for Testing

### WebGL Build

1. File → **Build Settings**
2. Click **Build**
3. Choose folder: `unity-games/builds/webgl/`
4. Wait for build to complete

### Test WebGL Build

1. Open `index.html` from build folder in browser
2. Game should load and be playable

## Troubleshooting

### Cards not showing
- Check Card prefab has SpriteRenderers
- Assign sprites to renderers
- Check camera can see the piles

### Cards not dragging
- Ensure Card has Collider2D
- Check DraggableCard script is attached
- Verify Pile Layer Mask is set

### Cards go to wrong pile
- Check Pile Types are set correctly
- Verify Pile Indices are unique
- Test CanAcceptCard logic

### Build errors
- Check all scripts compile (no errors in Console)
- Verify all prefab references are assigned
- Check target platform is set correctly

## Next Steps

1. **Add Card Sprites** - Replace placeholder with real card images
2. **Improve Animations** - Add smooth card movement
3. **Add Sound Effects** - Card flip, place, win sounds
4. **Polish UI** - Better score display, win screen
5. **Integrate with React Native** - Test bridge communication

## Quick Reference

### Important GameObjects
- **GameManager** - Has SolitaireGameManager + ReactNativeBridge
- **Card Prefab** - Has Card + DraggableCard scripts
- **Pile Prefab** - Has Pile script + Collider2D

### Key Scripts
- **Card.cs** - Card data and logic
- **Pile.cs** - Pile management
- **SolitaireGameManager.cs** - Game orchestration
- **DraggableCard.cs** - Drag & drop interaction
- **ReactNativeBridge.cs** - RN communication

### Testing Shortcuts
- **Play Mode**: F5 or click Play button
- **Pause**: Ctrl+Shift+P
- **Step Frame**: Ctrl+Alt+P
- **Scene View**: Double-click GameObject in Hierarchy

## Resources

- Unity Manual: https://docs.unity3d.com/Manual/
- Unity Scripting API: https://docs.unity3d.com/ScriptReference/
- 2D Game Development: https://learn.unity.com/tutorial/2d-game-kit

Good luck! Your Solitaire game foundation is now ready to build! 🎮
