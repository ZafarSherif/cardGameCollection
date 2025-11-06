# Internationalization (i18n) Setup

The app supports multiple languages using a JSON-based translation system.

## Supported Languages

- 🇬🇧 English (en) - Default
- 🇪🇸 Spanish (es)
- 🇫🇷 French (fr)
- 🇧🇷 Portuguese (pt)

## File Structure

```
src/i18n/
├── locales/
│   ├── en.json         # English translations
│   ├── es.json         # Spanish translations
│   ├── fr.json         # French translations
│   └── pt.json         # Portuguese translations
├── types.ts            # TypeScript types for translations
├── index.ts            # Translation loader and exports
└── LanguageContext.tsx # React Context for language management
```

## Usage

### In Components

```typescript
import { useLanguage } from '../i18n/LanguageContext';

function MyComponent() {
  const { t, language, setLanguage } = useLanguage();

  return (
    <View>
      <Text>{t.home.welcomeBack}</Text>
      <Text>{t.games.solitaire.name}</Text>
    </View>
  );
}
```

### Translation Keys

All translations are type-safe and accessed via dot notation:

```typescript
t.common.back           // "Back" / "Atrás" / "Retour" / "Voltar"
t.home.welcomeBack      // "Welcome back," / etc.
t.games.solitaire.name  // "Solitaire" / "Solitario" / etc.
```

### Variables in Translations

For dynamic content, use `{{variableName}}` in JSON:

```json
{
  "gameWillLoad": "The {{gameName}} game will be loaded here"
}
```

Replace in code:

```typescript
t.gameScreen.gameWillLoad.replace('{{gameName}}', gameType)
```

## Adding New Languages

1. **Create translation file:**
   ```bash
   src/i18n/locales/de.json  # German example
   ```

2. **Add to types:**
   ```typescript
   // src/i18n/types.ts
   export type Language = 'en' | 'es' | 'fr' | 'pt' | 'de';
   ```

3. **Import in index.ts:**
   ```typescript
   import de from './locales/de.json';

   export const translations: Record<Language, Translations> = {
     en, es, fr, pt, de
   };

   export const availableLanguages: Language[] = ['en', 'es', 'fr', 'pt', 'de'];
   ```

4. **Add language name to all locale files:**
   ```json
   {
     "languages": {
       "de": "Deutsch"
     }
   }
   ```

## Language Persistence

Language selection is automatically saved using `AsyncStorage` and persists across app restarts.

## Language Selector Component

The `<LanguageSelector />` component provides a modal UI for language selection:

- Shows current language with flag emoji
- Modal with all available languages
- Checkmark on selected language
- Saves selection automatically

## Best Practices

### 1. Keep Keys Organized

Group related translations:

```json
{
  "games": {
    "solitaire": { "name": "...", "description": "..." },
    "poker": { "name": "...", "description": "..." }
  }
}
```

### 2. Avoid Hardcoded Strings

❌ Bad:
```typescript
<Text>Welcome back</Text>
```

✅ Good:
```typescript
<Text>{t.home.welcomeBack}</Text>
```

### 3. Add Context for Translators

Use clear, descriptive keys:

```json
{
  "common": {
    "back": "Back",              // Navigation button
    "cancel": "Cancel",          // Action cancellation
    "confirm": "Confirm"         // Action confirmation
  }
}
```

### 4. Handle Plurals

For counts, create separate keys:

```json
{
  "game": "Game",
  "games": "Games",
  "gamesPlayed": "{{count}} games played"
}
```

### 5. RTL Languages (Future)

When adding Arabic, Hebrew, etc., consider:
- Text direction (RTL)
- Layout mirroring
- Icon positioning

## Testing Translations

1. **Manual Testing:**
   - Open app
   - Click language selector (globe icon)
   - Switch between languages
   - Verify all text updates

2. **Check Missing Keys:**
   - All locale files should have the same structure
   - Use TypeScript to catch missing translations

3. **Test Long Translations:**
   - German and French tend to be longer
   - Ensure UI doesn't break with longer text

## Common Issues

### "useLanguage must be used within a LanguageProvider"

**Solution:** Ensure `<LanguageProvider>` wraps your app in `App.tsx`:

```typescript
export default function App() {
  return (
    <LanguageProvider>
      <RootNavigator />
    </LanguageProvider>
  );
}
```

### Translation not updating

**Solution:**
- Check hot reload is working
- Restart Expo server
- Clear cache: `npm start -- --reset-cache`

### Type errors with translations

**Solution:**
- Update `src/i18n/types.ts` with new keys
- Ensure all locale files match the structure

## Unity Integration

When integrating Unity games, send language to Unity:

```typescript
// React Native → Unity
unityView.postMessage('SetLanguage', JSON.stringify({
  language: language
}));
```

Unity can then use the language code to load appropriate assets or text.

## Future Enhancements

- [ ] Add more languages (German, Chinese, Japanese, Arabic)
- [ ] Implement pluralization helper
- [ ] Add date/time formatting per locale
- [ ] Add number formatting per locale
- [ ] Create translation management tool
- [ ] Add automated translation checks in CI/CD
