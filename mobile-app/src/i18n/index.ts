import { Language, Translations } from './types';
import en from './locales/en.json';
import es from './locales/es.json';
import fr from './locales/fr.json';
import pt from './locales/pt.json';

export const translations: Record<Language, Translations> = {
  en: en as Translations,
  es: es as Translations,
  fr: fr as Translations,
  pt: pt as Translations,
};

export const availableLanguages: Language[] = ['en', 'es', 'fr', 'pt'];

export const getTranslations = (language: Language): Translations => {
  return translations[language] || translations.en;
};

export * from './types';
