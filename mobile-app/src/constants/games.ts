import { GameType } from '../types';

// Game metadata (non-translatable)
export const GAME_METADATA = [
  {
    id: GameType.SOLITAIRE,
    icon: '🃏',
    available: true,
  },
  {
    id: GameType.FREECELL,
    icon: '🎴',
    available: false,
  },
  {
    id: GameType.SPIDER,
    icon: '🕷️',
    available: false,
  },
  {
    id: GameType.SUDOKU,
    icon: '🔢',
    available: false,
  },
  {
    id: GameType.GAME_2048,
    icon: '📱',
    available: true,
  },
  {
    id: GameType.MEMORY_MATCH,
    icon: '🎯',
    available: false,
  },
  {
    id: GameType.CRAZY_EIGHTS,
    icon: '8️⃣',
    available: false,
  },
  {
    id: GameType.GO_FISH,
    icon: '🐟',
    available: false,
  },
  {
    id: GameType.POKER,
    icon: '🎰',
    available: false,
  },
  {
    id: GameType.BLACKJACK,
    icon: '🎲',
    available: false,
  },
];

export const STARTING_BALANCE = 1000;
export const DAILY_BONUS = 100;
