import React, { useEffect, useState } from 'react';
import {
  StyleSheet,
  View,
  Text,
  TouchableOpacity,
  SafeAreaView,
  Dimensions,
} from 'react-native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RouteProp } from '@react-navigation/native';
import { RootStackParamList } from '../types';
import { COLORS, SPACING, FONT_SIZES } from '../constants/theme';
import { useLanguage } from '../i18n/LanguageContext';
import { usePlayerStore } from '../store/playerStore';
import { analytics } from '../services/analytics';
import { useUnityGame } from '../hooks/useUnityGame';
import { GameHeader } from '../components/game/GameHeader';
import { GameActions } from '../components/game/GameActions';
import { WinModal } from '../components/game/WinModal';
import { UnityFrame } from '../components/game/UnityFrame';

type GameScreenNavigationProp = NativeStackNavigationProp<
  RootStackParamList,
  'Game'
>;

type GameScreenRouteProp = RouteProp<RootStackParamList, 'Game'>;

interface GameScreenProps {
  navigation: GameScreenNavigationProp;
  route: GameScreenRouteProp;
}

export const GameScreen: React.FC<GameScreenProps> = ({ navigation, route }) => {
  const { gameType } = route.params;
  const { t } = useLanguage();
  const { addGameResult, updateBalance } = usePlayerStore();
  const [orientation, setOrientation] = useState<'portrait' | 'landscape'>('portrait');
  const [gameStartTime] = useState(Date.now());

  // Use Unity game hook
  const { webViewRef, gameState, gameEndData, handleUnityMessage, newGame, restart, undo } =
    useUnityGame();

  const handleBack = () => {
    // Track game quit
    const timeSpent = Math.floor((Date.now() - gameStartTime) / 1000);
    analytics.trackGameQuit(gameType, timeSpent);
    navigation.goBack();
  };

  // Track game start
  useEffect(() => {
    analytics.trackGameStart(gameType);
  }, [gameType]);

  // Detect orientation changes
  useEffect(() => {
    const updateOrientation = () => {
      const { width, height } = Dimensions.get('window');
      setOrientation(width > height ? 'landscape' : 'portrait');
    };

    const subscription = Dimensions.addEventListener('change', updateOrientation);
    updateOrientation();

    return () => subscription?.remove();
  }, []);

  // Handle game end
  useEffect(() => {
    if (gameEndData?.won) {
      // Calculate time spent
      const timeSpent = Math.floor((Date.now() - gameStartTime) / 1000);

      // Track game completion
      analytics.trackGameComplete(
        gameType,
        gameEndData.won,
        gameEndData.finalScore,
        timeSpent
      );

      // Calculate coins earned (example: 10 coins per game + bonus based on score)
      const coinsEarned = 10 + Math.floor(gameEndData.finalScore / 10);
      updateBalance(coinsEarned);

      // Save game result
      addGameResult({
        gameType: gameType,
        won: gameEndData.won,
        score: gameEndData.finalScore,
        coinsEarned,
        timeSpent,
      });

      console.log('[RN] Game Complete! Earned:', coinsEarned, 'coins');
    }
  }, [gameEndData, gameType, addGameResult, updateBalance, gameStartTime]);

  const isLandscape = orientation === 'landscape';

  return (
    <SafeAreaView style={styles.container}>
      {/* Header with back button - only in portrait */}
      {!isLandscape && (
        <View style={styles.header}>
          <TouchableOpacity style={styles.backButton} onPress={handleBack}>
            <Text style={styles.backButtonText}>← {t.common.back}</Text>
          </TouchableOpacity>
          <Text style={styles.title}>{gameType.toUpperCase()}</Text>
        </View>
      )}

      {/* Game Stats Header - Top in portrait, Left in landscape */}
      {!isLandscape && (
        <GameHeader score={gameState.score} moves={gameState.moves} time={gameState.time} />
      )}

      <View style={[styles.gameRow, isLandscape && styles.gameRowLandscape]}>
        {/* Left panel in landscape */}
        {isLandscape && (
          <View style={styles.sidePanel}>
            <TouchableOpacity style={styles.backButtonLandscape} onPress={handleBack}>
              <Text style={styles.backButtonText}>←</Text>
            </TouchableOpacity>
            <GameHeader score={gameState.score} moves={gameState.moves} time={gameState.time} />
          </View>
        )}

        {/* Unity Game */}
        <View style={styles.gameContainer}>
          <UnityFrame
            webViewRef={webViewRef}
            onMessage={handleUnityMessage}
            style={styles.webview}
          />
        </View>

        {/* Right panel in landscape */}
        {isLandscape && (
          <View style={styles.sidePanel}>
            <GameActions
              onNewGame={newGame}
              onRestart={restart}
              onUndo={undo}
              orientation={orientation}
            />
          </View>
        )}
      </View>

      {/* Actions - Bottom in portrait */}
      {!isLandscape && (
        <GameActions
          onNewGame={newGame}
          onRestart={restart}
          onUndo={undo}
          orientation={orientation}
        />
      )}

      {/* Win Modal - Overlays everything */}
      {gameEndData?.won && (
        <WinModal
          visible={true}
          finalScore={gameEndData.finalScore}
          finalTime={gameEndData.finalTime}
          onNewGame={newGame}
          onClose={() => navigation.goBack()}
        />
      )}
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#1a1a1a',
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: SPACING.sm,
    backgroundColor: COLORS.surface,
  },
  backButton: {
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm,
  },
  backButtonLandscape: {
    paddingHorizontal: SPACING.sm,
    paddingVertical: SPACING.sm,
    alignSelf: 'flex-start',
  },
  backButtonText: {
    fontSize: FONT_SIZES.md,
    color: COLORS.primary,
    fontWeight: '600',
  },
  title: {
    flex: 1,
    fontSize: FONT_SIZES.lg,
    fontWeight: 'bold',
    color: COLORS.text,
    textAlign: 'center',
  },
  gameRow: {
    flex: 1,
  },
  gameRowLandscape: {
    flexDirection: 'row',
  },
  gameContainer: {
    flex: 1,
    backgroundColor: '#1a1a1a',
  },
  sidePanel: {
    width: 120,
    backgroundColor: '#2c3e50',
    justifyContent: 'flex-start',
    paddingTop: SPACING.md,
  },
  webview: {
    flex: 1,
    backgroundColor: '#1a1a1a',
  },
});
