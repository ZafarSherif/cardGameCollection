# Card Game Collection

A cross-platform card game collection featuring Solitaire, Poker, Blackjack and more.

## Architecture

**Hybrid Approach:**
- **Unity (C#)** - Game logic, animations, and gameplay
- **React Native (TypeScript)** - Lobby, menus, user management, social features

## Project Structure

```
CardGame/
├── unity-games/          # Unity projects for all card games
│   └── CardGames/        # Main Unity project
├── mobile-app/           # React Native + Expo app (iOS/Android/Web)
├── shared/               # Shared types, constants, and utilities
├── docs/                 # Project documentation
└── README.md
```

## Platforms

- ✅ Android
- ✅ iOS
- ✅ Web
- ✅ Windows (future)
- ✅ macOS (future)

## Tech Stack

### Frontend
- React Native + Expo
- TypeScript
- Zustand (state management)
- React Navigation

### Game Engine
- Unity 2022 LTS
- C#

### Backend (Future)
- Firebase Authentication
- Cloud Firestore
- Cloud Functions
- Firebase Analytics

### Monetization (Future)
- Unity Ads
- AdMob
- In-app purchases

## Getting Started

### Prerequisites
- Node.js 18+
- Unity 2022 LTS
- Expo CLI
- Xcode (for iOS)
- Android Studio (for Android)

### Setup

1. **Unity Project:**
   ```bash
   cd unity-games
   # Open CardGames project in Unity Hub
   ```

2. **React Native App:**
   ```bash
   cd mobile-app
   npm install
   npx expo start
   ```

## Development Workflow

1. Build game logic in Unity
2. Export Unity builds (iOS/Android/WebGL)
3. Integrate with React Native via bridge
4. Test on all platforms

## Games

- [x] Solitaire (in progress)
- [ ] Poker
- [ ] Blackjack
- [ ] More to come...
