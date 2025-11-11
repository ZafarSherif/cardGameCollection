# Native Build Setup Guide

This guide will help you set up native iOS and Android builds for the Card Game Collection app.

## ✅ Completed Steps

1. **Ejected from Expo** - The `ios/` and `android/` folders have been created
2. **Updated Configuration** - `app.json` and `package.json` are configured for native builds
3. **Added Build Scripts** - npm scripts are ready for building

## 🚀 Next Steps

### Prerequisites

Before building, you need to install:

#### For iOS Development:
- **Xcode** (from Mac App Store) - Version 15.0+ recommended
- **Xcode Command Line Tools**: `xcode-select --install`
- **CocoaPods** (dependency manager for iOS)

#### For Android Development:
- **Android Studio** (https://developer.android.com/studio)
- **Java JDK 17** (usually installed with Android Studio)
- **Android SDK** (installed via Android Studio)

---

## iOS Setup

### Step 1: Install CocoaPods

CocoaPods is required for managing iOS dependencies. Install it with:

```bash
sudo gem install cocoapods
```

If this fails, try with Homebrew:
```bash
brew install cocoapods
```

### Step 2: Install iOS Dependencies

Navigate to the ios folder and install pods:

```bash
cd ios
pod install
```

This will:
- Download all iOS native dependencies
- Create `CardGameCollection.xcworkspace`

**IMPORTANT**: Always open the `.xcworkspace` file, NOT the `.xcodeproj` file!

### Step 3: Open in Xcode

```bash
open CardGameCollection.xcworkspace
```

Or double-click `CardGameCollection.xcworkspace` in Finder.

### Step 4: Configure Code Signing

1. In Xcode, select the project in the left sidebar
2. Select the **CardGameCollection** target
3. Go to **Signing & Capabilities** tab
4. Under **Team**, select your Apple Developer account (or "Add Account" if none)
   - For local testing, you can use a free Personal Team
   - For App Store distribution, you need a paid Developer account ($99/year)
5. Xcode will automatically create provisioning profiles

### Step 5: Build & Run on Simulator

1. In Xcode, select a simulator from the device dropdown (e.g., "iPhone 16 Pro")
2. Click the Play ▶️ button or press `Cmd+R`
3. The app will build and launch in the simulator

### Step 6: Build & Run on Physical Device

1. Connect your iPhone/iPad via USB
2. **Trust your Mac** on the device if prompted
3. In Xcode, select your device from the device dropdown
4. Click Play ▶️ button
5. **First time only**: On your device, go to:
   - Settings → General → VPN & Device Management
   - Tap your developer account
   - Tap "Trust"
6. Return to your device and launch the app

### Alternative: Build via Command Line

From the project root:
```bash
npm run ios
```

This will build and run in the simulator.

---

## Android Setup

### Step 1: Install Android Studio

1. Download from https://developer.android.com/studio
2. Run the installer
3. In the setup wizard, install:
   - Android SDK
   - Android SDK Platform
   - Android Virtual Device (AVD)

### Step 2: Set Environment Variables

Add to your `~/.zshrc` or `~/.bash_profile`:

```bash
export ANDROID_HOME=$HOME/Library/Android/sdk
export PATH=$PATH:$ANDROID_HOME/emulator
export PATH=$PATH:$ANDROID_HOME/platform-tools
export PATH=$PATH:$ANDROID_HOME/tools
export PATH=$PATH:$ANDROID_HOME/tools/bin
```

Then reload:
```bash
source ~/.zshrc  # or source ~/.bash_profile
```

### Step 3: Accept Android Licenses

```bash
cd android
./gradlew --version  # Test Gradle works
cd ..
```

Accept all SDK licenses:
```bash
yes | $ANDROID_HOME/cmdline-tools/latest/bin/sdkmanager --licenses
```

### Step 4: Open in Android Studio

1. Open Android Studio
2. Click "Open"
3. Select the `android/` folder
4. Wait for Gradle sync to complete

### Step 5: Create an Emulator (if you don't have one)

1. In Android Studio, click **Device Manager** (phone icon)
2. Click **Create Device**
3. Select a device (e.g., "Pixel 8")
4. Select a system image (e.g., "Tiramisu" API 33)
5. Click Finish

### Step 6: Build & Run

#### Option A: Via Android Studio
1. Click the green Play ▶️ button in Android Studio
2. Select your emulator or connected device
3. Wait for build to complete and app to launch

#### Option B: Via Command Line
From the project root:
```bash
npm run android
```

This will build and install on a running emulator or connected device.

---

## Testing Both Platforms

### Test iOS:
```bash
npm run ios
```

### Test Android:
```bash
npm run android
```

### Test Web (existing):
```bash
npm run web
```

---

## Creating Production Builds

### iOS Production Build (Archive for App Store)

1. In Xcode, select "Any iOS Device (arm64)" from device dropdown
2. Menu: Product → Archive
3. Wait for build to complete
4. Xcode Organizer will open showing your archive
5. Click "Distribute App" → "App Store Connect" → follow prompts

Or via command line:
```bash
npm run build:ios
```

### Android Production Build (APK/AAB)

Generate a signing key (first time only):
```bash
cd android/app
keytool -genkeypair -v -storetype PKCS12 -keystore cardgame-release.keystore -alias cardgame -keyalg RSA -keysize 2048 -validity 10000
```

Create `android/gradle.properties` and add:
```properties
CARDGAME_UPLOAD_STORE_FILE=cardgame-release.keystore
CARDGAME_UPLOAD_KEY_ALIAS=cardgame
CARDGAME_UPLOAD_STORE_PASSWORD=your_password_here
CARDGAME_UPLOAD_KEY_PASSWORD=your_password_here
```

Then build:
```bash
npm run build:android
```

The APK will be at:
`android/app/build/outputs/apk/release/app-release.apk`

---

## Common Issues & Solutions

### iOS Issues

**Issue**: `pod install` fails
- **Solution**: Update CocoaPods: `sudo gem install cocoapods --pre`

**Issue**: Code signing error
- **Solution**: Ensure you're signed in to Xcode with an Apple ID (Xcode → Settings → Accounts)

**Issue**: Simulator not showing up
- **Solution**: Open Xcode → Window → Devices and Simulators → Add simulator

### Android Issues

**Issue**: `ANDROID_HOME` not set
- **Solution**: Add environment variables (see Step 2 above)

**Issue**: Gradle sync fails
- **Solution**: In Android Studio, File → Invalidate Caches → Restart

**Issue**: "No connected devices"
- **Solution**: Start an emulator in Android Studio first, then run `npm run android`

**Issue**: Build fails with "SDK not found"
- **Solution**: Open Android Studio → Preferences → Appearance & Behavior → System Settings → Android SDK
  - Install Android 14.0 (API 35) and Android SDK Build-Tools 35

---

## Project Structure

After prebuild, your project structure:

```
mobile-app/
├── android/              ← Android native project (generated)
│   ├── app/
│   ├── gradle/
│   └── build.gradle
├── ios/                  ← iOS native project (generated)
│   ├── CardGameCollection/
│   ├── CardGameCollection.xcodeproj
│   └── CardGameCollection.xcworkspace  ← Open this!
├── src/                  ← React Native code (shared)
│   ├── components/
│   ├── screens/
│   └── ...
├── app.json              ← Expo configuration
└── package.json          ← Dependencies & scripts
```

**Key Points**:
- `ios/` and `android/` folders are **generated** from `app.json`
- If you run `expo prebuild --clean`, these folders are recreated
- You can make manual native changes, but they may be overwritten
- For persistence, use Expo config plugins

---

## Bundle Identifiers

- **iOS**: `com.zafarsherif.cardgamecollection`
- **Android**: `com.zafarsherif.cardgamecollection`

These are configured in `app.json` and propagated to native projects.

---

## App Store Submission (When Ready)

### iOS App Store

1. Create an App Store Connect account (requires paid Apple Developer account)
2. Create app listing at https://appstoreconnect.apple.com
3. Archive in Xcode (see "iOS Production Build" above)
4. Upload to App Store Connect
5. Fill in metadata (screenshots, description, etc.)
6. Submit for review

### Google Play Store

1. Create Google Play Console account ($25 one-time fee)
2. Create app listing at https://play.google.com/console
3. Build AAB: `cd android && ./gradlew bundleRelease`
4. Upload AAB to Google Play Console
5. Fill in store listing
6. Submit for review

---

## Next Steps

1. ✅ **Install CocoaPods**: `sudo gem install cocoapods`
2. ✅ **Install iOS dependencies**: `cd ios && pod install`
3. ✅ **Install Android Studio**: Download and set up
4. ✅ **Test iOS build**: `npm run ios`
5. ✅ **Test Android build**: `npm run android`
6. ✅ **Test on physical devices**

---

## Resources

- [Expo Bare Workflow Docs](https://docs.expo.dev/bare/overview/)
- [React Native iOS Setup](https://reactnative.dev/docs/environment-setup?platform=ios)
- [React Native Android Setup](https://reactnative.dev/docs/environment-setup?platform=android)
- [CocoaPods Guide](https://guides.cocoapods.org/using/getting-started.html)
- [Android Gradle Docs](https://developer.android.com/studio/build)

---

**Happy Building! 🎉**
