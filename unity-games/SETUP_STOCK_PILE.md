# Setup Stock Pile Click to Deal Cards

## What You Need to Do

The `DrawFromStock()` method already exists in SolitaireGameManager.cs, but clicking the stock pile doesn't trigger it. We need to add a component to make the stock pile clickable.

## Quick Setup (2 minutes)

### Step 1: Add Component to Stock Pile

1. **Open Unity**
2. **Select Stock Pile** GameObject in Hierarchy
3. Click **Add Component**
4. Search for **Clickable Stock Pile**
5. Click to add it

### Step 2: Verify Collider

Make sure the Stock Pile has a Box Collider 2D:

1. **Stock Pile should already have** Box Collider 2D (you added it earlier)
2. If not, add it:
   - Add Component → Box Collider 2D
   - Size: (1.0, 1.5)
   - Is Trigger: ✅ Checked

### Step 3: Test It

1. **Press Play**
2. **Click on the Stock Pile** (the face-down cards on the left)
3. **Watch**: Cards should move to Waste Pile
4. **Click again**: More cards should move
5. **When empty**: Clicking recycles Waste back to Stock

## Expected Behavior

### First Click
```
Stock pile clicked!
```
→ Draws 3 cards (or 1, depending on settings) from Stock to Waste

### Subsequent Clicks
→ Keeps drawing until Stock is empty

### When Stock is Empty
→ Automatically recycles all Waste cards back to Stock (face-down)

## Debug Messages

In Console, you should see:
```
Stock pile clicked!
```

If you see nothing when clicking:
- ❌ Collider missing or too small
- ❌ Wrong layer (should be Default)
- ❌ Cards blocking the pile (shouldn't matter, but check)

## Troubleshooting

### Problem: Clicking stock does nothing

**Solution 1: Check collider**
- Select Stock Pile
- Verify Box Collider 2D exists
- Make sure "Is Trigger" is checked
- Try increasing collider size to (1.5, 2.0)

**Solution 2: Check component**
- Select Stock Pile
- Verify ClickableStockPile component is attached
- Check Console for errors

**Solution 3: Check card blocking**
- Cards have colliders that might block clicks
- The pile collider should still be clickable though
- If issue persists, you can set cards to a different layer

### Problem: Cards don't appear in Waste

**Solution: Check Waste Pile setup**
- Waste Pile GameObject exists in scene
- Assigned in SolitaireGameManager Inspector
- Has correct position

### Problem: Stock doesn't recycle when empty

**Solution: Check SolitaireGameManager**
- The `RecycleWasteToStock()` method is called automatically
- Check Console for errors
- Verify Waste Pile is assigned in Inspector

## Game Settings

You can adjust draw count in SolitaireGameManager:

1. **Select SolitaireGameManager** GameObject in Hierarchy
2. In Inspector, find **Game Settings**
3. **Draw Count**:
   - Set to **1** for easier game (draw 1 card at a time)
   - Set to **3** for standard Solitaire (draw 3 cards at a time)

## How It Works

### ClickableStockPile.cs
```csharp
private void OnMouseDown()
{
    Debug.Log("Stock pile clicked!");
    gameManager.DrawFromStock();
}
```

### SolitaireGameManager.DrawFromStock()
```csharp
public void DrawFromStock()
{
    if (stockPile.IsEmpty())
    {
        // Recycle waste back to stock
        RecycleWasteToStock();
        return;
    }

    // Draw cards to waste
    for (int i = 0; i < drawCount; i++)
    {
        Card card = stockPile.RemoveTopCard();
        card.SetFaceUp(true);
        wastePile.AddCard(card);
    }
}
```

## Visual Reference

```
Before Click:          After Click:
┌─────┐               ┌─────┐     ┌─────┐
│ ▓▓▓ │  ←Click       │ ▓▓▓ │     │ 7♠  │
│ ▓▓▓ │               │ ▓▓▓ │     │     │
└─────┘               └─────┘     └─────┘
 Stock                 Stock       Waste
(24 cards)            (21 cards)  (3 cards)
```

## Complete Checklist

- [ ] ClickableStockPile script created
- [ ] Script added to Stock Pile GameObject
- [ ] Stock Pile has Box Collider 2D
- [ ] Collider "Is Trigger" is checked
- [ ] Tested clicking stock pile
- [ ] Cards move to waste pile
- [ ] Recycles when stock is empty
- [ ] Draw count set in GameManager (1 or 3)

## Next Steps

Once stock pile clicking works:

1. **Test the full game flow**:
   - Deal initial cards ✅
   - Click stock to draw ✅
   - Drag cards between piles ✅
   - Move to foundation ✅
   - Cards flip automatically ✅

2. **Add polish**:
   - Smooth card movement animations
   - Flip animations
   - Highlight on hover
   - Sound effects

3. **Add UI**:
   - New Game button
   - Score display
   - Moves counter
   - Timer

4. **Build and deploy**:
   - Build WebGL
   - Copy to React Native
   - Test in browser

---

**The code is ready - you just need to add the component in Unity!** 🎮
