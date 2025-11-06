# Card Sprites Setup - Step by Step

This guide will walk you through setting up card sprites in your Unity project.

## What Changed

I've updated the code to use a **CardSpriteManager** that automatically loads sprites for you. You no longer need to manually assign sprites in the Inspector!

### Updated Files:
1. **Card.cs** - Now uses `CardSpriteManager.Instance.GetCardSprite()`
2. **CardSpriteManager.cs** - New manager class that handles sprite loading
3. **SolitaireGameManager.cs** - Simplified to only pass suit and rank

## Quick Start Options

### Option 1: Use Resources Folder (Recommended for Beginners)

This method loads sprites automatically from the `Resources` folder.

**Step 1:** Create folder structure
```
Assets/
  Resources/
    Cards/
      Faces/
        (put your 52 card sprites here)
      Backs/
        (put your card back sprite here)
```

**Step 2:** Name your sprites correctly
- **Card faces**: `{suit}_{rank}.png`
  - Examples: `spades_ace.png`, `hearts_10.png`, `clubs_king.png`
- **Card back**: `card_back_blue.png`

**Step 3:** That's it! The CardSpriteManager will automatically load them.

### Option 2: Manual Assignment (For Full Control)

This method lets you drag and drop sprites in the Inspector.

**Step 1:** Create a CardSpriteManager GameObject
1. In Unity, right-click Hierarchy → Create Empty
2. Name it "CardSpriteManager"
3. Add the CardSpriteManager component

**Step 2:** Change Load Method
1. Select CardSpriteManager in Hierarchy
2. In Inspector, change "Load Method" to "Serialized"

**Step 3:** Import your sprites
1. Create folder: `Assets/Art/Cards/Faces/`
2. Import all 52 card sprites
3. Import card back sprite

**Step 4:** Auto-populate sprites
1. Select CardSpriteManager in Hierarchy
2. Right-click the component in Inspector
3. Click "Auto-Populate Serialized Sprites"
4. This will automatically find and assign all sprites

## Getting Free Card Sprites

### Quick Download: Kenney's Cards

1. Go to: https://kenney.nl/assets/boardgame-pack
2. Click "Download" (free!)
3. Extract the ZIP file
4. Inside, find the `PNG/Cards/` folder
5. You'll have 52 card faces + card back

### Import into Unity

**Drag and drop the sprites into Unity:**
```
Assets/
  Art/
    Cards/
      Faces/
        cardClubsA.png → rename to: clubs_ace.png
        cardClubs2.png → rename to: clubs_2.png
        ... (rename all 52 cards)
      Backs/
        cardBack_blue1.png → rename to: card_back_blue.png
```

## Renaming Sprites

If your downloaded sprites have different names, you need to rename them to match this pattern:

### Card Face Naming:
```
{suit}_{rank}.png
```

**Suits:** clubs, diamonds, hearts, spades
**Ranks:** ace, 2, 3, 4, 5, 6, 7, 8, 9, 10, jack, queen, king

### Examples:
```
Kenney's Name → Your Name
cardClubsA.png → clubs_ace.png
cardClubs2.png → clubs_2.png
cardClubsJ.png → clubs_jack.png
cardHeartsK.png → hearts_king.png
cardDiamonds10.png → diamonds_10.png
```

### Quick Rename Script (Python)

Save this as `rename_cards.py` in your sprites folder:

```python
import os
import re

# Mapping of Kenney card names to our format
suit_map = {
    'Clubs': 'clubs',
    'Diamonds': 'diamonds',
    'Hearts': 'hearts',
    'Spades': 'spades'
}

rank_map = {
    'A': 'ace',
    'J': 'jack',
    'Q': 'queen',
    'K': 'king'
}

for filename in os.listdir('.'):
    if filename.startswith('card') and filename.endswith('.png'):
        # Parse: cardClubsA.png → Clubs, A
        match = re.match(r'card([A-Z][a-z]+)([A-Z]|\d+)\.png', filename)

        if match:
            suit = match.group(1)
            rank = match.group(2)

            # Convert suit
            suit = suit_map.get(suit, suit.lower())

            # Convert rank
            if rank in rank_map:
                rank = rank_map[rank]

            # New filename
            new_name = f'{suit}_{rank}.png'
            os.rename(filename, new_name)
            print(f'{filename} → {new_name}')

print('Done!')
```

Run it:
```bash
cd path/to/your/sprites
python3 rename_cards.py
```

## Unity Import Settings

After importing sprites:

1. **Select all card sprites** in Project window
2. **In Inspector, set:**
   - Texture Type: `Sprite (2D and UI)`
   - Sprite Mode: `Single`
   - Pixels Per Unit: `100`
   - Filter Mode: `Bilinear`
   - Compression: `None` (or High Quality)
   - Max Size: `512` or `1024`

3. Click **Apply**

## Testing Your Setup

### Test 1: Check CardSpriteManager

1. Play your game in Unity
2. Open Console (Ctrl+Shift+C / Cmd+Shift+C)
3. You should see logs like:
   ```
   Card front sprite set: Ace of Spades
   Card front sprite set: Two of Hearts
   ...
   ```

### Test 2: Check Visually

1. Play the game
2. Cards should now show their proper faces when face-up
3. Cards should show the blue back when face-down

### If Sprites Don't Load

**Problem: "No sprite found for Ace of Spades"**

**Solution:**
- Check your folder structure matches exactly
- Verify sprite names are lowercase
- Make sure folder is named `Resources` (case-sensitive!)
- Path should be: `Assets/Resources/Cards/Faces/spades_ace.png`

**Problem: "Sprite not found at: Resources/Cards/Faces/..."**

**Solution:**
- The Resources folder must be a direct child of Assets
- NOT: `Assets/Art/Resources` ❌
- YES: `Assets/Resources` ✅

## Performance Optimization

### Create a Sprite Atlas (Optional)

For better performance, especially on mobile:

1. Right-click in Project → Create → 2D → Sprite Atlas
2. Name it "CardsAtlas"
3. In Inspector:
   - **Include in Build**: ✅ Checked
   - **Allow Rotation**: ❌ Unchecked
   - **Tight Packing**: ✅ Checked
   - **Padding**: 2
4. **Objects for Packing** → Click ➕
5. Drag your `Cards/Faces` folder into the list
6. Unity will automatically pack all sprites into one texture

## Next Steps

Once you have sprites set up:

1. **Rebuild for WebGL** to see the changes
2. **Add card animations** (flip, move, etc.)
3. **Add particle effects** for winning
4. **Improve card visuals** (shadows, glows)

## Troubleshooting

### Cards appear as white rectangles
- Check if sprites are imported as Sprite (2D and UI)
- Verify sprites are in the correct folder
- Check console for "sprite not found" warnings

### Card back doesn't appear
- Make sure `card_back_blue.png` is in `Resources/Cards/Backs/`
- Or assign it manually in CardSpriteManager Inspector

### Performance issues
- Use Sprite Atlas
- Reduce sprite sizes (140x190 is enough)
- Enable texture compression

## Summary

**Easiest method:**
1. Download Kenney's card pack
2. Rename sprites to match pattern
3. Put in `Assets/Resources/Cards/Faces/`
4. Play and enjoy!

**What the code does automatically:**
- ✅ Loads sprites based on card suit and rank
- ✅ Caches sprites for better performance
- ✅ Handles missing sprites gracefully
- ✅ Works with Resources folder or manual assignment

You're now ready to see beautiful cards in your game! 🎴
