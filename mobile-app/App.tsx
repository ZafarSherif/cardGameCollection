import React, { useEffect } from 'react';
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
    <LanguageProvider>
      <RootNavigator />
    </LanguageProvider>
  );
}
