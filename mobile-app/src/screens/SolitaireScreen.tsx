import React, { useState, useEffect } from 'react';
import { View, StyleSheet, Dimensions } from 'react-native';
import { useUnityGame } from '../hooks/useUnityGame';
import { GameHeader } from '../components/game/GameHeader';
import { GameActions } from '../components/game/GameActions';
import { WinModal } from '../components/game/WinModal';
import { UnityFrame } from '../components/game/UnityFrame';

export const SolitaireScreen: React.FC = () => {
  const [orientation, setOrientation] = useState<'portrait' | 'landscape'>('portrait');
  const { webViewRef, gameState, gameEndData, isReady, handleUnityMessage, newGame, restart, undo } =
    useUnityGame();

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

  const isLandscape = orientation === 'landscape';

  return (
    <View style={styles.container}>
      {/* Header - Top in portrait, Left in landscape */}
      {!isLandscape && (
        <GameHeader score={gameState.score} moves={gameState.moves} time={gameState.time} />
      )}

      <View style={[styles.gameRow, isLandscape && styles.gameRowLandscape]}>
        {/* Left panel in landscape */}
        {isLandscape && (
          <View style={styles.sidePanel}>
            <GameHeader
              score={gameState.score}
              moves={gameState.moves}
              time={gameState.time}
              orientation={orientation}
            />
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
          onClose={() => {
            // Just close without starting new game
            // Could add setGameEndData(null) if we want to hide modal
          }}
        />
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#1a1a1a',
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
    justifyContent: 'center',
  },
  webview: {
    flex: 1,
    backgroundColor: '#1a1a1a',
  },
});
