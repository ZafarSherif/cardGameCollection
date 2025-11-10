import React, { useEffect } from 'react';
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { RootNavigator } from './src/navigation/RootNavigator';
import { LanguageProvider } from './src/i18n/LanguageContext';
import { analytics } from './src/services/analytics';
// Import Firebase config to ensure it's initialized
import './src/config/firebase';

export default function App() {
  useEffect(() => {
    // Initialize analytics and track app open
    analytics.initialize();
    analytics.trackAppOpen();
  }, []);

  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <LanguageProvider>
        <RootNavigator />
      </LanguageProvider>
    </GestureHandlerRootView>
  );
}
