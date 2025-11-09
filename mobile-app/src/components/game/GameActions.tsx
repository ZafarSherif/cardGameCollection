import React from 'react';
import { View, TouchableOpacity, Text, StyleSheet } from 'react-native';

interface GameActionsProps {
  onNewGame: () => void;
  onRestart: () => void;
  onUndo: () => void;
  orientation: 'portrait' | 'landscape';
}

export const GameActions: React.FC<GameActionsProps> = ({
  onNewGame,
  onRestart,
  onUndo,
  orientation,
}) => {
  const isLandscape = orientation === 'landscape';

  return (
    <View style={[styles.container, isLandscape && styles.containerLandscape]}>
      <TouchableOpacity style={styles.button} onPress={onNewGame}>
        <Text style={styles.buttonText}>New Game</Text>
      </TouchableOpacity>
      <TouchableOpacity style={styles.button} onPress={onRestart}>
        <Text style={styles.buttonText}>Restart</Text>
      </TouchableOpacity>
      <TouchableOpacity style={styles.button} onPress={onUndo}>
        <Text style={styles.buttonText}>Undo</Text>
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    alignItems: 'center',
    padding: 12,
    backgroundColor: '#34495e',
  },
  containerLandscape: {
    flexDirection: 'column',
    padding: 12,
    justifyContent: 'center',
    alignItems: 'stretch',
  },
  button: {
    backgroundColor: '#3498db',
    paddingHorizontal: 20,
    paddingVertical: 12,
    borderRadius: 8,
    marginVertical: 8,
    minWidth: 100,
  },
  buttonText: {
    color: '#fff',
    fontWeight: '600',
    fontSize: 14,
    textAlign: 'center',
  },
});
