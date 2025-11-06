# Firebase Setup Guide 🔥

Complete guide for setting up Firebase as your all-in-one backend platform.

---

## Why Firebase?

Firebase provides **everything you need** in one platform:

- ✅ **Analytics** - Free, unlimited event tracking
- ✅ **Database** - Realtime & Firestore
- ✅ **Authentication** - Social login, email, anonymous
- ✅ **Multiplayer** - Real-time game state sync
- ✅ **Push Notifications** - Re-engage users
- ✅ **Cloud Functions** - Backend logic (leaderboards, matchmaking)
- ✅ **Crash Reporting** - Debug production issues
- ✅ **Remote Config** - A/B testing, feature flags
- ✅ **Generous Free Tier** - Perfect for starting out

**Plus easy integration with:**
- AdMob (ads)
- RevenueCat (in-app purchases)
- Unity (WebGL analytics)

---

## 📋 Phase 1: Basic Setup (Start Here)

### Step 1: Create Firebase Project

1. Go to [Firebase Console](https://console.firebase.google.com/)
2. Click "Add project"
3. Name: "Card Games Collection"
4. Enable Google Analytics: **YES**
5. Create project

### Step 2: Register Your Apps

**For Web App:**
1. In Firebase Console → Project Overview → Add app → Web
2. App nickname: "Web App"
3. Check "Also set up Firebase Hosting" (optional)
4. Register app
5. Copy the config object:

```javascript
const firebaseConfig = {
  apiKey: "AIza...",
  authDomain: "your-app.firebaseapp.com",
  projectId: "your-project-id",
  storageBucket: "your-app.appspot.com",
  messagingSenderId: "123456789",
  appId: "1:123:web:abc",
  measurementId: "G-XXXXXXXXXX"
};
```

**For iOS App:**
1. Add app → iOS
2. iOS bundle ID: `com.yourname.cardgames`
3. Download `GoogleService-Info.plist`

**For Android App:**
1. Add app → Android
2. Package name: `com.yourname.cardgames`
3. Download `google-services.json`

### Step 3: Install Firebase SDK

```bash
cd mobile-app

# Core Firebase
npm install firebase

# Expo Firebase packages (for mobile)
npx expo install expo-firebase-analytics
npx expo install expo-firebase-core
```

### Step 4: Create Firebase Config

Create `mobile-app/src/config/firebase.ts`:

```typescript
import { initializeApp } from 'firebase/app';
import { getAnalytics, isSupported } from 'firebase/analytics';
import { getAuth } from 'firebase/auth';
import { getFirestore } from 'firebase/firestore';
import { Platform } from 'react-native';

// Your Firebase config from Step 2
const firebaseConfig = {
  apiKey: "AIza...",
  authDomain: "your-app.firebaseapp.com",
  projectId: "your-project-id",
  storageBucket: "your-app.appspot.com",
  messagingSenderId: "123456789",
  appId: "1:123:web:abc",
  measurementId: "G-XXXXXXXXXX"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);

// Initialize services
export const auth = getAuth(app);
export const db = getFirestore(app);

// Analytics (web only for now)
let analytics: any = null;
if (Platform.OS === 'web') {
  isSupported().then((supported) => {
    if (supported) {
      analytics = getAnalytics(app);
    }
  });
}

export { analytics };
export default app;
```

### Step 5: Update Analytics Service

Update `mobile-app/src/services/analytics.ts`:

```typescript
import { Platform } from 'react-native';
import { analytics } from '../config/firebase';
import { logEvent as firebaseLogEvent } from 'firebase/analytics';

class AnalyticsService {
  private isEnabled: boolean = true;

  initialize() {
    console.log('[Analytics] Firebase Analytics initialized');
  }

  trackEvent(eventName: string, properties?: Record<string, string | number | boolean>) {
    if (!this.isEnabled) return;

    console.log('[Analytics] Event:', eventName, properties);

    if (Platform.OS === 'web' && analytics) {
      // Firebase Analytics for web
      firebaseLogEvent(analytics, eventName, properties);
    } else {
      // Mobile - will add Firebase native SDK later
      console.log('[Mobile Analytics]', eventName, properties);
    }
  }

  // ... rest of methods stay the same
}

export const analytics = new AnalyticsService();
```

### Step 6: Test Analytics

1. Run your app: `npm start`
2. Open browser to your app
3. Click around, play a game
4. Go to Firebase Console → Analytics → Events
5. Wait 24 hours (yeah, Firebase analytics has delay)
6. See your events!

**Debug mode for real-time testing:**

Add to `mobile-app/public/index.html`:

```html
<script>
  // Enable debug mode for development
  window['ga-disable-G-XXXXXXXXXX'] = false;
</script>
```

---

## 📱 Phase 2: Mobile Native Support

For native mobile apps, use Expo's Firebase packages:

### Install Expo Firebase

```bash
npx expo install expo-firebase-analytics expo-firebase-core
```

### Configure for iOS

1. Add `GoogleService-Info.plist` to your project
2. Update `app.json`:

```json
{
  "expo": {
    "ios": {
      "googleServicesFile": "./GoogleService-Info.plist"
    }
  }
}
```

### Configure for Android

1. Add `google-services.json` to your project
2. Update `app.json`:

```json
{
  "expo": {
    "android": {
      "googleServicesFile": "./google-services.json"
    }
  }
}
```

---

## 🔐 Phase 3: Authentication (Future)

When you're ready to add user accounts:

```typescript
import { auth } from '../config/firebase';
import { signInAnonymously, signInWithEmailAndPassword } from 'firebase/auth';

// Anonymous sign-in (great for games!)
async function signInAnon() {
  const userCredential = await signInAnonymously(auth);
  console.log('User:', userCredential.user.uid);
}

// Email sign-in
async function signIn(email: string, password: string) {
  await signInWithEmailAndPassword(auth, email, password);
}
```

---

## 💾 Phase 4: Database (Future)

When you need to save data:

```typescript
import { db } from '../config/firebase';
import { collection, doc, setDoc, getDoc } from 'firebase/firestore';

// Save user data
async function saveUserData(userId: string, data: any) {
  await setDoc(doc(db, 'users', userId), data);
}

// Get user data
async function getUserData(userId: string) {
  const docSnap = await getDoc(doc(db, 'users', userId));
  return docSnap.data();
}

// Leaderboards
async function saveScore(userId: string, gameType: string, score: number) {
  await setDoc(doc(db, 'scores', `${userId}_${gameType}`), {
    userId,
    gameType,
    score,
    timestamp: Date.now()
  });
}
```

---

## 🎮 Phase 5: Multiplayer (Future)

For real-time multiplayer:

```typescript
import { getDatabase, ref, onValue, set } from 'firebase/database';

const realtimeDb = getDatabase();

// Listen to game state
function joinGame(gameId: string) {
  const gameRef = ref(realtimeDb, `games/${gameId}`);
  onValue(gameRef, (snapshot) => {
    const gameState = snapshot.val();
    // Update Unity game state
  });
}

// Update game state
function updateGameState(gameId: string, state: any) {
  set(ref(realtimeDb, `games/${gameId}`), state);
}
```

---

## 💰 Phase 6: Monetization

### Option A: AdMob (Ads)

1. Sign up at [AdMob](https://admob.google.com/)
2. Create ad units
3. Install:
```bash
npx expo install expo-ads-admob
```

4. Link to Firebase Analytics (auto revenue tracking!)

### Option B: RevenueCat (In-App Purchases)

1. Sign up at [RevenueCat](https://www.revenuecat.com/)
2. Install SDK:
```bash
npm install react-native-purchases
npx expo prebuild
```

3. Configure products in RevenueCat dashboard
4. Implement:

```typescript
import Purchases from 'react-native-purchases';

// Initialize
await Purchases.configure({
  apiKey: 'your_revenuecat_api_key'
});

// Get offerings
const offerings = await Purchases.getOfferings();

// Make purchase
const purchaseResult = await Purchases.purchasePackage(package);
```

---

## 📊 What You Get with This Stack

### Analytics Dashboard (Firebase Console):

- **Daily Active Users (DAU)**
- **User Retention** (1-day, 7-day, 30-day)
- **User Lifetime Value**
- **Top Events** (game_start, game_complete, etc.)
- **User Demographics** (country, device, OS)
- **Conversion Funnels** (lobby → game → win)
- **Crash-Free Users %**
- **Revenue per User** (when you add IAP/ads)

### Database (Firestore):

```
users/
  {userId}/
    username: "player123"
    level: 5
    gamesPlayed: 42
    balance: 1500

scores/
  {scoreId}/
    userId: "abc123"
    gameType: "solitaire"
    score: 350
    timestamp: 1234567890

games/ (Realtime DB for multiplayer)
  {gameId}/
    players: ["user1", "user2"]
    state: {...}
    turn: "user1"
```

---

## 💵 Cost Breakdown

### Firebase Pricing (Free Tier):

| Service | Free Tier | Enough For |
|---------|-----------|------------|
| Analytics | Unlimited | ♾️ Forever |
| Firestore | 50K reads/day | ~500 DAU |
| Realtime DB | 100 concurrent | ~100 multiplayer games |
| Auth | Unlimited | ♾️ Forever |
| Functions | 2M invocations/mo | ~5K DAU |
| Storage | 5GB total | ~10K users |
| Hosting | 10GB transfer | ~50K visits/mo |

**You can serve 500-1000 active users entirely FREE.**

### When You Outgrow Free Tier:

Pay-as-you-go (Blaze plan):
- Firestore: $0.06 per 100K reads ($6 per 10M reads)
- Functions: $0.40 per 1M invocations
- Storage: $0.026 per GB
- **Typical cost for 10K DAU: $25-50/month**

### RevenueCat Pricing:

- Free up to $2.5K/month revenue
- Then 1% of revenue
- No minimum fees

### AdMob:

- **FREE** (they pay YOU)
- Typical: $1-5 per 1000 impressions
- With 10K DAU seeing 3 ads/day = ~$90-450/month revenue

---

## 🚀 Migration Steps (From Current Setup)

**Week 1: Analytics**
- ✅ Set up Firebase project
- ✅ Install Firebase SDK
- ✅ Update analytics.ts to use Firebase
- ✅ Test on web
- ✅ Deploy and monitor

**Week 2: Database (when needed)**
- Add Firestore
- Save user profiles
- Implement leaderboards

**Week 3: Authentication (optional)**
- Add anonymous auth (recommended for games)
- Or email/social login

**Week 4: Mobile Native**
- Configure iOS/Android apps
- Test Firebase Analytics on mobile
- Build APK/IPA

**Future: Monetization**
- Set up AdMob or RevenueCat
- A/B test pricing with Remote Config

---

## 🔗 Useful Links

- [Firebase Console](https://console.firebase.google.com/)
- [Firebase Docs](https://firebase.google.com/docs)
- [Expo Firebase Guide](https://docs.expo.dev/guides/using-firebase/)
- [RevenueCat Docs](https://docs.revenuecat.com/)
- [AdMob Console](https://admob.google.com/)

---

## ✅ Next Steps

**Right Now:**
1. Create Firebase project
2. Register web app
3. Copy config to `firebase.ts`
4. Install Firebase SDK
5. Update analytics.ts
6. Deploy and test

**This Week:**
1. Monitor analytics in Firebase Console
2. Share game with friends
3. Get real usage data

**Next Month:**
1. Add user accounts (anonymous auth)
2. Add leaderboards (Firestore)
3. Consider monetization options

---

**You're now set up for:**
- ✅ Analytics
- ✅ Future database
- ✅ Future authentication
- ✅ Future multiplayer
- ✅ Future monetization
- ✅ Scales to millions of users

All on ONE platform! 🎉
