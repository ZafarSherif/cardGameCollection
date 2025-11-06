# Solitaire Rules Testing Checklist

## ✅ Fixes Applied

1. **Foundation now accepts only single cards**
2. **Waste pile now allows dragging only the top card**

## 🧪 Testing Instructions

Test each scenario in Unity Play mode before building.

---

## Tableau Tests (Moving Cards Between Piles)

### Test 1: Valid Stacking ✅
- [ ] Find a red 6 (6♥ or 6♦)
- [ ] Find a black 7 (7♠ or 7♣)
- [ ] Drag the red 6 onto the black 7
- **Expected:** Should snap into place
- **Rule:** Descending rank, alternating colors

### Test 2: Same Color Rejection ❌
- [ ] Find a red 6 (6♥ or 6♦)
- [ ] Find a red 7 (7♥ or 7♦)
- [ ] Try to drag the 6 onto the 7
- **Expected:** Card returns to original position
- **Console:** "✗ Move rejected by game manager"
- **Rule:** Must alternate colors

### Test 3: Wrong Rank Rejection ❌
- [ ] Find an 8
- [ ] Find a 6
- [ ] Try to drag the 8 onto the 6
- **Expected:** Card returns to original position
- **Rule:** Must be descending (one rank lower)

### Test 4: King on Empty Tableau ✅
- [ ] Clear a tableau pile (move all cards away)
- [ ] Find a King (any suit)
- [ ] Drag the King onto the empty pile
- **Expected:** Should snap into place
- **Rule:** Only Kings on empty tableau

### Test 5: Non-King on Empty Tableau ❌
- [ ] Have an empty tableau pile
- [ ] Find a Queen (or any non-King)
- [ ] Try to drag it onto the empty pile
- **Expected:** Card returns to original position
- **Rule:** Only Kings allowed

### Test 6: Multi-Card Dragging ✅
- [ ] Find a sequence of face-up cards (e.g., 5♥ on 6♠ on 7♥)
- [ ] Click and drag the middle card (6♠)
- [ ] Move it to a valid pile (black 7)
- **Expected:** Both 6♠ AND 5♥ should move together
- **Rule:** Can move sequences together

### Test 7: Card Flipping ✅
- [ ] Find a tableau pile with face-down cards
- [ ] Drag the top face-up card away
- [ ] Watch the card below
- **Expected:** Card below should flip face-up
- **Rule:** Auto-flip when revealed

---

## Foundation Tests (Building Up by Suit)

### Test 8: Ace on Empty Foundation ✅
- [ ] Find an Ace (any suit)
- [ ] Drag it to an empty foundation pile
- **Expected:** Should snap into place
- **Rule:** Foundations start with Ace

### Test 9: Non-Ace on Empty Foundation ❌
- [ ] Find a 2 (any suit)
- [ ] Try to drag it to an empty foundation
- **Expected:** Card returns to original position
- **Rule:** Must start with Ace

### Test 10: Building Foundation (Same Suit) ✅
- [ ] Have A♠ in foundation
- [ ] Find 2♠
- [ ] Drag 2♠ onto A♠
- **Expected:** Should snap into place
- [ ] Find 3♠ and place on 2♠
- **Expected:** Should work
- **Rule:** Build up by suit (A→2→3→...→K)

### Test 11: Wrong Suit on Foundation ❌
- [ ] Have A♠ in foundation
- [ ] Find 2♥
- [ ] Try to drag 2♥ onto A♠
- **Expected:** Card returns to original position
- **Rule:** Must be same suit

### Test 12: Wrong Rank on Foundation ❌
- [ ] Have A♠ in foundation
- [ ] Find 3♠
- [ ] Try to drag 3♠ onto A♠
- **Expected:** Card returns to original position
- **Rule:** Must be sequential (A→2, not A→3)

### Test 13: Single Card Only to Foundation ❌ **NEW FIX**
- [ ] Have a sequence of cards (e.g., 5♥ on 6♠)
- [ ] Try to drag both cards to foundation
- **Expected:** Card returns to original position
- **Console:** "✗ Foundation only accepts single cards"
- **Rule:** Only one card at a time to foundation

### Test 14: Double-Click Auto-Move ✅
- [ ] Find an Ace
- [ ] Double-click it
- **Expected:** Should automatically move to foundation
- **Rule:** Bonus feature for convenience

---

## Stock/Waste Tests

### Test 15: Draw from Stock ✅
- [ ] Click on the stock pile (face-down cards)
- **Expected:** 3 cards move to waste pile (or 1 if set to 1)
- **Console:** "Stock pile clicked!"
- **Rule:** Draw 1 or 3 cards

### Test 16: Cannot Drag from Stock ❌
- [ ] Try to click and drag a card from stock pile
- **Expected:** Nothing happens
- **Rule:** Stock cards can't be dragged

### Test 17: Drag Top Card from Waste ✅ **NEW FIX**
- [ ] Draw some cards to waste
- [ ] Drag the TOP card from waste
- **Expected:** Should drag successfully
- **Rule:** Can drag top card from waste

