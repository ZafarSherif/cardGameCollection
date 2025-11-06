# Debug Guide - Card Pickup Issues

## Enhanced Debugging Added

I've added detailed logging to help diagnose the issues. When you click a card, you'll now see:

```
[Card] OnMouseDown: Seven of Hearts | FaceUp: True | Pile: Tableau | Pile Index: 3
[StartDrag] Got 2 movable cards from Tableau
  - Seven of Hearts (FaceUp: True)
  - Six of Spades (FaceUp: True)
```

## Issue 1: "Card is face down" on Tableau

### What to Check

When you click a face-up card on tableau and see "Card is face down", look at the console output:

**Example of the PROBLEM:**
```
[Card] OnMouseDown: Five of Diamonds | FaceUp: False | Pile: Tableau | Pile Index: 2
✗ Card is face down - cannot drag Five of Diamonds
```

This tells us the card component thinks it's face-down when it shouldn't be.

**Possible causes:**

1. **Raycast hitting wrong card** - You clicked a face-up card, but raycast hit a face-down card below it
2. **Card not flipped** - The card should have been flipped face-up but wasn't
3. **Collider overlap** - Multiple card colliders overlap and wrong one gets the click

### Solutions:

#### Solution 1: Check Collider Sizes
Cards might have overlapping colliders.

1. Select a card in the scene during Play mode
2. Check Box Collider 2D size
3. Should match card sprite size (e.g., 0.7 x 0.95)
4. If too large, it might cover cards below it

#### Solution 2: Check Z-Positions
Cards should have different Z-positions so top cards are clickable.

When cards are in a tableau pile, they should have:
- Face-down cards: Z = 0
- Face-up cards: Z = -0.01 * index (slightly in front)

Check in Pile.cs UpdateCardPositions() - does it set Z positions?

#### Solution 3: Disable Colliders on Face-Down Cards
Face-down cards shouldn't respond to clicks anyway.

Add to Card.cs UpdateVisuals():
```csharp
// Also enable/disable collider based on face state
Collider2D collider = GetComponent<Collider2D>();
if (collider != null)
{
    collider.enabled = isFaceUp;
}
```

---

## Issue 2: Cannot Pick Up Multiple Cards

### What to Check

When you click a sequence of cards on tableau, check the console:

**Example of working correctly:**
```
[Card] OnMouseDown: Seven of Hearts | FaceUp: True | Pile: Tableau | Pile Index: 3
[StartDrag] Got 3 movable cards from Tableau
  - Seven of Hearts (FaceUp: True)
  - Six of Spades (FaceUp: True)
  - Five of Diamonds (FaceUp: True)
```

**Example of the PROBLEM:**
```
[Card] OnMouseDown: Seven of Hearts | FaceUp: True | Pile: Tableau | Pile Index: 3
[StartDrag] Got 1 movable cards from Tableau
  - Seven of Hearts (FaceUp: True)
```

Only 1 card even though there are more below it.

**Possible causes:**

1. **Cards below aren't face-up** - GetMovableCards() only includes face-up cards
2. **CurrentPile not set** - Cards don't know they're in the pile
3. **Pile.Cards list doesn't include them** - Cards weren't added to pile properly

### Solutions:

#### Solution 1: Check if Cards Below are Face-Up

The log shows the face-up state of each card being dragged. If you see:
```
  - Seven of Hearts (FaceUp: True)
  - Six of Spades (FaceUp: False)  ← This is the problem!
```

The card below isn't face-up, so it can't be dragged.

**Fix:** Ensure cards are flipped when revealed. In SolitaireGameManager.cs:
```csharp
// After dealing cards to tableau
for (int pileIndex = 0; pileIndex < 7; pileIndex++)
{
    tableauPiles[pileIndex].FlipTopCard();
}
```

#### Solution 2: Verify Pile.GetMovableCards()

Check Pile.cs GetMovableCards():
```csharp
public List<Card> GetMovableCards(Card startCard)
{
    List<Card> movableCards = new List<Card>();
    int startIndex = cards.IndexOf(startCard);

    if (startIndex < 0) return movableCards;

    // Can only move if startCard is face up
    if (!startCard.IsFaceUp) return movableCards;

    // Get all cards from startCard to the end (if all are face up)
    for (int i = startIndex; i < cards.Count; i++)
    {
        if (!cards[i].IsFaceUp)
            break; // Stop if we hit a face-down card

        movableCards.Add(cards[i]);
    }

    return movableCards;
}
```

