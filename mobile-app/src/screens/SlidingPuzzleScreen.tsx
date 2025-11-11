import React, { useState, useEffect, useRef } from 'react';
import {
  StyleSheet,
  View,
  Text,
  TouchableOpacity,
  SafeAreaView,
  ScrollView,
  Dimensions,
  Image,
} from 'react-native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RouteProp } from '@react-navigation/native';
import { RootStackParamList } from '../types';
import { useLanguage } from '../i18n/LanguageContext';
import { usePlayerStore } from '../store/playerStore';
import { analytics } from '../services/analytics';
import { CommonGameHeader } from '../components/game/CommonGameHeader';
import { GameHeader } from '../components/game/GameHeader';
import { WinModal } from '../components/game/WinModal';
import { HowToPlayModal } from '../components/HowToPlayModal';
import { DifficultySelector } from '../components/game/DifficultySelector';
import { PuzzleTile } from '../games/sliding-puzzle/PuzzleTile';
import {
  Difficulty,
  PuzzleState,
  initializePuzzle,
  shufflePuzzle,
  canMoveTile,
  moveTile,
  getGridSize,
} from '../games/sliding-puzzle/gameLogic';

type SlidingPuzzleScreenNavigationProp = NativeStackNavigationProp<
  RootStackParamList,
  'SlidingPuzzle'
>;

type SlidingPuzzleScreenRouteProp = RouteProp<RootStackParamList, 'SlidingPuzzle'>;

interface SlidingPuzzleScreenProps {
  navigation: SlidingPuzzleScreenNavigationProp;
  route: SlidingPuzzleScreenRouteProp;
}

// Puzzle images
const PUZZLE_IMAGES = [
  { id: 1, source: require('../assets/puzzles/landscape1.jpg'), name: 'Mountain Lake' },
  { id: 2, source: require('../assets/puzzles/landscape2.jpg'), name: 'Sunset Beach' },
  { id: 3, source: require('../assets/puzzles/landscape3.jpg'), name: 'Forest Path' },
];

