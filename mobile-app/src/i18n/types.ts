export type Language = 'en' | 'es' | 'fr' | 'pt';

export interface Translations {
  common: {
    back: string;
    close: string;
    cancel: string;
    confirm: string;
    ok: string;
    save: string;
    loading: string;
    error: string;
    success: string;
  };
  home: {
    welcomeBack: string;
    balance: string;
    level: string;
    games: string;
    winRate: string;
    chooseGame: string;
    moreGamesComing: string;
  };
  games: {
    solitaire: {
      name: string;
      description: string;
    };
    poker: {
      name: string;
      description: string;
    };
    blackjack: {
      name: string;
      description: string;
    };
    comingSoon: string;
  };
  gameScreen: {
    unityView: string;
    gameWillLoad: string;
    integrateNext: string;
    controlsPlaceholder: string;
  };
  settings: {
    title: string;
    language: string;
    sound: string;
    music: string;
    notifications: string;
    about: string;
  };
  languages: {
    en: string;
    es: string;
    fr: string;
    de: string;
    pt: string;
    zh: string;
    ja: string;
    ar: string;
  };
  howToPlay: {
    solitaire: {
      title: string;
      objective: {
        title: string;
        text: string;
      };
      howTo: {
        title: string;
        steps: string[];
      };
      scoring: {
        title: string;
        points: string[];
      };
      tips: {
        title: string;
        list: string[];
      };
    };
  };
}

export interface LanguageContextType {
  language: Language;
  setLanguage: (lang: Language) => void;
  t: Translations;
}
