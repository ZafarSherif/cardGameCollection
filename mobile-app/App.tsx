import React from 'react';
import { RootNavigator } from './src/navigation/RootNavigator';
import { LanguageProvider } from './src/i18n/LanguageContext';

export default function App() {
  return (
    <LanguageProvider>
      <RootNavigator />
    </LanguageProvider>
  );
}
