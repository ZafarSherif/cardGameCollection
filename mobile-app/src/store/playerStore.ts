import { create } from 'zustand';
import { Player, GameResult } from '../types';
import { STARTING_BALANCE } from '../constants/games';

interface PlayerState {
  player: Player;
  updateBalance: (amount: number) => void;
  addGameResult: (result: GameResult) => void;
  resetPlayer: () => void;
}

const DEFAULT_PLAYER: Player = {
  id: 'player-1', // TODO: Replace with real user ID after auth
  username: 'Player',
  balance: STARTING_BALANCE,
  level: 1,
  experience: 0,
  gamesPlayed: 0,
  gamesWon: 0,
  totalWinnings: 0,
};

export const usePlayerStore = create<PlayerState>((set) => ({
  player: DEFAULT_PLAYER,

  updateBalance: (amount: number) =>
    set((state) => ({
      player: {
        ...state.player,
        balance: state.player.balance + amount,
      },
    })),

  addGameResult: (result: GameResult) =>
    set((state) => {
      const newBalance = state.player.balance + result.coinsEarned;
      const newGamesPlayed = state.player.gamesPlayed + 1;
      const newGamesWon = result.won ? state.player.gamesWon + 1 : state.player.gamesWon;
      const newTotalWinnings = result.won
        ? state.player.totalWinnings + result.coinsEarned
        : state.player.totalWinnings;
      const newExperience = state.player.experience + (result.won ? 100 : 50);

      return {
        player: {
          ...state.player,
          balance: newBalance,
          gamesPlayed: newGamesPlayed,
          gamesWon: newGamesWon,
          totalWinnings: newTotalWinnings,
          experience: newExperience,
          level: Math.floor(newExperience / 500) + 1,
        },
      };
    }),

  resetPlayer: () => set({ player: DEFAULT_PLAYER }),
}));
