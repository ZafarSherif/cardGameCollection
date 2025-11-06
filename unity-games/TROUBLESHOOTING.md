# Troubleshooting Guide

## Issues Fixed

### ✅ Debug Logging Re-enabled
- Added detailed logs to DraggableCard OnMouseDown
- Added logs to ClickableStockPile
- Console will now show exactly what's happening

### ✅ ClickableWastePile Blocking Cards
- **Problem:** ClickableWastePile script was blocking card clicks
- **Solution:** Script now disables itself automatically
- **Action Required:** Remove ClickableWastePile component from Waste Pile (see below)

---

## Current Issues

### Issue 1: Cards Not Draggable on First Try

**Symptoms:**
- Click a card, nothing happens
- Click again, it drags successfully

**Debugging:**
Watch Console when clicking a card. You should see:
```
[Card] OnMouseDown: Ace of Spades from Tableau
```

**If you DON'T see this message:**
→ The click is being blocked by another collider

**Possible causes:**
1. **Pile collider is blocking card** (most likely)
2. **Another component intercepting clicks**
3. **Card collider is too small**

**Solutions:**

#### Solution 1: Adjust Pile Collider Z-Order
The pile's collider might be in front of the cards.

1. Select a Pile GameObject
2. In Inspector, find Transform component
3. Check Z position (should be 0 or positive)
4. **Cards should have negative Z** when placed on pile
5. Try setting pile Z to 0.1 (behind cards)

#### Solution 2: Disable Pile Collider Raycast
Cards should be detected before piles.

1. Select a Pile GameObject
2. Find Box Collider 2D component
3. Try disabling it temporarily
4. Test if cards drag on first click
5. If fixed, we need to adjust sorting order

#### Solution 3: Check Sorting Order
Cards need higher sorting order than piles.

In Pile.cs UpdateCardPositions():
```csharp
renderer.sortingOrder = i; // Cards use index
```

Piles should have sorting order -1 or less.

---

### Issue 2: Stock Pile Not Dealing Cards

**Symptoms:**
- Click on stock pile
- Nothing happens
- No cards move to waste

**Debugging Steps:**

**Step 1: Check Console**
When you click stock, you should see:
```
[StockPile] Mouse entered stock pile area
[StockPile] OnMouseDown - Stock pile clicked!
[StockPile] Calling DrawFromStock() - Stock has X cards
```

**If you see nothing:**
→ ClickableStockPile component not receiving clicks

**If you see "Mouse entered" but not "OnMouseDown":**
→ Click is being blocked by cards on top of the pile

**Step 2: Verify Component Setup**
1. Select Stock Pile GameObject in Hierarchy
2. Verify components in Inspector:
   - ✅ Pile component
   - ✅ Box Collider 2D (Is Trigger: checked)
   - ✅ ClickableStockPile component
   - ❌ NO ClickableWastePile component!

**Step 3: Check Collider Size**
1. Select Stock Pile
2. Box Collider 2D component
3. Size should be: (1.0-1.5, 1.5-2.0)
4. Offset should be: (0, 0)
5. **Is Trigger must be CHECKED** ✅

**Step 4: Check Card Overlap**
The issue might be that face-down cards on stock pile are blocking clicks.

**Temporary fix:**
Add this to ClickableStockPile.cs Awake():
```csharp
// Ensure this collider is in front of cards
GetComponent<Collider2D>().offset = new Vector2(0, 0.5f);
```

**Better fix:**
Cards on stock pile should NOT have colliders enabled, or should be on a different layer.

---

## How to Remove ClickableWastePile Component

**Important:** This component is blocking card clicks on waste pile!

1. **Select Waste Pile** GameObject in Hierarchy
2. Find **ClickableWastePile** component in Inspector
3. Click the **⋮** (three dots) on the component
4. Select **Remove Component**
5. **Test:** Cards should now drag from waste pile on first click

---

## Expected Console Output

### When Game Starts:
```
CardSpriteManager Instance created
Card front sprite set: Ace of Spades
Card front sprite set: Two of Hearts
... (52 cards loading)
```

### When Clicking Cards:
```
[Card] OnMouseDown: Six of Hearts from Tableau
Stop dragging - checking for target pile
Found pile via overlap: Tableau
Attempting to move 1 card(s) from Tableau to Tableau
✓ Move successful!
```

### When Clicking Stock:
```
[StockPile] Mouse entered stock pile area
[StockPile] OnMouseDown - Stock pile clicked!
[StockPile] Calling DrawFromStock() - Stock has 24 cards
```

### When Dragging from Waste:
```
[Card] OnMouseDown: Seven of Diamonds from Waste
Stop dragging - checking for target pile
Found pile via overlap: Tableau
✓ Move successful!
```

---

## Debug Checklist

Run through this checklist:

### Stock Pile:
- [ ] Has Pile component
- [ ] Has Box Collider 2D (Is Trigger ✅)
- [ ] Has ClickableStockPile component
- [ ] NO ClickableWastePile component
- [ ] Collider size: (1.0-1.5, 1.5-2.0)
- [ ] Console shows "[StockPile]" messages when clicked

### Waste Pile:
- [ ] Has Pile component
- [ ] Has Box Collider 2D (Is Trigger ✅)
- [ ] **NO ClickableWastePile component** ⚠️
- [ ] Collider size: (1.0-1.5, 1.5-2.0)

### Tableau Piles (all 7):
- [ ] Have Pile component
- [ ] Have Box Collider 2D (Is Trigger ✅)
- [ ] NO other clickable components
- [ ] Collider size: (1.5, 2.0)

### Foundation Piles (all 4):
- [ ] Have Pile component
- [ ] Have Box Collider 2D (Is Trigger ✅)
- [ ] NO other clickable components
- [ ] Collider size: (1.5, 2.0)

### Cards:
- [ ] Have Card component
- [ ] Have DraggableCard component
- [ ] Have Box Collider 2D (Is Trigger ❌ unchecked)
- [ ] Console shows "[Card]" messages when clicked

---

## Quick Fixes

### If cards not dragging at all:
```
1. Check card has Box Collider 2D
2. Check card has DraggableCard component
3. Check card is face-up
4. Check Console for "[Card] OnMouseDown" message
```

### If stock pile not clicking:
```
1. Remove ClickableWastePile from Waste Pile ⚠️
2. Check Stock Pile has ClickableStockPile component
3. Check Stock Pile collider size (1.5 x 2.0)
4. Check Console for "[StockPile]" messages
```

### If waste cards not dragging:
```
1. Remove ClickableWastePile component ⚠️
2. Check only top card is draggable (this is correct)
3. Check Console shows which card you're clicking
```

---

## Layer Setup (Advanced)

If collider conflicts persist, set up layers:

1. **Create Layers** (Edit → Project Settings → Tags and Layers):
   - Layer 8: "Piles"
   - Layer 9: "Cards"

2. **Assign Layers:**
   - All Pile GameObjects → Layer "Piles"
   - All Card GameObjects → Layer "Cards"

3. **Raycast Filter:**
   - Cards raycast only hits "Cards" layer
   - Piles raycast only hits "Piles" layer

But this is usually not necessary if collider sizes are correct!

---

## Still Having Issues?

If problems persist after these fixes:

1. **Share Console output** when clicking cards/piles
2. **Check Transform Z positions** of piles and cards
3. **Verify all components** are on correct GameObjects
4. **Try disabling pile colliders** temporarily to test

The debug logs will tell us exactly what's happening! 🔍
