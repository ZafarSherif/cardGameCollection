# Quick Reference - Card Sprites Integration

## ✅ What's Ready

Your setup is complete! Here's what we have:

### Code Ready
- ✅ **CardSpriteManager.cs** - Automatically loads sprites by suit and rank
- ✅ **Card.cs** - Updated to use CardSpriteManager
- ✅ **SolitaireGameManager.cs** - Simplified initialization
- ✅ **CardSpriteImporter.cs** - Auto-configures import settings
- ✅ **Editor menu tools** - For verification and testing

### Assets Ready
- ✅ **52 card face sprites** in `Assets/Resources/Cards/Faces/`
- ✅ **Card back sprite** in `Assets/Resources/Cards/Backs/`
- ✅ **Perfect naming** (e.g., `spades_ace.png`, `hearts_10.png`)
- ✅ **Perfect dimensions** (140x190 RGBA PNG)

## 🎯 What You Need to Do

### In Unity (5 minutes):

1. **Open Unity** - Assets will auto-import
2. **Run verification**: CardGames → Verify Card Sprites
3. **Press Play** - See cards in action
4. **Build WebGL** - File → Build Settings → Build

### In Terminal (1 minute):

```bash
# Copy Unity build to React Native
cd /Users/lazy/Desktop/Projects/AITries/TestingGame/CardGame
rm -rf mobile-app/public/unity
cp -r unity-games/builds/webgl/webgl mobile-app/public/unity
```

### In Browser (10 seconds):

1. Refresh browser (Cmd+R)
2. Click on Solitaire
3. **See real cards!** 🎴

## 📋 Unity Menu Tools

Under **CardGames** menu:

| Tool | What It Does |
|------|-------------|
| **Verify Card Sprites** | Checks all 52 cards are present |
| **Test Sprite Loading** | Tests if Resources.Load() works |
| **Reimport All Card Sprites** | Forces reimport with correct settings |

## 🔍 Quick Checks

### In Unity Console, you should see:

**During import:**
```
[CardSpriteImporter] Configured sprite: Assets/Resources/Cards/Faces/spades_ace.png
```

**During Play:**
```
Card front sprite set: Ace of Spades
Card front sprite set: King of Hearts
```

**After Verify:**
```
Card Faces: 52/52 ✓
Card Backs: 1/1 ✓
All card sprites verified successfully!
```

## 🐛 Common Issues

| Problem | Quick Fix |
|---------|-----------|
| Sprites don't import | Check: Is folder named exactly `Resources`? |
| Cards show as squares | Run: CardGames → Reimport All Card Sprites |
| "Sprite not found" error | Verify: Naming matches `{suit}_{rank}.png` |
| WebGL doesn't show cards | Rebuild WebGL + copy to public folder |

## 📁 Folder Structure

```
Assets/
  Resources/           ← MUST be named exactly this
    Cards/
      Faces/          ← 52 card PNG files
        clubs_ace.png
        clubs_2.png
        ...
        spades_king.png
      Backs/          ← Card back PNG file
        card_back_blue.png
```

## 🎨 Sprite Naming Pattern

```
{suit}_{rank}.png

Suits: clubs, diamonds, hearts, spades (lowercase)
Ranks: ace, 2-10, jack, queen, king (lowercase)

Examples:
✅ clubs_ace.png
✅ hearts_10.png
✅ spades_king.png
❌ Clubs_Ace.png (wrong case)
❌ club_ace.png (wrong suit name)
❌ hearts_ten.png (use "10" not "ten")
```

## 🚀 Expected Result

**Before (with squares):**
```
[■] [■] [■] [■]
```

**After (with real cards):**
```
[A♠] [K♥] [10♦] [J♣]
```

## 💡 Tips

1. **Auto-import is automatic** - Just open Unity and wait
2. **Use menu tools** - They save time debugging
3. **Check Console** - It tells you what's happening
4. **Resources folder is magic** - Unity bakes it into builds

## 📝 File Checklist

Copy this checklist into your terminal to verify everything:

```bash
# Go to Unity project
cd /Users/lazy/Desktop/Projects/AITries/TestingGame/CardGame/unity-games/CardGames

# Check card faces (should output: 52)
ls Assets/Resources/Cards/Faces/*.png | wc -l

# Check card back (should output: card_back_blue.png)
ls Assets/Resources/Cards/Backs/*.png

# Check if Editor script exists (should output the filename)
ls Assets/Scripts/Editor/CardSpriteImporter.cs

# Check if CardSpriteManager exists (should output the filename)
ls Assets/Scripts/Core/CardSpriteManager.cs
```

All commands should succeed! ✅

## 🎯 Success Criteria

You'll know it's working when:

1. ✅ Unity Console shows no import errors
2. ✅ "Verify Card Sprites" reports 52/52
3. ✅ Play mode shows actual card graphics (not squares)
4. ✅ Card backs show blue pattern
5. ✅ Drag and drop works with new graphics
6. ✅ WebGL build includes card graphics
7. ✅ Browser shows cards in React Native app

## ⏭️ Next Steps After Integration

Once cards are showing:

**Short term (polish):**
- Add card flip animations
- Add smooth movement
- Add particle effects
- Add sound effects

**Medium term (features):**
- Implement undo/redo
- Add hint system
- Add auto-complete
- Add game statistics

**Long term (expand):**
- Build Poker game
- Build Blackjack game
- Add multiplayer
- Deploy to production

---

**Need help?** Check `NEXT_STEPS_IN_UNITY.md` for detailed instructions.

**Everything working?** Time to add visual polish and animations! 🎨
