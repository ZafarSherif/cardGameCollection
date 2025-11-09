# Unity WebGL Build Instructions
## For Hybrid React Native + Unity Architecture

## 🎯 Prerequisites

1. Unity project has ReactBridge component in scene
2. ReactBridge WebGL template created (in Assets/WebGLTemplates/ReactBridge/)
3. Canvas UI removed from scene

---

## 📋 Step-by-Step Build Process

### 1. Select WebGL Template

1. Open Unity Editor
2. Go to **File → Build Settings**
3. Select **WebGL** platform
4. Click **Player Settings** button
5. In Inspector, find **Resolution and Presentation** section
6. Under **WebGL Template**, select **ReactBridge** from dropdown
7. Close Player Settings

### 2. Configure Build Settings

In **File → Build Settings**:
- ✅ Platform: **WebGL**
- ✅ Scenes in Build: Add **Solitaire** scene (drag from Project panel)
- ✅ Development Build: **OFF** (for production) or **ON** (for debugging)

### 3. Build the Game

1. Click **Build** (NOT "Build And Run")
2. Navigate to: `CardGame/mobile-app/public/`
3. Select the `unity` folder
4. In "Save As" field, type: `Build`
5. Click Save
6. Wait for build to complete (2-5 minutes)

### 4. Fix Unity's Nested Folder Structure

Unity creates a nested `Build/Build/` structure. Run this script to fix it:

```bash
cd mobile-app
./fix-unity-build.sh
```

**Expected output structure after fix:**
```
mobile-app/public/unity/
├── Build/
│   ├── Build.data
│   ├── Build.framework.js
│   ├── Build.loader.js
│   └── Build.wasm
└── index.html
```

---

## 🔍 Verify Build

After building, check:

### ✅ Files Exist:
```bash
cd mobile-app/public/unity
ls -la
# Should see: Build/, index.html, StreamingAssets/
```

### ✅ Index.html has React Bridge:
```bash
grep "SendMessageToReact" index.html
# Should output: window.SendMessageToReact = function(message) {
```

---

## 🚀 Test Locally

### Option 1: React Native Dev Server (Recommended)

```bash
cd mobile-app
npm start
# Open web browser to http://localhost:8081
# Navigate to game
```

### Option 2: Local HTTP Server

```bash
cd mobile-app/public/unity
python3 -m http.server 8000
# Open browser to http://localhost:8000
```

**Test Checklist:**
- [ ] Game loads without errors
- [ ] Cards are visible and positioned correctly
- [ ] Console shows: `[ReactBridge] Initialized`
- [ ] Console shows: `[ReactBridge → React] {"type":"gameReady"}`
- [ ] Can drag cards
- [ ] Score/moves update (check console for events)

---

## 🐛 Common Build Issues

### Issue: "ReactBridge template not found"
**Fix:**
- Make sure folder is: `Assets/WebGLTemplates/ReactBridge/`
- Restart Unity Editor
- Check Player Settings again

### Issue: Build fails with errors
**Fix:**
- Clear browser cache
- Delete old Build folder before rebuilding
- Check Unity Console for script errors

### Issue: Unity loads but React can't communicate
**Fix:**
- Verify index.html contains `SendMessageToReact` function
- Check browser console for JavaScript errors
- Make sure ReactBridge GameObject exists in scene

### Issue: Cards not visible
**Fix:**
- Check Camera position (should be at 0,0,-10)
- Verify ResponsiveLayout is positioning piles
- Check console for positioning logs

---

## 📦 Production Build

For final deployment:

1. **Unity Build Settings:**
   - ❌ Development Build: OFF
   - ❌ Autoconnect Profiler: OFF
   - ✅ Compression Format: Brotli (smaller files)

2. **Build to:** `mobile-app/public/unity/`

3. **Deploy React Native:**
```bash
cd mobile-app
npm run build:web
npm run deploy
```

---

## 🎮 Adding ReactBridge GameObject (if not already done)

If you haven't added ReactBridge to your scene:

1. In Hierarchy, right-click → **Create Empty**
2. Rename to `ReactBridge`
3. In Inspector, click **Add Component**
4. Search for **React Bridge**
5. Add the component
6. **Save scene** (Ctrl/Cmd + S)

The ReactBridge component should be at root level, not a child of anything.

---

## 📊 Build Size Optimization (Optional)

To reduce WebGL build size:

**Player Settings → Publishing Settings:**
- Compression Format: **Brotli** (best compression)
- Code Optimization: **Shorter Build Time** (dev) or **Faster Runtime** (prod)
- Enable Exceptions: **None** (smallest size)

**Quality Settings:**
- Remove unused quality levels
- Keep only "Medium" or "Low" for web

**Audio:**
- Compress audio clips
- Use Vorbis format

**Expected sizes:**
- Development build: 15-25 MB
- Production build: 8-15 MB (with Brotli)

---

## ✅ Success Criteria

Your build is ready when:
- ✅ Game loads in browser without errors
- ✅ Console shows ReactBridge messages
- ✅ Cards render and respond to clicks/drags
- ✅ React Native receives game state updates
- ✅ Buttons send actions to Unity
- ✅ Win modal appears when game is won

---

## 🔗 Next Steps

After successful build:
1. Test in React Native web version
2. Test portrait/landscape rotation
3. Deploy to GitHub Pages
4. Test on real mobile device

---

## 💡 Tips

- **Always test locally before deploying**
- **Clear browser cache** if game doesn't update
- **Check browser console** for communication logs
- **Use Development Build** for debugging
- **Rebuild Unity** if you change scripts or scene
