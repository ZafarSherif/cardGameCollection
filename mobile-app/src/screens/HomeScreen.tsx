import React, { useState } from 'react';
import {
  StyleSheet,
  View,
  Text,
  ScrollView,
  SafeAreaView,
  StatusBar,
  Dimensions,
} from 'react-native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { GameCard } from '../components/GameCard';
import { HowToPlayModal } from '../components/HowToPlayModal';
import { LanguageSelector } from '../components/LanguageSelector';
import { usePlayerStore } from '../store/playerStore';
import { GAME_METADATA } from '../constants/games';
import { COLORS, SPACING, FONT_SIZES } from '../constants/theme';
import { RootStackParamList, GameType } from '../types';
import { useLanguage } from '../i18n/LanguageContext';

type HomeScreenNavigationProp = NativeStackNavigationProp<
  RootStackParamList,
  'Home'
>;

interface HomeScreenProps {
  navigation: HomeScreenNavigationProp;
}

export const HomeScreen: React.FC<HomeScreenProps> = ({ navigation }) => {
  const player = usePlayerStore((state) => state.player);
  const { t } = useLanguage();
  const [showHowToPlay, setShowHowToPlay] = useState(false);
  const [selectedGame, setSelectedGame] = useState<'solitaire' | 'game2048' | null>(null);

  // Detect landscape mode
  const { width, height } = Dimensions.get('window');
  const isLandscape = width > height;

  const handleGamePress = (gameType: GameType) => {
    if (gameType === GameType.SOLITAIRE) {
      navigation.navigate('Game', { gameType });
    } else if (gameType === GameType.GAME_2048) {
      navigation.navigate('Game2048');
    }
  };

  const handleHowToPlay = (gameType: GameType) => {
    if (gameType === GameType.SOLITAIRE) {
      setSelectedGame('solitaire');
      setShowHowToPlay(true);
    } else if (gameType === GameType.GAME_2048) {
      setSelectedGame('game2048');
      setShowHowToPlay(true);
    }
  };

  // Build games list with translations and sort (available first)
  const games = GAME_METADATA.map((meta) => {
    const gameKey = meta.id.toLowerCase().replace(/-/g, '_') as keyof typeof t.games;
    return {
      id: meta.id,
      name: t.games[gameKey]?.name || meta.id,
      description: t.games[gameKey]?.description || 'Coming soon!',
      icon: meta.icon,
      available: meta.available,
    };
  }).sort((a, b) => {
    // Available games first
    if (a.available && !b.available) return -1;
    if (!a.available && b.available) return 1;
    return 0;
  });

  return (
    <SafeAreaView style={styles.container}>
      <StatusBar barStyle="light-content" backgroundColor={COLORS.background} />

      {/* Language Selector */}
      <View style={styles.topBar}>
        <LanguageSelector />
      </View>

      {/* Header */}
      <View style={[styles.header, isLandscape && styles.headerLandscape]}>
        <View>
          <Text style={[styles.welcomeText, isLandscape && styles.textCompact]}>{t.home.welcomeBack}</Text>
          <Text style={[styles.username, isLandscape && styles.textCompact]}>{player.username}</Text>
        </View>

        <View style={styles.balanceContainer}>
          <Text style={[styles.balanceLabel, isLandscape && styles.textCompact]}>{t.home.balance}</Text>
          <View style={[styles.balanceAmount, isLandscape && styles.balanceAmountCompact]}>
            <Text style={[styles.coinIcon, isLandscape && styles.textCompact]}>💰</Text>
            <Text style={[styles.balanceText, isLandscape && styles.textCompact]}>{player.balance}</Text>
          </View>
        </View>
      </View>

      {/* Stats */}
      <View style={[styles.statsContainer, isLandscape && styles.statsContainerLandscape]}>
        <View style={styles.statItem}>
          <Text style={styles.statValue}>{player.level}</Text>
          <Text style={styles.statLabel}>{t.home.level}</Text>
        </View>

        <View style={styles.statDivider} />

        <View style={styles.statItem}>
          <Text style={styles.statValue}>{player.gamesPlayed}</Text>
          <Text style={styles.statLabel}>{t.home.games}</Text>
        </View>

        <View style={styles.statDivider} />

        <View style={styles.statItem}>
          <Text style={styles.statValue}>
            {player.gamesPlayed > 0
              ? Math.round((player.gamesWon / player.gamesPlayed) * 100)
              : 0}%
          </Text>
          <Text style={styles.statLabel}>{t.home.winRate}</Text>
        </View>
      </View>

      {/* Games List */}
      <ScrollView style={styles.scrollView} showsVerticalScrollIndicator={false}>
        <Text style={styles.sectionTitle}>{t.home.chooseGame}</Text>

        {games.map((game) => (
          <GameCard
            key={game.id}
            game={game}
            onPress={() => handleGamePress(game.id)}
            onHowToPlay={
              (game.id === GameType.SOLITAIRE || game.id === GameType.GAME_2048)
                ? () => handleHowToPlay(game.id)
                : undefined
            }
          />
        ))}

        <View style={styles.footer}>
          <Text style={styles.footerText}>{t.home.moreGamesComing}</Text>
        </View>
      </ScrollView>

      {/* How to Play Modal */}
      {selectedGame && (
        <HowToPlayModal
          visible={showHowToPlay}
          onClose={() => setShowHowToPlay(false)}
          gameType={selectedGame}
        />
      )}
    </SafeAreaView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: COLORS.background,
  },
  topBar: {
    padding: SPACING.md,
    backgroundColor: COLORS.background,
    alignItems: 'flex-end',
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: SPACING.lg,
    backgroundColor: COLORS.surface,
  },
  headerLandscape: {
    padding: SPACING.sm,
  },
  textCompact: {
    fontSize: FONT_SIZES.xs,
  },
  welcomeText: {
    fontSize: FONT_SIZES.sm,
    color: COLORS.textSecondary,
  },
  username: {
    fontSize: FONT_SIZES.xl,
    fontWeight: 'bold',
    color: COLORS.text,
  },
  balanceContainer: {
    alignItems: 'flex-end',
  },
  balanceLabel: {
    fontSize: FONT_SIZES.xs,
    color: COLORS.textSecondary,
    marginBottom: SPACING.xs,
  },
  balanceAmount: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: COLORS.cardBackground,
    paddingHorizontal: SPACING.md,
    paddingVertical: SPACING.sm,
    borderRadius: 20,
  },
  coinIcon: {
    fontSize: FONT_SIZES.md,
    marginRight: SPACING.xs,
  },
  balanceText: {
    fontSize: FONT_SIZES.lg,
    fontWeight: 'bold',
    color: COLORS.gold,
  },
  statsContainer: {
    flexDirection: 'row',
    backgroundColor: COLORS.cardBackground,
    margin: SPACING.lg,
    padding: SPACING.md,
    borderRadius: 12,
    justifyContent: 'space-around',
  },
  statsContainerLandscape: {
    margin: SPACING.sm,
    padding: SPACING.sm,
  },
  balanceAmountCompact: {
    paddingHorizontal: SPACING.sm,
    paddingVertical: SPACING.xs,
  },
  statItem: {
    flex: 1,
    alignItems: 'center',
  },
  statValue: {
    fontSize: FONT_SIZES.xl,
    fontWeight: 'bold',
    color: COLORS.text,
  },
  statLabel: {
    fontSize: FONT_SIZES.sm,
    color: COLORS.textSecondary,
    marginTop: SPACING.xs,
  },
  statDivider: {
    width: 1,
    backgroundColor: COLORS.surface,
  },
  scrollView: {
    flex: 1,
    paddingHorizontal: SPACING.lg,
  },
  sectionTitle: {
    fontSize: FONT_SIZES.xl,
    fontWeight: 'bold',
    color: COLORS.text,
    marginBottom: SPACING.md,
  },
  footer: {
    paddingVertical: SPACING.xl,
    alignItems: 'center',
  },
  footerText: {
    fontSize: FONT_SIZES.sm,
    color: COLORS.textSecondary,
  },
});
