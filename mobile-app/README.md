# Card Game Collection - Mobile App

React Native + Expo mobile application for the card game collection.

## Tech Stack

- **Framework:** React Native + Expo
- **Language:** TypeScript
- **Navigation:** React Navigation (Native Stack)
- **State Management:** Zustand
- **UI Components:** React Native Paper

## Project Structure

```
mobile-app/
├── src/
│   ├── components/       # Reusable UI components
│   │   └── GameCard.tsx
│   ├── screens/          # Screen components
│   │   ├── HomeScreen.tsx
│   │   └── GameScreen.tsx
│   ├── navigation/       # Navigation setup
│   │   └── RootNavigator.tsx
│   ├── store/            # Zustand stores
│   │   └── playerStore.ts
│   ├── types/            # TypeScript types
│   │   └── index.ts
│   ├── constants/        # Constants and config
│   │   ├── games.ts
│   │   └── theme.ts
│   └── assets/           # Images, fonts, etc.
├── App.tsx               # Root component
└── package.json
```

## Getting Started

### Prerequisites

- Node.js 18+
- npm or yarn
- Expo CLI
- iOS Simulator (Mac) or Android Emulator

### Installation

```bash
# Install dependencies
npm install --cache /tmp/npm-cache-cardgame

# Start Expo dev server
npm start
```

### Run on Platforms

```bash
# iOS
npm run ios

# Android
npm run android

# Web
npm run web
```

## Features

### Current

- ✅ Home/Lobby screen with game selection
- ✅ Player balance and stats display
- ✅ Game navigation
- ✅ Placeholder game screen
- ✅ State management with Zustand

### Coming Soon

- [ ] Unity view integration
- [ ] Solitaire gameplay
- [ ] Firebase backend integration
- [ ] User authentication
- [ ] Leaderboards
- [ ] Ads integration

## Screens

### HomeScreen

The main lobby where players can:
- View their balance and stats
- Select a game to play
- See available and upcoming games

### GameScreen

Container for Unity game views. Will embed Unity WebGL/native views.

## State Management

Using Zustand for simple, performant state management:

```typescript
const player = usePlayerStore((state) => state.player);
const updateBalance = usePlayerStore((state) => state.updateBalance);
```

## Unity Integration (Next Steps)

The app is set up to integrate Unity games via:
- **Mobile:** `react-native-unity-view`
- **Web:** Unity WebGL embedded

Game communication will happen through message passing between React Native and Unity.

## Development Notes

- Uses TypeScript strict mode
- Dark theme by default
- All navigation uses React Navigation
- State persists across screens with Zustand
