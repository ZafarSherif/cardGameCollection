export type Difficulty = 'easy' | 'medium' | 'hard';

export interface Tile {
  id: number;
  position: number;
  isEmpty: boolean;
}

export interface PuzzleState {
  tiles: Tile[];
  emptyPosition: number;
  moves: number;
  gridSize: { rows: number; cols: number };
  isWon: boolean;
}

/**
 * Get grid dimensions based on difficulty
 */
export const getGridSize = (difficulty: Difficulty): { rows: number; cols: number } => {
  switch (difficulty) {
    case 'easy':
      return { rows: 2, cols: 3 }; // 6 tiles (3x2)
    case 'medium':
      return { rows: 3, cols: 3 }; // 9 tiles (3x3)
    case 'hard':
      return { rows: 4, cols: 4 }; // 16 tiles (4x4)
  }
};

/**
 * Initialize puzzle with solved state
 */
export const initializePuzzle = (difficulty: Difficulty): PuzzleState => {
  const gridSize = getGridSize(difficulty);
  const totalTiles = gridSize.rows * gridSize.cols;

  // Create tiles in solved order (0 to n-1, with last tile empty)
  const tiles: Tile[] = [];
  for (let i = 0; i < totalTiles; i++) {
    tiles.push({
      id: i,
      position: i,
      isEmpty: i === totalTiles - 1, // Last tile is empty
    });
  }

  return {
    tiles,
    emptyPosition: totalTiles - 1,
    moves: 0,
    gridSize,
    isWon: false,
  };
};

/**
 * Check if puzzle is solved (all tiles in correct position)
 */
export const isPuzzleSolved = (state: PuzzleState): boolean => {
  return state.tiles.every((tile) => tile.id === tile.position);
};

/**
 * Get neighboring positions of a tile
 */
const getNeighbors = (position: number, gridSize: { rows: number; cols: number }): number[] => {
  const { rows, cols } = gridSize;
  const row = Math.floor(position / cols);
  const col = position % cols;
  const neighbors: number[] = [];

  // Up
  if (row > 0) neighbors.push((row - 1) * cols + col);
  // Down
  if (row < rows - 1) neighbors.push((row + 1) * cols + col);
  // Left
  if (col > 0) neighbors.push(row * cols + (col - 1));
  // Right
  if (col < cols - 1) neighbors.push(row * cols + (col + 1));

  return neighbors;
};

/**
 * Check if a tile can be moved (is next to empty space)
 */
export const canMoveTile = (state: PuzzleState, position: number): boolean => {
  const neighbors = getNeighbors(position, state.gridSize);
  return neighbors.includes(state.emptyPosition);
};

/**
 * Move a tile to empty space (if valid)
 */
export const moveTile = (state: PuzzleState, position: number): PuzzleState => {
  if (!canMoveTile(state, position)) {
    return state; // Invalid move
  }

  // Swap tile with empty space
  const newTiles = state.tiles.map((tile) => {
    if (tile.position === position) {
      return { ...tile, position: state.emptyPosition };
    }
    if (tile.position === state.emptyPosition) {
      return { ...tile, position };
    }
    return tile;
  });

  const newState: PuzzleState = {
    ...state,
    tiles: newTiles,
    emptyPosition: position,
    moves: state.moves + 1,
  };

  // Check if puzzle is solved
  newState.isWon = isPuzzleSolved(newState);

  return newState;
};

/**
 * Shuffle puzzle ensuring it's solvable
 * Uses random valid moves from solved state
 */
export const shufflePuzzle = (state: PuzzleState, numMoves: number = 100): PuzzleState => {
  let currentState = { ...state, moves: 0 }; // Reset move counter
  let lastMove = -1;

  for (let i = 0; i < numMoves; i++) {
    const neighbors = getNeighbors(currentState.emptyPosition, currentState.gridSize);

    // Filter out the tile we just moved (prevent immediate undo)
    const validNeighbors = neighbors.filter((pos) => pos !== lastMove);

    // Pick random neighbor
    const randomIndex = Math.floor(Math.random() * validNeighbors.length);
    const tileToMove = validNeighbors[randomIndex];

    lastMove = currentState.emptyPosition;
    currentState = moveTile(currentState, tileToMove);
  }

  // Reset moves counter after shuffle
  currentState.moves = 0;
  currentState.isWon = false;

  return currentState;
};

/**
 * Get tile by position
 */
export const getTileAtPosition = (state: PuzzleState, position: number): Tile | undefined => {
  return state.tiles.find((tile) => tile.position === position);
};

/**
 * Get direction to move tile (for animation)
 */
export const getMoveDirection = (
  tilePosition: number,
  emptyPosition: number,
  cols: number
): 'up' | 'down' | 'left' | 'right' | null => {
  const diff = emptyPosition - tilePosition;

  if (diff === cols) return 'down';
  if (diff === -cols) return 'up';
  if (diff === 1) return 'right';
  if (diff === -1) return 'left';

  return null;
};
