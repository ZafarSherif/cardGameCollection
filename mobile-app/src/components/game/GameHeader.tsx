import React from 'react';
import { View, Text, StyleSheet } from 'react-native';

interface GameHeaderProps {
  score: number;
  moves: number;
  time: string;
}

export const GameHeader: React.FC<GameHeaderProps> = ({ score, moves, time }) => {
  return (
    <View style={styles.container}>
      <View style={styles.stat}>
        <Text style={styles.label}>Score</Text>
        <Text style={styles.value}>{score}</Text>
      </View>
      <View style={styles.stat}>
        <Text style={styles.label}>Moves</Text>
        <Text style={styles.value}>{moves}</Text>
      </View>
      <View style={styles.stat}>
        <Text style={styles.label}>Time</Text>
        <Text style={styles.value}>{time}</Text>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    alignItems: 'center',
    padding: 16,
    backgroundColor: '#2c3e50',
  },
  stat: {
    alignItems: 'center',
  },
  label: {
    color: '#95a5a6',
    fontSize: 12,
    marginBottom: 4,
    fontWeight: '500',
  },
  value: {
    color: '#ecf0f1',
    fontSize: 20,
    fontWeight: 'bold',
  },
});
