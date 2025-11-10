import React, { useState, useEffect } from 'react';
import {
  StyleSheet,
  View,
  Text,
  TouchableOpacity,
  SafeAreaView,
  Dimensions,
  Animated,
} from 'react-native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RouteProp } from '@react-navigation/native';
import { GestureDetector, Gesture } from 'react-native-gesture-handler';
import { RootStackParamList } from '../types';
import { COLORS, SPACING, FONT_SIZES } from '../constants/theme';
import { useLanguage } from '../i18n/LanguageContext';
import { usePlayerStore } from '../store/playerStore';
import { CommonGameHeader } from '../components/game/CommonGameHeader';
import { HowToPlayModal } from '../components/HowToPlayModal';
import {
  initializeGame,
  move,
  getTileColor,
  getTextColor,
  GameState,
  Tile,
} from '../games/2048/gameLogic';

type Game2048ScreenNavigationProp = NativeStackNavigationProp<
  RootStackParamList,
  'Game'
>;

type Game2048ScreenRouteProp = RouteProp<RootStackParamList, 'Game'>;

interface Game2048ScreenProps {
  navigation: Game2048ScreenNavigationProp;
  route: Game2048ScreenRouteProp;
}

export const Game2048Screen: React.FC<Game2048ScreenProps> = ({ navigation }) => {
  const { t } = useLanguage();
  const { addGameResult, updateBalance } = usePlayerStore();
  const [gameState, setGameState] = useState<GameState>(initializeGame());
  const [animating, setAnimating] = useState(false);
  const [showHowToPlay, setShowHowToPlay] = useState(false);

  const handleBack = () => {
    navigation.goBack();
  };

  const handleNewGame = () => {
    setGameState(initializeGame());
  };

  const handleMove = (direction: 'up' | 'down' | 'left' | 'right') => {
    if (animating || gameState.gameOver) return;

    setAnimating(true);
    const newState = move(gameState, direction);
    setGameState(newState);

    setTimeout(() => setAnimating(false), 200);
  };

  // Arrow key support for desktop
  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (animating || gameState.gameOver) return;

      switch (event.key) {
        case 'ArrowUp':
          event.preventDefault();
          handleMove('up');
          break;
        case 'ArrowDown':
          event.preventDefault();
          handleMove('down');
          break;
        case 'ArrowLeft':
          event.preventDefault();
          handleMove('left');
          break;
        case 'ArrowRight':
          event.preventDefault();
          handleMove('right');
          break;
      }
    };

    // Only add listener on web
    if (typeof window !== 'undefined') {
      window.addEventListener('keydown', handleKeyDown);
      return () => window.removeEventListener('keydown', handleKeyDown);
    }
  }, [animating, gameState.gameOver]);

  // Gesture handling
  const gesture = Gesture.Pan()
    .onEnd((event) => {
      const { translationX, translationY } = event;
      const threshold = 50;

      if (Math.abs(translationX) > Math.abs(translationY)) {
        // Horizontal swipe
        if (translationX > threshold) {
          handleMove('right');
        } else if (translationX < -threshold) {
          handleMove('left');
        }
      } else {
        // Vertical swipe
        if (translationY > threshold) {
          handleMove('down');
        } else if (translationY < -threshold) {
          handleMove('up');
        }
      }
    });

  // Track game end
  useEffect(() => {
    if (gameState.gameOver || gameState.won) {
      const coinsEarned = Math.floor(gameState.score / 10);
      updateBalance(coinsEarned);

      addGameResult({
        gameType: '2048',
        won: gameState.won,
        score: gameState.score,
        coinsEarned,
        timeSpent: 0, // Track this later if needed
      });
    }
  }, [gameState.gameOver, gameState.won]);

  const { width } = Dimensions.get('window');
  const boardSize = Math.min(width - 32, 400);
  const tileSize = (boardSize - 16 * 5) / 4;

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <CommonGameHeader
        title="2048"
        onBack={handleBack}
        onHowToPlay={() => setShowHowToPlay(true)}
      />

      {/* Score */}
      <View style={styles.scoreContainer}>
        <View style={styles.scoreBox}>
          <Text style={styles.scoreLabel}>SCORE</Text>
          <Text style={styles.scoreValue}>{gameState.score}</Text>
        </View>
        <View style={styles.scoreBox}>
          <Text style={styles.scoreLabel}>BEST</Text>
          <Text style={styles.scoreValue}>{gameState.bestScore}</Text>
        </View>
        <TouchableOpacity style={styles.newGameButton} onPress={handleNewGame}>
          <Text style={styles.newGameText}>New Game</Text>
        </TouchableOpacity>
      </View>

      {/* Instructions */}
      <Text style={styles.instructions}>
        Join the numbers and get to the 2048 tile!
      </Text>

      {/* Game Board */}
      <GestureDetector gesture={gesture}>
        <View style={[styles.board, { width: boardSize, height: boardSize }]}>
          {/* Grid Background */}
          {[0, 1, 2, 3].map((row) =>
            [0, 1, 2, 3].map((col) => (
              <View
                key={`bg-${row}-${col}`}
                style={[
                  styles.cell,
                  {
                    width: tileSize,
                    height: tileSize,
                    top: row * (tileSize + 16) + 16,
                    left: col * (tileSize + 16) + 16,
                  },
                ]}
              />
            ))
          )}

          {/* Tiles */}
          {gameState.tiles.map((tile) => (
            <View
              key={tile.id}
              style={[
                styles.tile,
                {
                  width: tileSize,
                  height: tileSize,
                  top: tile.position.row * (tileSize + 16) + 16,
                  left: tile.position.col * (tileSize + 16) + 16,
                  backgroundColor: getTileColor(tile.value),
                },
              ]}
            >
              <Text
                style={[
                  styles.tileText,
                  {
                    color: getTextColor(tile.value),
                    fontSize: tile.value > 512 ? 30 : 40,
                  },
                ]}
              >
                {tile.value}
              </Text>
            </View>
          ))}
        </View>
      </GestureDetector>

      {/* Game Over / Win Overlay */}
      {(gameState.gameOver || gameState.won) && (
        <View style={styles.overlay}>
          <View style={styles.overlayContent}>
            <Text style={styles.overlayTitle}>
              {gameState.won ? '🎉 You Win!' : 'Game Over!'}
            </Text>
            <Text style={styles.overlayScore}>Score: {gameState.score}</Text>
            <TouchableOpacity style={styles.tryAgainButton} onPress={handleNewGame}>
              <Text style={styles.tryAgainText}>Try Again</Text>
            </TouchableOpacity>
          </View>
        </View>
      )}

      {/* How to Play Modal */}
      <HowToPlayModal
        visible={showHowToPlay}
        onClose={() => setShowHowToPlay(false)}
        gameType="2048"
      />
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#faf8ef',
  },
  scoreContainer: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    paddingHorizontal: SPACING.lg,
    marginBottom: SPACING.md,
  },
  scoreBox: {
    backgroundColor: '#bbada0',
    paddingHorizontal: 20,
    paddingVertical: 10,
    borderRadius: 3,
    marginHorizontal: 5,
    minWidth: 80,
    alignItems: 'center',
  },
  scoreLabel: {
    fontSize: 12,
    color: '#eee4da',
    fontWeight: 'bold',
  },
  scoreValue: {
    fontSize: 24,
    color: '#fff',
    fontWeight: 'bold',
  },
  newGameButton: {
    backgroundColor: '#8f7a66',
    paddingHorizontal: 20,
    paddingVertical: 10,
    borderRadius: 3,
    marginLeft: 10,
  },
  newGameText: {
    color: '#f9f6f2',
    fontWeight: 'bold',
    fontSize: 14,
  },
  instructions: {
    textAlign: 'center',
    fontSize: 16,
    color: '#776e65',
    marginBottom: SPACING.lg,
    paddingHorizontal: SPACING.lg,
  },
  board: {
    alignSelf: 'center',
    backgroundColor: '#bbada0',
    borderRadius: 6,
    position: 'relative',
  },
  cell: {
    position: 'absolute',
    backgroundColor: '#cdc1b4',
    borderRadius: 3,
  },
  tile: {
    position: 'absolute',
    borderRadius: 3,
    justifyContent: 'center',
    alignItems: 'center',
  },
  tileText: {
    fontWeight: 'bold',
  },
  overlay: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    backgroundColor: 'rgba(238, 228, 218, 0.73)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  overlayContent: {
    backgroundColor: '#fff',
    padding: 40,
    borderRadius: 10,
    alignItems: 'center',
  },
  overlayTitle: {
    fontSize: 36,
    fontWeight: 'bold',
    color: '#776e65',
    marginBottom: 10,
  },
  overlayScore: {
    fontSize: 24,
    color: '#776e65',
    marginBottom: 20,
  },
  tryAgainButton: {
    backgroundColor: '#8f7a66',
    paddingHorizontal: 30,
    paddingVertical: 15,
    borderRadius: 3,
  },
  tryAgainText: {
    color: '#f9f6f2',
    fontWeight: 'bold',
    fontSize: 16,
  },
});
