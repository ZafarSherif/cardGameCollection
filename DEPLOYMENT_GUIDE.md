# Deployment Guide - Unity to React Native

This guide covers building Unity WebGL and deploying to React Native.

## Quick Deployment (After Unity Build)

If you've already built the Unity project:

```bash
./deploy.sh
```

That's it! The script will copy the Unity build to React Native.

---

## Full Deployment Process

### Step 1: Build Unity Project for WebGL

1. **Open Unity Project**
   - Open Unity Hub
   - Open project at: `unity-games/CardGames/`

2. **Open Build Settings**
   - File → Build Settings
   - Platform: **WebGL** (or **Web** in Unity 6)
   - If not selected, click "Switch Platform"

3. **Configure Player Settings** (First time only)
   - Click "Player Settings..."
   - **Publishing Settings:**
     - Compression Format: **Disabled** (or Gzip for production)
     - Data caching: ✅ Enabled
     - Decompression Fallback: ✅ Enabled
   - **Resolution and Presentation:**
     - Default Canvas Width: 1920
     - Default Canvas Height: 1080
     - Run In Background: ✅ Checked

4. **Build the Project**
   - Close Player Settings
   - Click **Build** (not "Build and Run")
   - Choose folder: `unity-games/builds/webgl/`
   - Click "Select Folder"
   - Wait 2-5 minutes for build to complete

5. **Verify Build Success**
   - Console should show: "Build completed successfully"
   - Check folder: `unity-games/builds/webgl/`
   - Should contain:
     - Build/ folder
     - TemplateData/ folder
     - index.html

### Step 2: Deploy to React Native

Run the deployment script:

```bash
cd /Users/lazy/Desktop/Projects/AITries/TestingGame/CardGame
./deploy.sh
```

This will:
- Remove old Unity build from React Native
- Copy new build to `mobile-app/public/unity/`
- Verify deployment success

### Step 3: Test in Browser

1. **Make sure dev server is running:**
   ```bash
   cd mobile-app
   npm run web
   ```

2. **Open browser** to displayed URL (usually http://localhost:8081)

3. **Hard refresh** to clear cache:
   - Mac: Cmd + Shift + R
   - Windows: Ctrl + Shift + F5

4. **Navigate to Solitaire game** and test!

---

## Testing Checklist

After deployment, verify these features:

### Core Gameplay
- [ ] Game loads with all cards visible
- [ ] Can click stock pile to draw cards
- [ ] Can drag and drop cards
- [ ] Cards return to original position on invalid moves
- [ ] Face-down cards flip automatically when revealed
- [ ] Stock pile recycles and shuffles when empty

### Game Rules
- [ ] Can stack cards in descending rank (6 on 7)
- [ ] Must alternate colors (red on black)
- [ ] King can go on empty tableau pile
- [ ] Ace can go on empty foundation pile
- [ ] Can build foundation in ascending order (A→K)

### Updated Features (Latest Changes)
- [ ] Empty piles are clickable (for Kings/Aces)
- [ ] Stock pile always clickable (for dealing)
- [ ] Cards on tableau/foundation/waste don't block pile clicks
- [ ] Recycled waste cards are shuffled (not in same order)

---

## Common Issues and Solutions

### Issue: "Build failed" in Unity

**Solution:**
- Check Console for errors
- Fix any script errors
- Try: Unity → Assets → Reimport All
- Clean build folder and rebuild

### Issue: "Deploy script fails - build not found"

**Solution:**
- Make sure you built Unity first
- Check build path: `unity-games/builds/webgl/`
- Verify folder contains Build/ and index.html

### Issue: "Game doesn't load in browser"

**Solution:**
- Hard refresh browser (Cmd+Shift+R)
- Check browser Console (F12) for errors
- Make sure dev server is running (`npm run web`)
- Check that Unity files are in `mobile-app/public/unity/`

### Issue: "Cards don't show up"

**Solution:**
- Check card sprites are in Unity Resources folder
- Rebuild Unity WebGL
- Check browser Console for loading errors
- Verify Build/webgl.data file is large (20-50 MB)

### Issue: "Old version still showing"

**Solution:**
- Hard refresh browser
- Clear browser cache completely
- Stop dev server, run deploy.sh again, restart server

---

## File Structure After Deployment

```
CardGame/
├── mobile-app/
│   └── public/
│       └── unity/              ← Unity WebGL build
│           ├── Build/
│           │   ├── webgl.data
│           │   ├── webgl.framework.js
│           │   ├── webgl.loader.js
│           │   └── webgl.wasm
│           ├── TemplateData/
│           └── index.html
├── unity-games/
│   ├── CardGames/              ← Unity project source
│   └── builds/
│       └── webgl/              ← Unity build output
└── deploy.sh                   ← Deployment script
```

---

## Rebuilding After Changes

When you make changes to Unity scripts (C#):

1. **Save in Unity** (Cmd+S)
2. **Build again** (File → Build Settings → Build)
3. **Run deploy script**: `./deploy.sh`
4. **Hard refresh browser**

---

## Performance Notes

Expected performance in browser:
- **Load time**: 3-10 seconds (first load)
- **FPS**: 60fps smooth gameplay
- **Memory**: ~100-200 MB
- **Build size**: ~30-50 MB total

If performance is poor:
- Check browser Console for errors
- Try different browser (Chrome recommended)
- Enable compression in Unity build settings

---

## Production Optimization (Optional)

For production deployment (not needed for development):

### 1. Enable Gzip Compression
```
Unity → Player Settings → Publishing Settings
Compression Format: Gzip
```
Reduces file size by 70-80%

### 2. Strip Engine Code
```
Unity → Player Settings → Other Settings
Strip Engine Code: ✅ Enabled
```
Removes unused Unity features

### 3. Minify Build
```
Unity → Build Settings
Development Build: ❌ Unchecked
```
Smaller file size, harder to debug

---

## Next Steps After Deployment

1. ✅ **Unity WebGL working in browser** ← You are here!
2. 🔄 **Set up Git repository** (preserve your work)
3. 🚀 **Push to GitHub** (backup and share)
4. 📱 **Mobile builds** (iOS/Android) - Future
5. 🎨 **Polish and features** - Future

---

## Quick Reference Commands

```bash
# Deploy after Unity build
./deploy.sh

# Start React Native dev server
cd mobile-app && npm run web

# Check Unity build exists
ls unity-games/builds/webgl/Build/

# Check React Native has Unity files
ls mobile-app/public/unity/Build/
```

---

**You're all set! Build Unity, run `./deploy.sh`, and test in browser!** 🚀
