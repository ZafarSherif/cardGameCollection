import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { Platform } from 'react-native';
import { HomeScreen } from '../screens/HomeScreen';
import { GameScreen } from '../screens/GameScreen';
import { Game2048Screen } from '../screens/Game2048Screen';
import { SlidingPuzzleScreen } from '../screens/SlidingPuzzleScreen';
import { RootStackParamList } from '../types';

const Stack = createNativeStackNavigator<RootStackParamList>();

export const RootNavigator: React.FC = () => {
  // Configure linking for GitHub Pages subdirectory
  const linking = Platform.OS === 'web' ? {
    prefixes: ['https://ZafarSherif.github.io/cardGameCollection/', 'http://localhost:19006/'],
    config: {
      screens: {
        Home: '',
        Game: 'game',
      },
    },
    // Ensure the app respects the base path from index.html
    enabled: true,
    getPathFromState(state, config) {
      // Preserve the /cardGameCollection base path in the URL
      return state?.routes?.[state.index]?.name === 'Home' ? '' : state?.routes?.[state.index]?.name?.toLowerCase() || '';
    },
  } : undefined;

  return (
    <NavigationContainer
      linking={linking}
      documentTitle={{
        formatter: (options, route) => `Card Games - ${route?.name || 'Home'}`,
      }}
    >
      <Stack.Navigator
        initialRouteName="Home"
        screenOptions={{
          headerShown: false,
          animation: 'slide_from_right',
        }}
      >
        <Stack.Screen name="Home" component={HomeScreen} />
        <Stack.Screen
          name="Game"
          component={GameScreen}
          options={{
            animation: 'fade',
          }}
        />
        <Stack.Screen
          name="Game2048"
          component={Game2048Screen}
          options={{
            animation: 'fade',
          }}
        />
        <Stack.Screen
          name="SlidingPuzzle"
          component={SlidingPuzzleScreen}
          options={{
            animation: 'fade',
          }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
};
