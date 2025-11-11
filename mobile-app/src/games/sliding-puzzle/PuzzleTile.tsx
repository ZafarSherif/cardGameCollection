import React, { useEffect, useRef } from 'react';
import { TouchableOpacity, Image, StyleSheet, Animated, View, PanResponder } from 'react-native';
import { Tile } from './gameLogic';

interface PuzzleTileProps {
  tile: Tile;
  tileSize: number;
  imageSource: any;
  gridCols: number;
  onPress: () => void;
  canMove: boolean;
  onSwipe?: (direction: 'up' | 'down' | 'left' | 'right') => void;
  onLongPressStart?: () => void;
  onLongPressEnd?: () => void;
}

export const PuzzleTile: React.FC<PuzzleTileProps> = ({
  tile,
  tileSize,
  imageSource,
  gridCols,
  onPress,
  canMove,
  onSwipe,
  onLongPressStart,
  onLongPressEnd,
}) => {
  const animatedPosition = useRef(new Animated.ValueXY({
    x: (tile.position % gridCols) * tileSize,
    y: Math.floor(tile.position / gridCols) * tileSize,
  })).current;

  // Animate tile position when it changes
  useEffect(() => {
    const targetX = (tile.position % gridCols) * tileSize;
    const targetY = Math.floor(tile.position / gridCols) * tileSize;

    Animated.spring(animatedPosition, {
      toValue: { x: targetX, y: targetY },
      useNativeDriver: true,
      tension: 80,
      friction: 10,
    }).start();
  }, [tile.position, tileSize, gridCols]);

  // Long press timer and state
  const longPressTimer = useRef<NodeJS.Timeout | null>(null);
  const wasLongPress = useRef(false);

  const handleTouchStart = () => {
    wasLongPress.current = false;
    onLongPressStart?.();
    longPressTimer.current = setTimeout(() => {
      // Long press detected - set flag to prevent tap action
      wasLongPress.current = true;
    }, 300);
  };

  const handleTouchEnd = () => {
    onLongPressEnd?.();
    if (longPressTimer.current) {
      clearTimeout(longPressTimer.current);
      longPressTimer.current = null;
    }
  };

  // Cleanup on unmount
  useEffect(() => {
    return () => {
      if (longPressTimer.current) {
        clearTimeout(longPressTimer.current);
      }
    };
  }, []);

  // Pan responder for swipe gestures
  const panResponder = PanResponder.create({
    onStartShouldSetPanResponder: () => false,
    onMoveShouldSetPanResponder: (evt, gestureState) => {
      // Only capture if there's significant movement (more than 5 pixels)
      return Math.abs(gestureState.dx) > 5 || Math.abs(gestureState.dy) > 5;
    },
    onPanResponderTerminationRequest: () => false, // Don't allow termination - keep tracking even if swipe goes outside
    onPanResponderTerminate: () => {
      handleTouchEnd();
    },
    onPanResponderGrant: () => {
      handleTouchStart();
    },
    onPanResponderMove: () => {},
    onPanResponderRelease: (evt, gestureState) => {
      handleTouchEnd();

      const { dx, dy } = gestureState;
      const minSwipeDistance = 30;

      // Determine swipe direction
      if (Math.abs(dx) > Math.abs(dy)) {
        // Horizontal swipe
        if (Math.abs(dx) > minSwipeDistance) {
          if (dx > 0) {
            // Swiped right
            onSwipe?.('right');
          } else {
            // Swiped left
            onSwipe?.('left');
          }
        } else {
          // Not a swipe, check if it was a long press
          if (!wasLongPress.current) {
            onPress();
          }
        }
      } else {
        // Vertical swipe
        if (Math.abs(dy) > minSwipeDistance) {
          if (dy > 0) {
            // Swiped down
            onSwipe?.('down');
          } else {
            // Swiped up
            onSwipe?.('up');
          }
        } else {
          // Not a swipe, check if it was a long press
          if (!wasLongPress.current) {
            onPress();
          }
        }
      }
    },
  });

  if (tile.isEmpty) {
    // Empty space - invisible but handles long press
    return (
      <Animated.View
        style={[
          styles.tileContainer,
          {
            width: tileSize,
            height: tileSize,
            transform: animatedPosition.getTranslateTransform(),
          },
        ]}
      >
        <TouchableOpacity
          style={styles.tile}
          activeOpacity={1}
          onPressIn={handleTouchStart}
          onPressOut={handleTouchEnd}
        />
      </Animated.View>
    );
  }

  // Calculate which part of the image to show
  const originalRow = Math.floor(tile.id / gridCols);
  const originalCol = tile.id % gridCols;

  return (
    <Animated.View
      style={[
        styles.tileContainer,
        {
          width: tileSize,
          height: tileSize,
          transform: animatedPosition.getTranslateTransform(),
        },
      ]}
    >
      <TouchableOpacity
        style={styles.tile}
        activeOpacity={0.7}
        onPress={() => {
          // Only trigger press if it wasn't a long press
          if (!wasLongPress.current) {
            onPress();
          }
          wasLongPress.current = false; // Reset for next interaction
        }}
        onPressIn={handleTouchStart}
        onPressOut={handleTouchEnd}
        {...panResponder.panHandlers}
      >
        <Image
          source={imageSource}
          style={[
            styles.tileImage,
            {
              width: tileSize * gridCols,
              height: tileSize * gridCols,
              left: -originalCol * tileSize,
              top: -originalRow * tileSize,
            },
          ]}
          resizeMode="cover"
        />
        <View
          style={[
            styles.tileBorder,
            canMove && styles.tileBorderHighlight,
          ]}
        />
      </TouchableOpacity>
    </Animated.View>
  );
};

const styles = StyleSheet.create({
  tileContainer: {
    position: 'absolute',
  },
  tile: {
    width: '100%',
    height: '100%',
    overflow: 'hidden',
    borderRadius: 4,
  },
  tileImage: {
    position: 'absolute',
  },
  tileBorder: {
    position: 'absolute',
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    borderWidth: 1,
    borderColor: 'rgba(255, 255, 255, 0.3)',
    borderRadius: 4,
  },
  tileBorderHighlight: {
    borderColor: 'rgba(52, 152, 219, 0.6)',
    borderWidth: 2,
  },
});
