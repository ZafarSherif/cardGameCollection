# Memory Match Game

## Card Sprites Setup

**IMPORTANT:** You need **at least 12 unique card sprites** to support all difficulty modes.

### Sprite Requirements:
- **Easy mode (4x4)**: 8 pairs = 8 unique sprites
- **Medium mode (5x4)**: 10 pairs = 10 unique sprites
- **Hard mode (6x4)**: 12 pairs = **12 unique sprites** ⚠️

### How to Add Card Sprites:

1. Open Unity
2. Select the **MemoryMatchGameManager** GameObject in the scene
3. In the Inspector, find **Card Front Sprites**
4. Set **Size** to **12** (or more)
5. Drag 12 different card images into the array slots

### Current Issue:
If you don't have 12 unique sprites, the game will **reuse sprites** with different pairIds, causing:
- Cards that look identical but aren't matches
- Confusing gameplay
- Failed match detection

### Testing:
After adding sprites, test each difficulty:
- Easy: Should have 8 distinct card designs
- Medium: Should have 10 distinct card designs
- Hard: Should have 12 distinct card designs

## Debug Logs

When testing buttons, check browser console for:
- `[React → Unity] sendToUnity called:` - Button clicked
- `[React → Unity] Sending message:` - Message being sent
- `[MemoryMatch] OnReactMessage called with JSON:` - Unity received message
- `[MemoryMatch] Executing newGame action` - Action executed

If buttons don't work, check if Unity is ready:
- `✅ Unity loaded successfully` should appear in console
- `✅ React Bridge ready for communication` should appear
