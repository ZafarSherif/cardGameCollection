import React from 'react';
import { View, TouchableOpacity, Text, StyleSheet } from 'react-native';

type Difficulty = 'Easy' | 'Medium' | 'Hard';

interface DifficultySelectorProps {
  currentDifficulty: Difficulty;
  onDifficultyChange: (difficulty: Difficulty) => void;
  disabled?: boolean;
  orientation?: 'portrait' | 'landscape';
}

export const DifficultySelector: React.FC<DifficultySelectorProps> = ({
  currentDifficulty,
  onDifficultyChange,
  disabled = false,
  orientation = 'portrait',
}) => {
  const difficulties: Difficulty[] = ['Easy', 'Medium', 'Hard'];
  const isLandscape = orientation === 'landscape';

  return (
    <View style={[styles.container, isLandscape && styles.containerLandscape]}>
      <Text style={styles.label}>Difficulty:</Text>
      <View style={[styles.buttonContainer, isLandscape && styles.buttonContainerLandscape]}>
        {difficulties.map((difficulty) => (
          <TouchableOpacity
            key={difficulty}
            style={[
              styles.button,
              isLandscape && styles.buttonLandscape,
              currentDifficulty === difficulty && styles.buttonActive,
              disabled && styles.buttonDisabled,
            ]}
            onPress={() => !disabled && onDifficultyChange(difficulty)}
            disabled={disabled}
          >
            <Text
              style={[
                styles.buttonText,
                currentDifficulty === difficulty && styles.buttonTextActive,
              ]}
            >
              {difficulty}
            </Text>
          </TouchableOpacity>
        ))}
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    paddingVertical: 8,
    paddingHorizontal: 12,
    backgroundColor: '#2c3e50',
  },
  containerLandscape: {
    flexDirection: 'column',
    paddingVertical: 12,
  },
  label: {
    color: '#ecf0f1',
    fontSize: 14,
    fontWeight: '600',
    marginRight: 12,
  },
  buttonContainer: {
    flexDirection: 'row',
    gap: 8,
  },
  buttonContainerLandscape: {
    flexDirection: 'column',
    width: '100%',
    gap: 6,
  },
  button: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 6,
    backgroundColor: '#34495e',
    borderWidth: 2,
    borderColor: 'transparent',
  },
  buttonLandscape: {
    width: '100%',
    marginTop: 4,
  },
  buttonActive: {
    backgroundColor: '#3498db',
    borderColor: '#2980b9',
  },
  buttonDisabled: {
    opacity: 0.5,
  },
  buttonText: {
    color: '#bdc3c7',
    fontSize: 13,
    fontWeight: '600',
  },
  buttonTextActive: {
    color: '#fff',
  },
});
