# Fix Drag and Drop Issues - Unity Setup

## Issues Identified

1. ✅ **Pile detection not working** - Fixed in DraggableCard.cs
2. ✅ **Return to original position broken** - Fixed with RefreshCardPositions()
3. ⚠️ **Piles need colliders** - You need to set this up in Unity Editor
4. ⚠️ **Card flipping works** - But only triggers on successful moves

## Code Changes Made

### 1. DraggableCard.cs
- **Removed** `pileLayer` LayerMask dependency
- **Added** three-method pile detection (raycast, overlap, proximity)
- **Fixed** `ReturnToOriginalPosition()` to use `RefreshCardPositions()`
- **Added** extensive debug logging

### 2. Pile.cs
- **Added** `RefreshCardPositions()` public method

## What You Need to Do in Unity

### Step 1: Add Colliders to Pile GameObjects (CRITICAL)

**For each pile in your scene:**

1. **Select Stock Pile** GameObject in Hierarchy
2. In Inspector, click **Add Component**
3. Search for **Box Collider 2D**
4. Set collider size:
   - **Size X**: 1.5 (or width of card area)
   - **Size Y**: 2.0 (or height of card area)
5. Make sure **Is Trigger** is CHECKED ✅

**Repeat for all piles:**
- Stock Pile (1 pile)
- Waste Pile (1 pile)
- Tableau Piles (7 piles)
- Foundation Piles (4 piles)

**Total: 13 pile colliders needed**

### Step 2: Verify Card Colliders

**Select Card Prefab** (Assets/Prefabs/Card)

1. Should already have **Box Collider 2D**
2. Verify settings:
   - **Size**: Matches card sprite size (e.g., 0.7 x 0.95)
   - **Is Trigger**: UNCHECKED ❌
3. If missing, add it now

### Step 3: Test Pile Detection

1. **Play the game**
2. **Open Console** (Ctrl+Shift+C / Cmd+Shift+C)
3. **Drag a card and drop it**
4. **Look for these messages:**

**Expected console output:**
```
MOUSE DOWN DETECTED!
Stop dragging - checking for target pile
Found pile via overlap: Tableau
Attempting to move 1 card(s) from Tableau to Tableau
✓ Move successful!
```

**If you see:**
```
✗ No target pile found
```
→ **Colliders are missing or wrong size**

### Step 4: Adjust Collider Sizes

If pile detection is spotty:

1. **Select a Pile** in Hierarchy
2. **Enable Gizmos** (Scene view, top right)
3. **See the green box** (that's the collider)
4. **Adjust Size X and Size Y** to cover drop zone
5. **Larger is better** for easier dropping

**Recommended sizes:**
- **Tableau/Foundation**: 1.5 x 2.0
- **Stock/Waste**: 1.0 x 1.5

### Step 5: Test Card Flipping

The card flipping should now work automatically:

1. **Stack some cards** on a tableau pile
2. **Drag the top card away**
3. **Drop it elsewhere**
4. **Watch the card below** - should flip face-up automatically ✨

If it doesn't flip, check:
- Was the move successful? (Check Console)
- Is the source pile a Tableau pile?
- Was the card face-down before?

## Debug Checklist

Run through this checklist in Unity:

### ✅ Pile Colliders
- [ ] Stock pile has Box Collider 2D
- [ ] Waste pile has Box Collider 2D
- [ ] All 7 Tableau piles have Box Collider 2D
- [ ] All 4 Foundation piles have Box Collider 2D
- [ ] All pile colliders have "Is Trigger" checked

### ✅ Card Colliders
- [ ] Card prefab has Box Collider 2D
- [ ] Card collider size matches sprite
- [ ] Card collider "Is Trigger" is unchecked

### ✅ Game Functionality
- [ ] Can drag cards
- [ ] Console shows "Found pile via..." messages
- [ ] Cards snap to piles on valid drop
- [ ] Cards return on invalid drop
- [ ] Bottom cards flip face-up after move
- [ ] Can stack red on black, descending rank
- [ ] Can move card stacks (multiple cards at once)

## Console Messages Explained

### Good Messages ✅
```
MOUSE DOWN DETECTED!
```
→ Card click detected

```
Found pile via overlap: Tableau
```
→ Pile detection working! (overlap method is most reliable)

```
✓ Move successful!
```
→ Card moved to pile correctly

### Problem Messages ⚠️
```
✗ No target pile found
```
→ **Fix**: Add/resize colliders on piles

```
✗ Move rejected by game manager
```
→ **Normal**: Move not allowed by Solitaire rules (wrong color, rank, etc.)

```
✗ GameManager is null!
```
→ **Fix**: Make sure SolitaireGameManager component is in scene

### Debug Messages ℹ️
```
Found pile via raycast: Tableau
Found pile via overlap: Foundation
Found pile via proximity: Waste
```
→ Shows which detection method succeeded

```
Returning cards to original position
```
→ Card returned after invalid drop

## Common Issues

### Issue 1: Cards stick where dropped (don't return or snap)

**Cause**: No pile colliders
**Fix**: Add Box Collider 2D to all piles (see Step 1)

### Issue 2: Can't drop cards anywhere

**Cause**: All moves rejected as invalid
**Fix**:
- Check Solitaire rules (red on black, descending)
- Try dropping King on empty tableau
- Try dropping Ace on empty foundation

### Issue 3: Cards return even on valid drops

**Cause**: Pile.CanAcceptCard() rejecting moves
**Fix**:
- Only kings can go on empty tableau
- Only aces can go on empty foundation
- Must alternate colors (red/black)
- Must be descending rank (6 on 7, not 8 on 7)

### Issue 4: Bottom cards don't flip

**Cause**: Move not actually succeeding
**Fix**:
- Check Console for "✓ Move successful!"
- If not successful, fix the drop issue first
- Flipping only happens after successful moves

### Issue 5: "Found pile via proximity" always triggers

**Cause**: Pile colliders too small or missing
**Fix**: Make pile colliders bigger (1.5 x 2.0)

## Quick Test Procedure

1. **Play game**
2. **Find a face-up card**
3. **Drag it to another tableau pile**
4. **Watch Console**:
   - Should see "Stop dragging"
   - Should see "Found pile via..."
   - Should see either "Move successful" or "Move rejected"
5. **If successful**: Card should snap to pile
6. **If rejected**: Card should smoothly return

## Next Steps After Fixing

Once drag-and-drop works:

1. **Test all pile types**:
   - Tableau to tableau
   - Waste to tableau
   - Tableau to foundation
   - Foundation to tableau (undo)

2. **Test edge cases**:
   - Empty tableau (only King)
   - Empty foundation (only Ace)
   - Multi-card dragging
   - Double-click auto-move

3. **Add stock pile click**:
   - Click stock to draw cards
   - Cards move to waste pile

4. **Polish**:
   - Smooth animations
   - Card flip effects
   - Win condition
   - New game button

## Settings Summary

### Pile Colliders
```
Box Collider 2D
- Size: (1.5, 2.0)
- Is Trigger: ✅ Checked
- Offset: (0, 0)
```

### Card Colliders
```
Box Collider 2D
- Size: (0.7, 0.95) [or match your sprite]
- Is Trigger: ❌ Unchecked
- Offset: (0, 0)
```

### DraggableCard Settings
```
DraggableCard Component
- Drag Z Offset: -2
- Pile Detection Radius: 1.5
```

---

**After adding colliders, test the game and watch the Console!**

The debug messages will tell you exactly what's happening. 🎮
