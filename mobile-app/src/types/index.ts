// Game types
export enum GameType {
  SOLITAIRE = 'solitaire',
  FREECELL = 'freecell',
  SPIDER = 'spider',
  SUDOKU = 'sudoku',
  CRAZY_EIGHTS = 'crazy_eights',
  GO_FISH = 'go_fish',
  GAME_2048 = '2048',
  MEMORY_MATCH = 'memory_match',
  SLIDING_PUZZLE = 'sliding_puzzle',
  POKER = 'poker',
  BLACKJACK = 'blackjack',
}

export interface Game {
  id: GameType;
  name: string;
  description: string;
  icon: string;
  minBet?: number;
  available: boolean;
}

// Player types
export interface Player {
  id: string;
  username: string;
  balance: number;
  level: number;
  experience: number;
  gamesPlayed: number;
  gamesWon: number;
  totalWinnings: number;
}

// Game result types
export interface GameResult {
  gameType: GameType;
  won: boolean;
  score: number;
  coinsEarned: number;
  timeSpent: number;
  moves?: number; // For solitaire
}

// Unity message types
export interface UnityMessage {
  type: string;
  data: any;
}

export interface StartGameData {
  gameType: GameType;
  difficulty?: 'easy' | 'medium' | 'hard';
  playerBalance: number;
  bet?: number;
}

export interface GameCompleteData {
  score: number;
  won: boolean;
  coinsEarned: number;
  moves?: number;
}

// Navigation types
export type RootStackParamList = {
  Home: undefined;
  Game: { gameType: GameType };
  Game2048: undefined;
  SlidingPuzzle: undefined;
  Profile: undefined;
  Settings: undefined;
  Leaderboard: undefined;
};
