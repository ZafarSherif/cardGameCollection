# Next Steps in Unity - Card Sprite Integration

Your card sprites are ready! Follow these steps to complete the integration.

## Current Status

✅ All 52 card sprites added to correct folder
✅ Perfect naming convention (suits_rank.png)
✅ Correct dimensions (140x190)
✅ Card back sprite present
✅ CardSpriteManager code ready
✅ Editor automation script created

## Step-by-Step Instructions

### Step 1: Open Unity and Import Assets

1. **Open your Unity project** (if not already open)
2. **Click on Unity Editor** to bring it to focus
3. Unity will automatically detect the new sprites in `Assets/Resources/Cards/`
4. **Wait for import to complete** - you'll see a progress bar in the bottom right
5. The Editor script will automatically configure all sprite settings

### Step 2: Verify Import

Once import is complete, verify the sprites:

**Method 1: Visual Check**
1. In Project window, navigate to: `Assets → Resources → Cards → Faces`
2. Click on any sprite (e.g., `spades_ace`)
3. You should see a preview of the card in the Inspector
4. Verify settings in Inspector:
   - **Texture Type**: Sprite (2D and UI)
   - **Sprite Mode**: Single
   - **Pixels Per Unit**: 100
   - **Filter Mode**: Bilinear

**Method 2: Use Menu Tools**
1. In Unity menu, click **CardGames → Verify Card Sprites**
2. Check Console - should show: "✓ All card sprites verified successfully!"
3. If you see any missing sprites, check the Console for details

### Step 3: Test Sprite Loading

Test if sprites can be loaded properly:

1. In Unity menu, click **CardGames → Test Sprite Loading**
2. Check Console - should show successful loads:
   ```
   ✓ Loaded: Cards/Faces/spades_ace (140x190)
   ✓ Loaded: Cards/Faces/hearts_king (140x190)
   ...
   ```

### Step 4: Test in Play Mode

Now test the game with real card sprites:

1. **Open your Solitaire scene** (if not already open)
2. **Press Play** (or Ctrl+P / Cmd+P)
3. Watch the Console for sprite loading messages:
   ```
   Card front sprite set: Ace of Spades
   Card front sprite set: Two of Hearts
   ...
   ```
4. **Visually verify**:
   - Cards should show actual card faces (not just squares)
   - Card backs should show the blue pattern
   - Drag and drop should work with the new sprites

### Step 5: Build for WebGL

Once everything works in Play mode:

1. **File → Build Settings**
2. Select **WebGL** (or **Web** in Unity 6)
3. Click **Build**
4. Choose output folder: `unity-games/builds/webgl/`
5. Wait for build to complete (2-5 minutes)

### Step 6: Copy to React Native

After build completes:

```bash
cd /Users/lazy/Desktop/Projects/AITries/TestingGame/CardGame

# Remove old build
rm -rf mobile-app/public/unity

# Copy new build
cp -r unity-games/builds/webgl/webgl mobile-app/public/unity
```

### Step 7: Test in Browser

Your React Native web app should already be running. Just refresh:

1. Go to your browser (should be at `http://localhost:8081` or similar)
2. **Refresh the page** (Cmd+R / Ctrl+R)
3. Click on a game
4. **You should see real card sprites!** 🎴

## Troubleshooting

### Problem: Sprites don't show in Unity Play mode

**Solution 1: Reimport sprites**
- Unity menu → CardGames → Reimport All Card Sprites
- Wait for reimport to complete
- Try Play mode again

**Solution 2: Check console for errors**
- Look for "sprite not found" warnings
- Verify the path and naming

### Problem: "Sprite not found" in Console

**Possible causes:**
1. Sprites not in Resources folder - must be `Assets/Resources/Cards/`
2. Incorrect naming - must match: `{suit}_{rank}.png`
3. Not imported as Sprite type - use Reimport menu item

### Problem: Cards show as squares in Play mode

**Check:**
1. Are sprites loaded? Check Console for loading messages
2. Is CardSpriteManager working? You should see debug logs
3. Are SpriteRenderers on the Card prefab? Check in Inspector

### Problem: WebGL build doesn't show cards

**Solution:**
1. Make sure sprites are in Resources folder (not just any folder)
2. Rebuild WebGL - Resources folder content is baked into build
3. Ensure proper copy to mobile-app/public/unity/
4. Hard refresh browser (Cmd+Shift+R / Ctrl+Shift+R)

## Menu Tools Reference

The Editor script adds these helpful menu items under **CardGames** menu:

### CardGames → Verify Card Sprites
- Checks if all 52 cards + card back are present
- Reports any missing sprites
- Shows summary in Console

### CardGames → Test Sprite Loading
- Tests loading a sample of cards
- Verifies Resources.Load() works
- Shows sprite dimensions

### CardGames → Reimport All Card Sprites
- Forces reimport of all sprites in Cards folder
- Useful if import settings get messed up
- Applies correct settings automatically

## Expected Console Output

When everything is working correctly, you should see:

### During Import:
```
[CardSpriteImporter] Configured sprite: Assets/Resources/Cards/Faces/spades_ace.png
[CardSpriteImporter] Configured sprite: Assets/Resources/Cards/Faces/hearts_king.png
... (52 times)
[CardSpriteImporter] Configured sprite: Assets/Resources/Cards/Backs/card_back_blue.png
```

### During Play:
```
Card front sprite set: Ace of Spades
Card front sprite set: King of Hearts
Card front sprite set: Ten of Diamonds
... (52 times)
```

### After Verification:
```
===== Card Sprite Verification =====
Card Faces: 52/52
Card Backs: 1/1
✓ All card sprites verified successfully!
====================================
```

## Success Checklist

Before moving to the next step, verify:

- [ ] All sprites imported in Unity (no import errors)
- [ ] Sprites show in Project window preview
- [ ] "Verify Card Sprites" shows 52/52 cards
- [ ] "Test Sprite Loading" succeeds
- [ ] Play mode shows actual card graphics
- [ ] Dragging cards works with new sprites
- [ ] WebGL build includes new sprites
- [ ] Browser shows cards in React Native app

## What's Next?

Once card sprites are working:

1. **Visual Polish**
   - Add card flip animations
   - Add smooth movement tweens
   - Add particle effects for winning

2. **Game Features**
   - Add undo/redo functionality
   - Add hint system
   - Add auto-complete

3. **More Games**
   - Start building Poker
   - Start building Blackjack

4. **Backend Integration**
   - Set up Firebase
   - Add user authentication
   - Save game progress

You're almost there! 🎮
