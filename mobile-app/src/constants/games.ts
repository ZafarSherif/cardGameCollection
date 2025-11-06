import { GameType } from '../types';

// Game metadata (non-translatable)
export const GAME_METADATA = [
  {
    id: GameType.SOLITAIRE,
    icon: '🃏',
    available: true,
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