This should work correctly.

#### Solution 3: Check Card Assignment to Pile

When cards are added to a tableau pile, make sure:
1. Card.SetPile() is called
2. Card.CurrentPile is set
3. Card is in Pile.Cards list

In SolitaireGameManager.cs DealCards():
```csharp
pile.AddCard(card); // This should call card.SetPile()
```

And in Pile.cs AddCard():
```csharp
public void AddCard(Card card)
{
    if (card == null) return;

    cards.Add(card);
    card.SetPile(this, cards.Count - 1);  // ← Important!
    card.transform.SetParent(transform);

    UpdateCardPositions();
}
```

---

## Quick Test Procedure

### Test 1: Click Face-Up Card on Tableau

1. Find a face-up card on tableau (top card of any pile)
2. Click it
3. Check Console:

**Expected:**
```
[Card] OnMouseDown: ... | FaceUp: True | Pile: Tableau
[StartDrag] Got X movable cards from Tableau
```

**If you see:**
```
✗ Card is face down - cannot drag ...
```
→ Raycast is hitting wrong card OR card isn't actually face-up

### Test 2: Click Card with Sequence Below

1. Find a tableau pile with multiple face-up cards stacked
2. Click the TOP card of the sequence
3. Check Console - should show multiple cards being dragged

**Expected:**
```
[StartDrag] Got 3 movable cards from Tableau
  - Seven of Hearts (FaceUp: True)
  - Six of Spades (FaceUp: True)
  - Five of Diamonds (FaceUp: True)
```

### Test 3: Click Middle of Sequence

1. Click a card in the MIDDLE of a face-up sequence
2. Should drag that card + all cards below it

**Expected:**
```
[Card] OnMouseDown: Six of Spades | FaceUp: True | Pile: Tableau | Pile Index: 4
[StartDrag] Got 2 movable cards from Tableau
  - Six of Spades (FaceUp: True)
  - Five of Diamonds (FaceUp: True)
```

---

## Common Patterns

### Pattern 1: Clicking Face-Up but Getting Face-Down Message

**Console shows:**
```
[Card] OnMouseDown: Five of Diamonds | FaceUp: False | Pile: Tableau
✗ Card is face down - cannot drag Five of Diamonds
```

**Problem:** You're clicking a card that LOOKS face-up visually, but its IsFaceUp property is false.

**Solutions:**
- Check if cards were properly flipped during deal
- Verify FlipTopCard() is being called after moves
- Check UpdateVisuals() is showing/hiding sprites correctly

### Pattern 2: Only Dragging One Card from Sequence

**Console shows:**
```
[StartDrag] Got 1 movable cards from Tableau
  - Seven of Hearts (FaceUp: True)
```

But visually you see multiple cards below it.

**Problem:** Cards below aren't in the pile's card list, or aren't face-up.

**Solutions:**
- Check DealCards() properly adds all cards to piles
- Verify all face-up cards in sequence are actually marked as IsFaceUp: true
- Check Pile.Cards list contains all visible cards

### Pattern 3: Raycast Hitting Wrong Card

**Console shows:**
```
[Card] OnMouseDown: Two of Clubs | FaceUp: False | Pile: Tableau | Pile Index: 0
```

But you clicked on the Seven of Hearts on top!

**Problem:** Raycast is hitting a card lower in the pile.

**Solutions:**
- Disable colliders on face-down cards
- Adjust Z-positions so top cards are in front
- Make sure collider sizes don't overlap too much

---

## Apply the Quick Fix

Try this quick fix for face-down card colliders:

**In Card.cs, update UpdateVisuals():**

```csharp
private void UpdateVisuals()
{
    if (cardFrontRenderer != null)
        cardFrontRenderer.enabled = isFaceUp;
    if (cardBackRenderer != null)
        cardBackRenderer.enabled = !isFaceUp;

    // NEW: Disable collider for face-down cards
    Collider2D collider = GetComponent<Collider2D>();
    if (collider != null)
    {
        collider.enabled = isFaceUp;
    }
}
```

This ensures face-down cards can't be clicked at all!

---

## Share Console Output

Test these scenarios and share the console output:

1. **Click top face-up card on tableau** → Copy the [Card] OnMouseDown and [StartDrag] lines
2. **Click face-down card** → Should see nothing (collider disabled)
3. **Click middle of sequence** → Copy the [StartDrag] output showing all cards

This will help identify exactly what's happening! 🔍
