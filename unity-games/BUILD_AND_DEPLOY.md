# Build and Deploy to React Native

Everything is working in Unity! Now let's get it into your React Native app.

## Pre-Build Checklist

Before building, verify these in Unity:

- [ ] All pile colliders added (13 total)
- [ ] ClickableStockPile component on Stock Pile
- [ ] Card sprites loading correctly
- [ ] Drag and drop working
- [ ] Stock pile clicking working
- [ ] Cards flip automatically
- [ ] No errors in Console

## Build for WebGL (5 minutes)

### Step 1: Open Build Settings

1. **File → Build Settings**
2. Select **WebGL** (or **Web** in Unity 6)
3. Click **Player Settings...**

### Step 2: Verify Settings

In Player Settings:

**Publishing Settings:**
- Compression Format: **Disabled** (or Gzip)
- Data caching: Enabled
- Decompression Fallback: Enabled

**Resolution and Presentation:**
- Default Canvas Width: 1920
- Default Canvas Height: 1080
- Run In Background: ✅ Checked

### Step 3: Build

1. **Close Player Settings**
2. **Click Build** (not Build and Run)
3. **Choose folder**: `unity-games/builds/webgl/`
4. **Click Select Folder**
5. **Wait** (2-5 minutes)

**Build complete when you see:**
```
Build completed successfully
```

### Step 4: Verify Build Output

Check that these files exist:
```
unity-games/builds/webgl/webgl/
  ├── Build/
  │   ├── webgl.data
  │   ├── webgl.framework.js
  │   ├── webgl.loader.js
  │   └── webgl.wasm
  ├── StreamingAssets/
  ├── TemplateData/
  └── index.html
```

## Deploy to React Native (1 minute)

### Step 1: Remove Old Build

```bash
cd /Users/lazy/Desktop/Projects/AITries/TestingGame/CardGame

# Remove old Unity build
rm -rf mobile-app/public/unity
```

### Step 2: Copy New Build

```bash
# Copy new build
cp -r unity-games/builds/webgl/webgl mobile-app/public/unity
```

### Step 3: Verify Copy

```bash
# Check files exist
ls mobile-app/public/unity/Build/
```

Should output:
```
webgl.data
webgl.framework.js
webgl.loader.js
webgl.wasm
```

## Test in Browser (10 seconds)

### If React Native is already running:

1. Go to your browser
2. Should be at: `http://localhost:8081` or similar
3. **Hard refresh**: Cmd+Shift+R (Mac) or Ctrl+Shift+F5 (Windows)
4. Click on **Solitaire** game
5. **Play the game!** 🎉

### If React Native is not running:

```bash
cd mobile-app
npm run web
```

Then open browser to displayed URL.

## Testing Checklist

Test these features in the browser:

### Basic Functionality
- [ ] Game loads (cards appear)
- [ ] Can drag face-up cards
- [ ] Can drop on valid piles
- [ ] Cards return on invalid drop
- [ ] Can click stock pile to draw
- [ ] Stock recycles when empty
- [ ] Bottom cards flip automatically

### Game Rules
- [ ] Can stack descending rank (6 on 7)
- [ ] Must alternate colors (red on black)
- [ ] King goes on empty tableau
- [ ] Ace goes on empty foundation
- [ ] Can build foundation (A→2→3→...→K)

### Visual
- [ ] Card sprites show correctly
- [ ] Card backs show correctly
- [ ] Cards position properly in piles
- [ ] No overlapping issues
- [ ] Smooth dragging

### UI Integration
- [ ] Score updates (check React Native header)
- [ ] Moves count updates
- [ ] Back button works
- [ ] Game complete message shows

## Common Build Issues

### Issue 1: Build fails with compression error

**Solution:**
- Player Settings → Publishing Settings
- Compression Format: **Disabled**
- Rebuild

### Issue 2: "Out of memory" error

**Solution:**
- Unity → Preferences → GI Cache
- Click "Clean Cache"
- Close Unity and reopen
- Try build again

### Issue 3: Build succeeds but game doesn't load

**Solution:**
- Check browser Console (F12)
- Look for CORS errors
- Make sure you're using `npm run web` (has proper server)
- Hard refresh (Cmd+Shift+R)

### Issue 4: Cards don't show in browser

**Solution:**
- Resources folder must be in build
- Check: `mobile-app/public/unity/Build/webgl.data` size
- Should be large (20-50 MB with sprites)
- If small (< 5 MB), sprites not included
- Rebuild WebGL

## Performance in Browser

Expected performance:
- **Load time**: 3-10 seconds (first load)
- **FPS**: 60 fps smooth
- **Memory**: ~100-200 MB
- **File size**: ~30-50 MB total

If slower:
- Check browser Console for errors
- Test in different browser
- Check network tab in DevTools

## Build Optimization (Optional)

For faster loading in production:

### 1. Enable Gzip Compression
```
Player Settings → Publishing Settings
Compression Format: Gzip
```
*Reduces file size by 70-80%*

### 2. Strip Engine Code
```
Player Settings → Other Settings
Strip Engine Code: Enabled
```
*Removes unused Unity features*

### 3. Code Optimization
```
Player Settings → Other Settings
Scripting Backend: IL2CPP
IL2CPP Code Generation: Faster runtime
```
*Faster execution, slower build*

### 4. Asset Optimization
- Use Sprite Atlases (reduces draw calls)
- Compress textures
- Remove unused assets

## File Size Breakdown

**Typical WebGL build:**
```
webgl.data        ~20-40 MB  (Assets, Resources)
webgl.wasm        ~10-20 MB  (Game code)
webgl.framework   ~5-10 MB   (Unity engine)
webgl.loader      ~500 KB    (Loader script)
```

**With compression (Gzip):**
```
webgl.data.gz     ~5-10 MB   (70% smaller)
webgl.wasm.gz     ~3-5 MB    (70% smaller)
webgl.framework.gz ~2-3 MB   (70% smaller)
```

## Deployment Checklist

Before showing others:

- [ ] Build without errors
- [ ] Tested all features working
- [ ] No console errors in browser
- [ ] Good performance (smooth 60fps)
- [ ] Sprites loading correctly
- [ ] Game rules working correctly
- [ ] UI integration working
- [ ] Mobile responsive (if targeting mobile web)

## What's Next?

After deploying to React Native web:

1. **Mobile builds** (iOS/Android)
   - Use react-native-unity-view
   - Different integration than web

2. **Backend integration**
   - Firebase for save game
   - User authentication
   - Leaderboards

3. **More games**
   - Poker
   - Blackjack
   - Other card games

4. **Polish**
   - Animations
   - Sound effects
   - Particle effects
   - Better UI

5. **Features**
   - Undo/Redo
   - Hints
   - Statistics
   - Achievements

## Quick Deploy Script

Save this as `deploy.sh`:

```bash
#!/bin/bash

echo "🎮 Deploying Unity to React Native..."

# Remove old build
rm -rf mobile-app/public/unity

# Copy new build
cp -r unity-games/builds/webgl/webgl mobile-app/public/unity

# Verify
if [ -f "mobile-app/public/unity/index.html" ]; then
    echo "✅ Deploy successful!"
    echo "🌐 Refresh your browser to see changes"
else
    echo "❌ Deploy failed!"
    exit 1
fi
```

Run with:
```bash
chmod +x deploy.sh
./deploy.sh
```

---

**You're ready to build and deploy! The game is fully functional.** 🚀

After testing in browser, you can share the link with others!
