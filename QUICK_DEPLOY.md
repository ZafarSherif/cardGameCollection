# Quick Deployment Checklist 🚀

Get your game live in 15 minutes!

---

## ✅ Step 1: Disable Cheat Mode (5 min)

1. **Open Unity:**
   - Open `unity-games/CardGames/`

2. **Select SolitaireGameManager:**
   - Hierarchy → Select `SolitaireGameManager`

3. **Disable Cheat Mode:**
   - Inspector → Find "Cheat Mode (Testing)" section
   - **UNCHECK** the "Cheat Mode" checkbox
   - Save Scene (Cmd+S / Ctrl+S)

✅ **Verification:** Cheat mode checkbox is unchecked

---

## ✅ Step 2: Rebuild Unity for WebGL (5 min)

1. **In Unity:**
   - File → Build Settings
   - Platform: **WebGL** (should already be selected)
   - Click **Build** (or Build and Run if you want to test)

2. **Output location:**
   - Navigate to: `unity-games/builds/webgl/`
   - Unity will rebuild all files here

3. **Wait for build:**
   - Takes 2-5 minutes
   - Coffee break! ☕

✅ **Verification:** New files in `unity-games/builds/webgl/Build/` with today's timestamp

---

## ✅ Step 3: Deploy Unity to React Native (1 min)

Run the deployment script:

```bash
cd /Users/lazy/Desktop/Projects/AITries/TestingGame/CardGame
./deploy.sh
```

You should see:
```
✅ Deploy successful!
📁 Unity files copied to: mobile-app/public/unity/
```

✅ **Verification:** Files in `mobile-app/public/unity/Build/` updated

---

## ✅ Step 4: Install gh-pages (1 min)

```bash
cd mobile-app
npm install --save-dev gh-pages
```

If you get permissions error, try:
```bash
sudo npm install --save-dev gh-pages
```

✅ **Verification:** `gh-pages` in `package.json` devDependencies

---

## ✅ Step 5: Add Your GitHub Username (1 min)

Edit `mobile-app/package.json`:

Find line 3 (after `"version": "1.0.0"`), add:

```json
{
  "name": "mobile-app",
  "version": "1.0.0",
  "homepage": "https://ZafarSherif.github.io/cardGameCollection",
  ...
}
```

Replace `ZafarSherif` with your actual GitHub username.

✅ **Verification:** Homepage URL has your username

---

## ✅ Step 6: Build and Deploy to GitHub Pages (3 min)

```bash
cd mobile-app
npm run deploy
```

This will:
1. Build the React Native web app
2. Create a `gh-pages` branch
3. Push to GitHub

You'll see:
```
Published
```

✅ **Verification:** See "Published" message, no errors

---

## ✅ Step 7: Enable GitHub Pages (2 min)

1. **Go to your GitHub repo:**
   - https://github.com/ZafarSherif/cardGameCollection

2. **Settings tab** → Pages (left sidebar)

3. **Configure:**
   - Source: **Deploy from a branch**
   - Branch: **gh-pages** / **root**
   - Click **Save**

4. **Wait 2-5 minutes** for GitHub to deploy

✅ **Verification:** Green checkmark "Your site is live at..."

---

## ✅ Step 8: Test Your Live Site!

Visit:
```
https://ZafarSherif.github.io/cardGameCollection
```

**Test checklist:**
- [ ] Page loads
- [ ] Can see game list
- [ ] Click "How to Play" button - modal opens
- [ ] Language switcher works
- [ ] Click Solitaire game - game loads
- [ ] Cards are draggable
- [ ] Score, moves, timer work
- [ ] Audio plays on card flip/place
- [ ] New Game button works
- [ ] Undo button works
- [ ] Can win the game (win panel appears)

---

## 🎉 You're Live!

Share with friends:
```
https://ZafarSherif.github.io/cardGameCollection
```

---

## 🐛 Troubleshooting

### Unity game doesn't load on GitHub Pages

**Problem:** Blank iframe or 404 errors

**Fix:** Update `mobile-app/src/screens/GameScreen.tsx` line ~108:

Change:
```tsx
src="/unity/index.html"
```

To:
```tsx
src={`${process.env.PUBLIC_URL || ''}/unity/index.html`}
```

Then redeploy:
```bash
cd mobile-app
npm run deploy
```

### GitHub Pages shows 404

**Solutions:**
1. Wait 5-10 minutes (first deploy takes time)
2. Check Settings → Pages shows green "Your site is live"
3. Clear browser cache (Cmd+Shift+R / Ctrl+Shift+F5)
4. Check branch is `gh-pages` not `main`

### Build fails with "out of memory"

**Fix:** Increase Node memory:
```bash
export NODE_OPTIONS="--max-old-space-size=4096"
npm run deploy
```

---

## 📱 Share on Mobile

Your game works on mobile browsers too!

Just share the same link:
```
https://ZafarSherif.github.io/cardGameCollection
```

Friends can:
- Open in Safari (iOS)
- Open in Chrome (Android)
- Add to home screen for app-like experience

---

## 🔄 Making Updates

When you make changes:

1. **Update Unity:**
   - Make changes in Unity
   - Build WebGL
   - Run `./deploy.sh`

2. **Update React Native:**
   - Make changes in code
   - (No need to redeploy Unity)

3. **Deploy:**
   ```bash
   cd mobile-app
   npm run deploy
   ```

4. **Wait 2-3 minutes** for GitHub to update

---

## ✅ Success Checklist

Before sharing with friends:

- [ ] Cheat mode disabled
- [ ] Unity rebuilt
- [ ] Deployed to GitHub Pages
- [ ] Tested on desktop browser
- [ ] Tested on mobile browser
- [ ] All features working
- [ ] No console errors
- [ ] Audio works
- [ ] Win panel shows correctly

---

## 🎮 Ready to Share!

Your game is live at:
```
https://ZafarSherif.github.io/cardGameCollection
```

Get feedback and iterate! 🚀

**Next steps:**
- Add Firebase for analytics (later)
- Monitor usage
- Fix bugs based on feedback
- Add more games!
