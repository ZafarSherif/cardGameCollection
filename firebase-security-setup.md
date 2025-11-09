# Firebase Security Setup Guide

## Step 1: Set Up Firestore Security Rules (If Using Database)

Go to: https://console.firebase.google.com → Your Project → Firestore Database → Rules

Replace default rules with:

```javascript
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    // Game results - users can only write their own
    match /gameResults/{userId}/{document=**} {
      allow read: if request.auth != null && request.auth.uid == userId;
      allow write: if request.auth != null && request.auth.uid == userId;
    }

    // Leaderboard - read-only for everyone, write from server only
    match /leaderboard/{document=**} {
      allow read: if true;
      allow write: if false; // Only server can write
    }

    // Block all other access by default
    match /{document=**} {
      allow read, write: if false;
    }
  }
}
```

## Step 2: Set Up Analytics Security

Analytics is already secure by default:
- ✅ Users can only send events, not read them
- ✅ Only you can view analytics in Firebase Console
- ✅ No additional rules needed

## Step 3: Restrict API Keys to Your Domain (Optional but Recommended)

### For Web (GitHub Pages):

1. Go to: https://console.firebase.google.com
2. Select your project: card-games-collection-12f3d
3. Click **Settings** (gear icon) → **Project Settings**
4. Scroll to **Your apps** section
5. Click on your web app
6. Under **App check** or **API restrictions**, click **Manage**

### Add Authorized Domains:

```
https://zafarsherif.github.io
https://localhost (for local testing)
```

### Restrict API Key:

1. Go to: https://console.cloud.google.com/apis/credentials
2. Find your Firebase API key
3. Click **Edit**
4. Under **Application restrictions**:
   - Select **HTTP referrers (websites)**
   - Add:
     ```
     https://zafarsherif.github.io/cardGameCollection/*
     http://localhost:*
     ```
5. Under **API restrictions**:
   - Select **Restrict key**
   - Enable only:
     - Firebase Installations API
     - Identity Toolkit API
     - Token Service API

## Step 4: Enable Firebase App Check (Advanced Security)

App Check prevents unauthorized access to your Firebase services:

1. Go to: https://console.firebase.google.com → App Check
2. Click **Get started**
3. For **Web apps**, register with reCAPTCHA v3
4. Enable enforcement for:
   - Analytics: **Enabled** (after testing)
   - Firestore: **Enabled** (if using database)

## Step 5: Monitor Usage

Set up alerts for unusual activity:

1. Go to: Firebase Console → Usage and billing
2. Set up budget alerts
3. Monitor daily API calls
4. Set limits:
   - Analytics: 500 events per user per day
   - Database reads: Alert if >100k/day

## Common Questions

### Q: Are Firebase API keys really meant to be public?
**A:** Yes! They're in every mobile app. Security comes from:
1. Firebase Security Rules (database/storage)
2. Domain restrictions (web apps)
3. App Check (prevents bots)

### Q: What if someone steals my API key?
**A:** They can't do anything harmful if you have:
- ✅ Proper Security Rules
- ✅ Domain restrictions
- ✅ App Check enabled

They can only:
- Send analytics events (which is fine, that's the point)
- Try to access database (blocked by Security Rules)

### Q: Should I rotate my API keys?
**A:** Only if:
- You committed keys to public Git history
- You suspect abuse (check Firebase usage)
- You didn't set up Security Rules yet

To rotate:
1. Create new Firebase web app in console
2. Get new API keys
3. Update .env file
4. Redeploy

## Security Checklist

Before going live:
- [ ] `.env` in `.gitignore`
- [ ] Environment variables set up
- [ ] Firebase Security Rules configured
- [ ] API key restricted to your domain
- [ ] App Check enabled (optional)
- [ ] Budget alerts set up
- [ ] No hardcoded keys in code
- [ ] `.env.example` in repo for reference

## For Production Deployment

### GitHub Pages:
- ✅ Environment variables are baked into build
- ✅ Keys are in deployed JS bundle (this is fine)
- ✅ Domain restrictions protect against abuse

### Native Apps (Future):
- Store keys in app config
- Use Firebase App Check
- API key is in the app binary (this is expected)

## Emergency: If Keys Are Compromised

If you committed keys to Git and pushed:

1. **Immediately restrict API key to your domain** (step 3 above)
2. **Enable App Check enforcement**
3. **Check Firebase usage for anomalies**
4. **Consider rotating keys** if you see abuse
5. **Clean Git history** (advanced):
   ```bash
   # Remove file from all Git history
   git filter-branch --force --index-filter \
     "git rm --cached --ignore-unmatch src/config/firebase.ts" \
     --prune-empty --tag-name-filter cat -- --all

   # Force push
   git push origin --force --all
   ```

## Resources

- Firebase Security Rules: https://firebase.google.com/docs/rules
- API Key Restrictions: https://cloud.google.com/docs/authentication/api-keys
- App Check: https://firebase.google.com/docs/app-check
