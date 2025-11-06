# Unity Project Setup Guide

This guide covers setting up the Unity project for the card game collection.

## Unity Version

**Recommended:** Unity 2022 LTS (Long Term Support)
- Download from: https://unity.com/download

## Creating the Project

### Step 1: Create New Unity Project

1. Open Unity Hub
2. Click "New Project"
3. Select "2D" template
4. Project name: `CardGames`
5. Location: `CardGame/unity-games/`
6. Click "Create"

### Step 2: Project Settings

#### Build Settings

Configure for multi-platform:

**For iOS:**
1. File → Build Settings
2. Select iOS
3. Click "Switch Platform"

**For Android:**
1. File → Build Settings
2. Select Android
3. Click "Switch Platform"

**For WebGL:**
1. File → Build Settings
2. Select WebGL
3. Click "Switch Platform"

#### Player Settings

1. Edit → Project Settings → Player
2. Set:
   - Company Name: Your name
   - Product Name: Card Games
   - Default Icon: (add your icon)

**iOS Settings:**
- Target minimum iOS version: 12.0
- Architecture: ARM64

**Android Settings:**
- Minimum API Level: 24 (Android 7.0)
- Target API Level: 33 (Android 13)
- Scripting Backend: IL2CPP
- Target Architectures: ARM64

**WebGL Settings:**
- Compression Format: Gzip
- Enable Exceptions: Explicitly Thrown Exceptions Only

## Project Structure

```
CardGames/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/              # Core game systems
│   │   ├── Solitaire/         # Solitaire-specific code
│   │   ├── Poker/             # Poker-specific code (future)
│   │   ├── Blackjack/         # Blackjack-specific code (future)
│   │   └── Bridge/            # React Native bridge
│   ├── Prefabs/
│   │   ├── Cards/             # Card prefabs
│   │   └── UI/                # UI elements
│   ├── Scenes/
│   │   ├── Solitaire.unity
│   │   ├── Poker.unity
│   │   └── Blackjack.unity
│   ├── Materials/
│   ├── Sprites/
│   │   └── Cards/             # Card sprites
│   └── Resources/
├── Packages/
└── ProjectSettings/
```

## Installing Required Packages

### 1. Unity Package Manager

Open Window → Package Manager and install:

1. **2D Sprite** (if not included)
2. **TextMeshPro** - For better text rendering
3. **Unity UI** - For UI elements

### 2. External Packages (Optional)

For JSON serialization:
- Newtonsoft Json: https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@latest

## Card Sprites

You'll need card sprites. Options:

1. **Free Assets:**
   - Search Unity Asset Store for "playing cards"
   - https://opengameart.org/

2. **Create Custom:**
   - Use design tools (Figma, Illustrator)
   - Export as PNG (recommended size: 256x384 per card)

3. **Placeholder:**
   - Start with colored rectangles
   - Add text for rank/suit

## Building for Different Platforms

### iOS Build

```
1. File → Build Settings
2. Select iOS
3. Click "Build"
4. Export to: unity-games/builds/ios/
```

The exported Xcode project will be integrated into React Native.

### Android Build

```
1. File → Build Settings
2. Select Android
3. Build App Bundle (AAB) or APK
4. Export to: unity-games/builds/android/
```

### WebGL Build

```
1. File → Build Settings
2. Select WebGL
3. Click "Build"
4. Export to: unity-games/builds/webgl/
```

The WebGL build will be embedded in the React web app.

## React Native Bridge Setup

### Message System

Create a bridge to communicate with React Native:

**Unity → RN:**
```csharp
public static void SendMessageToRN(string messageType, string data) {
    #if UNITY_WEBGL && !UNITY_EDITOR
        SendMessageToJS(messageType, data);
    #elif UNITY_IOS || UNITY_ANDROID
        SendMessageToNative(messageType, data);
    #endif
}
```

**RN → Unity:**
```csharp
public void ReceiveMessageFromRN(string message) {
    var data = JsonUtility.FromJson<MessageData>(message);
    // Handle message
}
```

## Testing

### In Unity Editor

- Play mode testing
- Use Debug.Log extensively
- Test all game states

### Build Testing

- Test on actual devices
- Check performance
- Verify message passing works

## Performance Tips

1. **Object Pooling:** Reuse card objects instead of instantiating/destroying
2. **Optimize Sprites:** Use sprite atlases
3. **Reduce Draw Calls:** Batch similar objects
4. **Profile:** Use Unity Profiler to identify bottlenecks

## Next Steps

1. Create basic card system
2. Build Solitaire game logic
3. Add animations
4. Set up bridge for RN communication
5. Build and test exports

## Resources

- Unity Manual: https://docs.unity3d.com/Manual/
- Unity Scripting API: https://docs.unity3d.com/ScriptReference/
- Unity Learn: https://learn.unity.com/
