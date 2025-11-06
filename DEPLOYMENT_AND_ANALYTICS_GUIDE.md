# Deployment & Analytics Guide

Complete guide for deploying to GitHub Pages and setting up analytics.

---

## 📊 Analytics Setup (Plausible - Privacy-Friendly)

### Why Plausible?
- ✅ **Privacy-friendly** - No cookies, GDPR compliant
- ✅ **Lightweight** - <1KB script
- ✅ **Simple** - Easy dashboard
- ✅ **Free tier** - Up to 10K pageviews/month

### Option 1: Use Plausible Cloud (Recommended)

1. **Sign up at [plausible.io](https://plausible.io)**
   - Free 30-day trial
   - Then $9/month for 10K pageviews

2. **Add your domain**
   - In Plausible dashboard, add: `yourusername.github.io`

3. **Get your tracking script**
   - Copy the script tag they provide

4. **Add script to your app**
   - Create `mobile-app/public/index.html`:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Card Games Collection</title>

    <!-- Plausible Analytics -->
    <script defer data-domain="yourusername.github.io" src="https://plausible.io/js/script.js"></script>
</head>
<body>
    <div id="root"></div>
</body>
</html>
```

### Option 2: Self-Host Plausible (Free)

Follow [Plausible self-hosting guide](https://plausible.io/docs/self-hosting)

### Option 3: Google Analytics 4 (Alternative)

If you prefer Google Analytics:

1. Create GA4 property
2. Get Measurement ID (G-XXXXXXXXXX)
3. Add to `mobile-app/public/index.html`:

```html
<!-- Google tag (gtag.js) -->
<script async src="https://www.googletagmanager.com/gtag/js?id=G-XXXXXXXXXX"></script>
<script>
  window.dataLayer = window.dataLayer || [];
  function gtag(){dataLayer.push(arguments);}
  gtag('js', new Date());
  gtag('config', 'G-XXXXXXXXXX');
</script>
```

### Analytics Events Being Tracked

The app automatically tracks:

- **app_open** - When app/page opens
- **game_start** - When a game starts (with game_type)
- **game_complete** - When game finishes (won/lost, score, time)
- **game_quit** - When user quits mid-game
- **language_change** - When user changes language

View these in Plausible dashboard under "Goals" section.

---

## 🚀 GitHub Pages Deployment

### Step 1: Prepare Your Repository

1. **Make sure your repo is public** (required for free GitHub Pages)
   - Go to repo Settings → Danger Zone → Change visibility

2. **Update package.json** with homepage:

```json
{
  "name": "mobile-app",
  "version": "1.0.0",
  "homepage": "https://yourusername.github.io/cardGameCollection",
  ...
}
```

Replace `yourusername` with your GitHub username.

### Step 2: Install gh-pages Package

```bash
cd mobile-app
npm install --save-dev gh-pages
```

### Step 3: Add Deployment Scripts

Update `mobile-app/package.json` scripts section:

```json
{
  "scripts": {
    "start": "expo start",
    "android": "expo start --android",
    "ios": "expo start --ios",
    "web": "expo start --web",
    "build:web": "expo export:web",
    "predeploy": "npm run build:web",
    "deploy": "gh-pages -d web-build"
  }
}
```

### Step 4: Build and Deploy

```bash
cd mobile-app

# Build the web version
npm run build:web

# Deploy to GitHub Pages
npm run deploy
```

This will:
1. Build the app to `web-build` folder
2. Create a `gh-pages` branch
3. Push the build files to that branch

### Step 5: Enable GitHub Pages

1. Go to your repo on GitHub
2. Settings → Pages
3. Source: Deploy from a branch
4. Branch: `gh-pages` / `root`
5. Click Save

### Step 6: Wait and Test

- GitHub Pages usually takes 2-5 minutes to deploy
- Visit: `https://yourusername.github.io/cardGameCollection`
- Share this URL with friends!

---

## 🔧 Important Before Deploying

### 1. Disable Cheat Mode

**In Unity Editor:**
1. Open Solitaire scene
2. Select `SolitaireGameManager`
3. **Uncheck "Cheat Mode"** in Inspector
4. Save scene
5. Build WebGL again
6. Run `./deploy.sh` from project root

### 2. Test Locally First

```bash
cd mobile-app
npm run build:web
npx serve web-build
```

Open http://localhost:3000 and test everything works.

### 3. Update Unity Build Path (if needed)

If Unity files aren't loading:

Check `mobile-app/src/screens/GameScreen.tsx` line ~108:

```tsx
<iframe
  src="/unity/index.html"  // or "/cardGameCollection/unity/index.html" for GitHub Pages
  ...
/>
```

For GitHub Pages with subdirectory, update to:
```tsx
src={`${process.env.PUBLIC_URL}/unity/index.html`}
```

---

## 📱 Mobile Build Setup (iOS/Android)

### Prerequisites

1. **Install EAS CLI:**
```bash
npm install -g eas-cli
```

2. **Create Expo account:**
```bash
eas login
```

### Configure Project

1. **Create eas.json:**

```bash
cd mobile-app
eas build:configure
```

This creates `eas.json`:

```json
{
  "build": {
    "preview": {
      "android": {
        "buildType": "apk"
      },
      "ios": {
        "simulator": true
      }
    },
    "production": {}
  }
}
```

2. **Update app.json with identifiers:**

```json
{
  "expo": {
    "name": "Card Games Collection",
    "slug": "card-games",
    "ios": {
      "bundleIdentifier": "com.yourname.cardgames",
      "supportsTablet": true
    },
    "android": {
      "package": "com.yourname.cardgames",
      "adaptiveIcon": {
        "foregroundImage": "./assets/adaptive-icon.png",
        "backgroundColor": "#ffffff"
      }
    }
  }
}
```

### Build for Android (APK for Testing)

```bash
cd mobile-app
eas build --platform android --profile preview
```

This will:
- Build in the cloud (free for <30 builds/month)
- Give you a download link for APK
- You can share this APK with friends

### Build for iOS (Simulator)

```bash
eas build --platform ios --profile preview
```

### Production Builds (App Stores)

For actual app store submission:

```bash
# Android
eas build --platform android --profile production

# iOS (requires Apple Developer account $99/year)
eas build --platform ios --profile production
```

---

## 🔄 Deployment Workflow

### Typical Development Cycle:

1. **Make changes** in Unity or React Native
2. **Test locally:**
   ```bash
   # Test Unity in browser
   open unity-games/builds/webgl/index.html

   # Test React Native
   cd mobile-app && npm start
   ```

3. **Build Unity** (if changed):
   - Unity → File → Build Settings → WebGL → Build
   - Output to: `unity-games/builds/webgl/`

4. **Deploy Unity to React Native:**
   ```bash
   ./deploy.sh
   ```

5. **Build and deploy to GitHub Pages:**
   ```bash
   cd mobile-app
   npm run deploy
   ```

6. **Share with friends:**
   - Web: `https://yourusername.github.io/cardGameCollection`
   - Android APK: Share the download link from EAS build
   - iOS TestFlight: Set up after production build

---

## 📈 Monitoring & Analytics

### Check Analytics

**Plausible Dashboard:**
- Visit plausible.io/yourdomain.com
- See real-time visitors
- Top pages visited
- Device types
- Countries
- Custom events (game_start, game_complete, etc.)

**Console Logs (Development):**
```javascript
// In browser console, you'll see:
[Analytics] Event: app_open {platform: "web", version: "..."}
[Analytics] Event: game_start {game_type: "solitaire"}
[Analytics] Event: game_complete {game_type: "solitaire", won: "true", score: 350}
```

### Privacy Considerations

**What we track:**
- ✅ Page views
- ✅ Game starts/completions
- ✅ Device type (mobile/desktop)
- ✅ Country (anonymized IP)

**What we DON'T track:**
- ❌ Personal information
- ❌ Email addresses
- ❌ Exact IP addresses
- ❌ Cookies

Fully GDPR and CCPA compliant!

---

## 🐛 Troubleshooting

### GitHub Pages shows 404

1. Check branch is `gh-pages`
2. Wait 5-10 minutes after first deploy
3. Clear browser cache
4. Check GitHub Actions tab for errors

### Unity game doesn't load

1. Check browser console for errors
2. Verify Unity files are in `mobile-app/public/unity/`
3. Check iframe src path
4. Test Unity build standalone first

### Analytics not working

1. Open browser console
2. Look for `[Analytics]` logs
3. For Plausible: Check Network tab for `plausible.io/api/event` calls
4. Verify script is loaded (check page source)

### Mobile build fails

1. Check `eas.json` is valid JSON
2. Verify bundle identifiers are unique
3. Check Expo account has build credits
4. Read error message carefully

---

## ✅ Pre-Launch Checklist

Before sharing with friends:

- [ ] Cheat mode disabled in Unity
- [ ] All Unity builds rebuilt without cheat mode
- [ ] Analytics script added
- [ ] GitHub Pages deployed successfully
- [ ] Tested on multiple devices/browsers
- [ ] Mobile builds created (if needed)
- [ ] README updated with link
- [ ] Game instructions clear (How to Play modal)
- [ ] No console errors in production
- [ ] Loading times acceptable

---

## 🎉 You're Ready to Launch!

Share your game:
- **Web:** `https://yourusername.github.io/cardGameCollection`
- **Android APK:** EAS build download link
- **iOS:** TestFlight link (after production build)

Get feedback from friends and iterate! 🚀

---

## Need Help?

- **Expo Docs:** https://docs.expo.dev
- **GitHub Pages:** https://pages.github.com
- **Plausible:** https://plausible.io/docs
- **Unity WebGL:** https://docs.unity3d.com/Manual/webgl.html
