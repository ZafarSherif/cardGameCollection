import React, { useEffect, useRef, useState } from 'react';
import {
  StyleSheet,
  View,
  Text,
  TouchableOpacity,
  SafeAreaView,
  Platform,
} from 'react-native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RouteProp } from '@react-navigation/native';
import { RootStackParamList } from '../types';
import { COLORS, SPACING, FONT_SIZES } from '../constants/theme';
import { useLanguage } from '../i18n/LanguageContext';
import { usePlayerStore } from '../store/playerStore';
import { analytics } from '../services/analytics';

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
  const { t, language } = useLanguage();
  const { addGameResult, updateBalance } = usePlayerStore();
  const [score, setScore] = useState(0);
  const [moves, setMoves] = useState(0);
  const [gameStartTime] = useState(Date.now());

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

  // Listen for messages from Unity (Web only for now)
  useEffect(() => {
    if (Platform.OS === 'web') {
      const handleMessage = (event: MessageEvent) => {
        // Check if message is from Unity
        if (event.data && event.data.source === 'unity') {
          console.log('[RN] Received from Unity:', event.data);

          switch (event.data.type) {
            case 'GameReady':
              console.log('[RN] Game is ready!');
              // Could send initial data to Unity here
              break;

            case 'ScoreUpdate':
              const scoreData = JSON.parse(event.data.data);
              setScore(scoreData.score);
              break;

            case 'MovesUpdate':
              const movesData = JSON.parse(event.data.data);
              setMoves(movesData.moves);
              break;

            case 'GameComplete':
              const resultData = JSON.parse(event.data.data);
              console.log('[RN] Game Complete!', resultData);

              // Track game completion
              analytics.trackGameComplete(
                gameType,
                resultData.won,
                resultData.score,
                resultData.timeSeconds
              );

              // Update player balance
              updateBalance(resultData.coinsEarned);

              // Save game result
              addGameResult({
                gameType: gameType,
                won: resultData.won,
                score: resultData.score,
                coinsEarned: resultData.coinsEarned,
                timeSpent: resultData.timeSeconds,
              });

              // Unity now handles the win panel UI
              // React layer just updates stats silently
              console.log('[RN] Stats updated: +' + resultData.coinsEarned + ' coins');
              break;
          }
        }
      };

      window.addEventListener('message', handleMessage);
      return () => window.removeEventListener('message', handleMessage);
    }
  }, [gameType, addGameResult, updateBalance]);

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity style={styles.backButton} onPress={handleBack}>
          <Text style={styles.backButtonText}>← {t.common.back}</Text>
        </TouchableOpacity>
        <Text style={styles.title}>{gameType.toUpperCase()}</Text>
        <View style={styles.stats}>
          <Text style={styles.statsText}>Score: {score} | Moves: {moves}</Text>
        </View>
      </View>

      {/* Unity view - Web version with iframe */}
      {Platform.OS === 'web' && (
        <View style={styles.gameContainer}>
          <iframe
            src="/unity/index.html"
            style={{
              width: '100%',
              height: '100%',
              border: 'none',
            }}
            title="Unity Game"
          />
        </View>
      )}

      {/* Mobile version - Placeholder for now */}
      {Platform.OS !== 'web' && (
        <View style={styles.gameContainer}>
          <View style={styles.placeholder}>
            <Text style={styles.placeholderIcon}>🎮</Text>
            <Text style={styles.placeholderTitle}>Mobile Version</Text>
            <Text style={styles.placeholderText}>
              Mobile integration coming soon!
            </Text>
            <Text style={styles.placeholderSubtext}>
              For now, please use the web version
            </Text>
          </View>
        </View>
      )}
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background,
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: SPACING.md,
    backgroundColor: COLORS.surface,
  },
  backButton: {
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm,
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
  stats: {
    paddingHorizontal: SPACING.sm,
  },
  statsText: {
    fontSize: FONT_SIZES.xs,
    color: COLORS.textSecondary,
  },
  gameContainer: {
    flex: 1,
    backgroundColor: COLORS.cardBackground,
    margin: SPACING.md,
    borderRadius: 12,
    overflow: 'hidden',
  },
  placeholder: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: SPACING.xl,
  },
  placeholderIcon: {
    fontSize: 64,
    marginBottom: SPACING.md,
  },
  placeholderTitle: {
    fontSize: FONT_SIZES.xl,
    fontWeight: 'bold',
    color: COLORS.text,
    marginBottom: SPACING.sm,
  },
  placeholderText: {
    fontSize: FONT_SIZES.md,
    color: COLORS.textSecondary,
    textAlign: 'center',
    marginBottom: SPACING.xs,
  },
  placeholderSubtext: {
    fontSize: FONT_SIZES.sm,
    color: COLORS.textSecondary,
    textAlign: 'center',
    fontStyle: 'italic',
  },
});