### Test 18: Cannot Drag Lower Cards from Waste ❌ **NEW FIX**
- [ ] Draw 3 cards to waste (so waste has multiple cards)
- [ ] Try to drag a card that's NOT on top
- **Expected:** Nothing happens
- **Console:** "✗ Can only drag top card from waste pile"
- **Rule:** Only top card is playable

### Test 19: Recycle Waste to Stock ✅
- [ ] Click stock until it's empty
- [ ] Click stock again
- **Expected:** All waste cards move back to stock (face-down)
- **Rule:** Stock recycles waste when empty

---

## Scoring Tests

### Test 20: Score for Moving to Foundation
- [ ] Note current score
- [ ] Move a card to foundation
- **Expected:** Score increases by 10
- **Rule:** +10 for foundation move

### Test 21: Score for Revealing Card
- [ ] Note current score
- [ ] Move a card from tableau with face-down card below
- **Expected:** Score increases by 5 (from revealing)
- **Rule:** +5 for flipping face-down card

### Test 22: Score Penalty for Foundation Undo
- [ ] Note current score
- [ ] Move a card FROM foundation back to tableau
- **Expected:** Score decreases by 15
- **Rule:** -15 for undoing foundation move

---

## Win Condition Tests

### Test 23: Win Detection
- [ ] Build all 4 foundation piles to King
- **Expected:**
  - Game won message appears
  - Score gets +100 bonus
  - Time recorded
- **Rule:** Win when all foundations complete

---

## Edge Case Tests

### Test 24: Empty Pile Drop
- [ ] Drag a card to an empty area (not a pile)
- **Expected:** Card returns to original position
- **Console:** "✗ No target pile found"

### Test 25: Simultaneous Moves
- [ ] Drag a card quickly multiple times
- **Expected:** Should handle gracefully, no glitches

### Test 26: Invalid Sequence Dragging
- [ ] Find cards: 5♥ on 8♠ (invalid sequence)
- [ ] Try to drag the 5♥
- **Expected:** Only 5♥ moves (sequence broken)
- **Rule:** Can only drag valid sequences

---

## Console Message Guide

### Good Messages ✅
```
Found pile via overlap: Tableau
✓ Move successful!
Stock pile clicked!
```

### Expected Rejections ✗ (Not Errors!)
```
✗ Move rejected by game manager       (Invalid Solitaire move)
✗ Foundation only accepts single cards (Multi-card to foundation)
✗ Can only drag top card from waste pile (Not top card)
✗ No target pile found                 (Dropped in empty space)
```

### Actual Errors ❌ (Should Not See These!)
```
✗ GameManager is null!
NullReferenceException
Missing component
```

---

## Quick Test Sequence (5 minutes)

For rapid testing of all rules:

1. **Start game** → Cards should deal correctly
2. **Drag red 6 on black 7** → Should work
3. **Try red 6 on red 7** → Should reject
4. **Drag King to empty tableau** → Should work
5. **Drag Ace to foundation** → Should work
6. **Try to drag 2-card stack to foundation** → Should reject
7. **Click stock 3 times** → Should draw cards
8. **Try to drag bottom waste card** → Should reject
9. **Drag top waste card** → Should work
10. **Move card from tableau with face-down below** → Should flip

**If all 10 tests pass, rules are correct!** ✅

---

## Testing After Build

After deploying to React Native web:

1. Test on **Chrome** (primary browser)
2. Test on **Firefox** (WebGL compatibility)
3. Test on **Safari** (iOS/Mac)
4. Test on **mobile browser** (touch controls)

**Confirm:**
- [ ] All rules work same as Unity
- [ ] Touch drag works on mobile
- [ ] No performance issues
- [ ] Score/moves update in React Native header
- [ ] Back button works

---

## Known Issues (Not Bugs)

These are intentional design choices:

- ✓ Can move cards FROM foundation back to tableau (allows undoing)
- ✓ Can see multiple cards in waste (standard in 3-card draw)
- ✓ Stock recycles unlimited times (standard rule)
- ✓ No move timer/limit (casual mode)

---

## Success Criteria

**Before building, verify:**
- [ ] All 26 tests pass
- [ ] No unexpected error messages in Console
- [ ] Game flows smoothly
- [ ] Win condition triggers correctly
- [ ] Score calculates correctly
- [ ] No visual glitches

**Game is ready for deployment!** 🎮

---

## Bug Reporting Format

If you find a bug during testing:

```
Bug: [Brief description]
Steps to Reproduce:
1. [Step 1]
2. [Step 2]
Expected: [What should happen]
Actual: [What actually happened]
Console Messages: [Any error messages]
```

Example:
```
Bug: Can drag multiple cards to foundation
Steps to Reproduce:
1. Have sequence: 5♥ on 6♠
2. Drag both to foundation
Expected: Should reject (foundation accepts single only)
Actual: Both cards move to foundation
Console Messages: None
```
