import React from 'react';
import {
  StyleSheet,
  View,
  Text,
  ScrollView,
  SafeAreaView,
  StatusBar,
} from 'react-native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { GameCard } from '../components/GameCard';
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

  const handleGamePress = (gameType: GameType) => {
    if (gameType === GameType.SOLITAIRE) {
      navigation.navigate('Game', { gameType });
    }
  };

  // Build games list with translations
  const games = GAME_METADATA.map((meta) => {
    const gameKey = meta.id.toLowerCase() as 'solitaire' | 'poker' | 'blackjack';
    return {
      id: meta.id,
      name: t.games[gameKey].name,
      description: t.games[gameKey].description,
      icon: meta.icon,
      available: meta.available,
    };
  });

  return (
    <SafeAreaView style={styles.container}>
      <StatusBar barStyle="light-content" backgroundColor={COLORS.background} />

      {/* Language Selector */}
      <View style={styles.topBar}>
        <LanguageSelector />
      </View>

      {/* Header */}
      <View style={styles.header}>
        <View>
          <Text style={styles.welcomeText}>{t.home.welcomeBack}</Text>
          <Text style={styles.username}>{player.username}</Text>
        </View>

        <View style={styles.balanceContainer}>
          <Text style={styles.balanceLabel}>{t.home.balance}</Text>
          <View style={styles.balanceAmount}>
            <Text style={styles.coinIcon}>💰</Text>
            <Text style={styles.balanceText}>{player.balance}</Text>
          </View>
        </View>
      </View>

      {/* Stats */}
      <View style={styles.statsContainer}>
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
          />
        ))}

        <View style={styles.footer}>
          <Text style={styles.footerText}>{t.home.moreGamesComing}</Text>
        </View>
      </ScrollView>
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