export const SlidingPuzzleScreen: React.FC<SlidingPuzzleScreenProps> = ({ navigation }) => {
  const { t } = useLanguage();
  const { addGameResult, updateBalance } = usePlayerStore();
  const [difficulty, setDifficulty] = useState<Difficulty>('medium');
  const [puzzleState, setPuzzleState] = useState<PuzzleState>(initializePuzzle(difficulty));
  const [currentImageIndex, setCurrentImageIndex] = useState(0);
  const [gameStartTime] = useState(Date.now());
  const [showHowToPlay, setShowHowToPlay] = useState(false);
  const [gameTime, setGameTime] = useState(0);
  const [showReferenceOverlay, setShowReferenceOverlay] = useState(false);
  const longPressTimer = useRef<NodeJS.Timeout | null>(null);

  const currentImage = PUZZLE_IMAGES[currentImageIndex];

  // Timer
  useEffect(() => {
    const interval = setInterval(() => {
      if (!puzzleState.isWon) {
        setGameTime((prev) => prev + 1);
      }
    }, 1000);

    return () => clearInterval(interval);
  }, [puzzleState.isWon]);

  // Cleanup long press timer on unmount
  useEffect(() => {
    return () => {
      if (longPressTimer.current) {
        clearTimeout(longPressTimer.current);
      }
    };
  }, []);

  // Track game start
  useEffect(() => {
    analytics.trackGameStart('sliding_puzzle');
  }, []);

  // Handle win
  useEffect(() => {
    if (puzzleState.isWon) {
      const timeSpent = Math.floor((Date.now() - gameStartTime) / 1000);
      const coinsEarned = Math.floor(1000 / puzzleState.moves); // Better score for fewer moves

      updateBalance(coinsEarned);
      addGameResult({
        gameType: 'sliding_puzzle',
        won: true,
        score: 1000 - puzzleState.moves * 10,
        coinsEarned,
        timeSpent,
      });
    }
  }, [puzzleState.isWon]);

  const handleBack = () => {
    const timeSpent = Math.floor((Date.now() - gameStartTime) / 1000);
    analytics.trackGameQuit('sliding_puzzle', timeSpent);
    navigation.goBack();
  };

  const handleNewGame = () => {
    const newState = initializePuzzle(difficulty);
    const shuffled = shufflePuzzle(newState, 100);
    setPuzzleState(shuffled);
    setGameTime(0);
  };

  const handleDifficultyChange = (newDifficulty: 'Easy' | 'Medium' | 'Hard') => {
    const difficultyMap: { [key: string]: Difficulty } = {
      Easy: 'easy',
      Medium: 'medium',
      Hard: 'hard',
    };
    setDifficulty(difficultyMap[newDifficulty]);
    const newState = initializePuzzle(difficultyMap[newDifficulty]);
    const shuffled = shufflePuzzle(newState, 100);
    setPuzzleState(shuffled);
    setGameTime(0);
  };

  const handleTilePress = (position: number) => {
    if (!canMoveTile(puzzleState, position)) return;

    const newState = moveTile(puzzleState, position);
    setPuzzleState(newState);
  };

  const handleSwipe = (position: number, direction: 'up' | 'down' | 'left' | 'right') => {
    // Calculate the target position based on swipe direction
    const row = Math.floor(position / gridSize.cols);
    const col = position % gridSize.cols;
    let targetPosition = position;

    switch (direction) {
      case 'up':
        if (row > 0) targetPosition = position - gridSize.cols;
        break;
      case 'down':
        if (row < gridSize.rows - 1) targetPosition = position + gridSize.cols;
        break;
      case 'left':
        if (col > 0) targetPosition = position - 1;
        break;
      case 'right':
        if (col < gridSize.cols - 1) targetPosition = position + 1;
        break;
    }

    // Check if the target position is the empty space
    if (targetPosition === puzzleState.emptyPosition) {
      const newState = moveTile(puzzleState, position);
      setPuzzleState(newState);
    }
  };

  const handleImageChange = () => {
    setCurrentImageIndex((prev) => (prev + 1) % PUZZLE_IMAGES.length);
    handleNewGame();
  };

  const handleBoardTouchStart = () => {
    // Show overlay after 300ms of holding
    longPressTimer.current = setTimeout(() => {
      setShowReferenceOverlay(true);
    }, 300);
  };

  const handleBoardTouchEnd = () => {
    // Cancel timer and hide overlay
    if (longPressTimer.current) {
      clearTimeout(longPressTimer.current);
      longPressTimer.current = null;
    }
    setShowReferenceOverlay(false);
  };

  // Calculate puzzle board size
  const { width: screenWidth } = Dimensions.get('window');
  const boardPadding = 40;
  const maxBoardSize = Math.min(screenWidth - boardPadding, 500);
  const gridSize = getGridSize(difficulty);
  const tileSize = maxBoardSize / gridSize.cols;
  const boardSize = tileSize * gridSize.cols;

  const formatTime = (seconds: number): string => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  };

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <CommonGameHeader
        title="Sliding Puzzle"
        onBack={handleBack}
        onHowToPlay={() => setShowHowToPlay(true)}
      />

      {/* Game Stats */}
      <GameHeader
        score={puzzleState.isWon ? 1000 - puzzleState.moves * 10 : 0}
        moves={puzzleState.moves}
        time={formatTime(gameTime)}
      />

      {/* Scrollable Content */}
      <ScrollView
        style={styles.scrollView}
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        {/* Difficulty Selector */}
        <DifficultySelector
          currentDifficulty={
            difficulty === 'easy' ? 'Easy' : difficulty === 'medium' ? 'Medium' : 'Hard'
          }
          onDifficultyChange={handleDifficultyChange}
        />

        {/* Reference Image */}
        <View style={styles.referenceContainer}>
          <Text style={styles.referenceLabel}>Reference:</Text>
          <Image
            source={currentImage.source}
            style={styles.referenceImage}
            resizeMode="cover"
          />
          <TouchableOpacity style={styles.changeImageButton} onPress={handleImageChange}>
            <Text style={styles.changeImageText}>Change Image</Text>
          </TouchableOpacity>
        </View>

        {/* Puzzle Board */}
        <View style={styles.boardContainer}>
          <View
            style={[
              styles.board,
              {
                width: boardSize,
                height: boardSize,
              },
            ]}
          >
            {puzzleState.tiles.map((tile) => (
              <PuzzleTile
                key={tile.id}
                tile={tile}
                tileSize={tileSize}
                imageSource={currentImage.source}
                gridCols={gridSize.cols}
                onPress={() => handleTilePress(tile.position)}
                onSwipe={(direction) => handleSwipe(tile.position, direction)}
                onLongPressStart={handleBoardTouchStart}
                onLongPressEnd={handleBoardTouchEnd}
                canMove={canMoveTile(puzzleState, tile.position)}
              />
            ))}

            {/* Reference Overlay - shown on long press */}
            {showReferenceOverlay && (
              <View style={styles.referenceOverlay} pointerEvents="none">
                <Image
                  source={currentImage.source}
                  style={{
                    width: boardSize,
                    height: boardSize,
                    opacity: 0.4,
                  }}
                  resizeMode="cover"
                />
              </View>
            )}
          </View>
        </View>

        {/* Action Buttons */}
        <View style={styles.actions}>
          <TouchableOpacity style={styles.button} onPress={handleNewGame}>
            <Text style={styles.buttonText}>{t.common.newGame}</Text>
          </TouchableOpacity>
        </View>
      </ScrollView>

      {/* Win Modal */}
      {puzzleState.isWon && (
        <WinModal
          visible={true}
          finalScore={1000 - puzzleState.moves * 10}
          finalTime={formatTime(gameTime)}
          onClose={() => navigation.goBack()}
          onNewGame={handleNewGame}
        />
      )}

      {/* How to Play Modal */}
      <HowToPlayModal
        visible={showHowToPlay}
        onClose={() => setShowHowToPlay(false)}
        gameType="sliding_puzzle"
      />
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#2c3e50',
  },
  scrollView: {
    flex: 1,
  },
  scrollContent: {
    flexGrow: 1,
    paddingBottom: 20,
  },
  referenceContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 12,
    paddingHorizontal: 16,
    backgroundColor: '#34495e',
  },
  referenceLabel: {
    color: '#ecf0f1',
    fontSize: 14,
    fontWeight: '600',
    marginRight: 12,
  },
  referenceImage: {
    width: 80,
    height: 80,
    borderRadius: 8,
    borderWidth: 2,
    borderColor: '#3498db',
  },
  changeImageButton: {
    marginLeft: 12,
    paddingHorizontal: 12,
    paddingVertical: 8,
    backgroundColor: '#3498db',
    borderRadius: 6,
  },
  changeImageText: {
    color: '#fff',
    fontSize: 12,
    fontWeight: '600',
  },
  boardContainer: {
    justifyContent: 'center',
    alignItems: 'center',
    paddingVertical: 20,
  },
  board: {
    backgroundColor: '#1a1a1a',
    borderRadius: 8,
    position: 'relative',
  },
  referenceOverlay: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    justifyContent: 'center',
    alignItems: 'center',
    borderRadius: 8,
    overflow: 'hidden',
  },
  actions: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    padding: 16,
    backgroundColor: '#34495e',
  },
  button: {
    backgroundColor: '#3498db',
    paddingHorizontal: 32,
    paddingVertical: 14,
    borderRadius: 8,
    minWidth: 120,
  },
  buttonText: {
    color: '#fff',
    fontWeight: '600',
    fontSize: 16,
    textAlign: 'center',
  },
});
