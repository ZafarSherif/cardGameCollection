import React from 'react';
import { StyleSheet, TouchableOpacity, View, Text } from 'react-native';
import { Game } from '../types';
import { COLORS, SPACING, FONT_SIZES } from '../constants/theme';
import { useLanguage } from '../i18n/LanguageContext';

interface GameCardProps {
  game: Game;
  onPress: () => void;
  onHowToPlay?: () => void;
}

export const GameCard: React.FC<GameCardProps> = ({ game, onPress, onHowToPlay }) => {
  const { t } = useLanguage();

  return (
    <TouchableOpacity
      style={[styles.container, !game.available && styles.disabled]}
      onPress={onPress}
      disabled={!game.available}
      activeOpacity={0.7}
    >
      <View style={styles.iconContainer}>
        <Text style={styles.icon}>{game.icon}</Text>
      </View>

      <View style={styles.content}>
        <Text style={styles.name}>{game.name}</Text>
        <Text style={styles.description}>{game.description}</Text>

        {!game.available && (
          <View style={styles.comingSoonBadge}>
            <Text style={styles.comingSoonText}>{t.games.comingSoon}</Text>
          </View>
        )}
      </View>

      {/* How to Play button - only show if handler provided */}
      {onHowToPlay && game.available && (
        <TouchableOpacity
          style={styles.infoButton}
          onPress={(e) => {
            e.stopPropagation();
            onHowToPlay();
          }}
          activeOpacity={0.7}
        >
          <Text style={styles.infoIcon}>ℹ️</Text>
        </TouchableOpacity>
      )}
    </TouchableOpacity>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    backgroundColor: COLORS.cardBackground,
    borderRadius: 12,
    padding: SPACING.md,
    marginBottom: SPACING.md,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.3,
    shadowRadius: 4,
    elevation: 5,
  },
  disabled: {
    opacity: 0.6,
  },
  iconContainer: {
    width: 60,
    height: 60,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: COLORS.surface,
    borderRadius: 12,
    marginRight: SPACING.md,
  },
  icon: {
    fontSize: 32,
  },
  content: {
    flex: 1,
    justifyContent: 'center',
  },
  name: {
    fontSize: FONT_SIZES.lg,
    fontWeight: 'bold',
    color: COLORS.text,
    marginBottom: SPACING.xs,
  },
  description: {
    fontSize: FONT_SIZES.sm,
    color: COLORS.textSecondary,
  },
  comingSoonBadge: {
    backgroundColor: COLORS.warning,
    paddingHorizontal: SPACING.sm,
    paddingVertical: 4,
    borderRadius: 8,
    alignSelf: 'flex-start',
    marginTop: SPACING.xs,
  },
  comingSoonText: {
    fontSize: FONT_SIZES.xs,
    fontWeight: 'bold',
    color: COLORS.background,
  },
  infoButton: {
    width: 40,
    height: 40,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: COLORS.surface,
    borderRadius: 20,
    marginLeft: SPACING.sm,
  },
  infoIcon: {
    fontSize: 20,
  },
});
