// 2048 Game Logic

export type Tile = {
  id: string;
  value: number;
  position: { row: number; col: number };
  mergedFrom?: Tile[];
  isNew?: boolean;
};

export type GameState = {
  grid: (Tile | null)[][];
  score: number;
  bestScore: number;
  tiles: Tile[];
  won: boolean;
  gameOver: boolean;
};

const GRID_SIZE = 4;
const WIN_VALUE = 2048;

// Create empty grid
export const createEmptyGrid = (): (Tile | null)[][] => {
  return Array(GRID_SIZE)
    .fill(null)
    .map(() => Array(GRID_SIZE).fill(null));
};

// Get empty cells
const getEmptyCells = (grid: (Tile | null)[][]): { row: number; col: number }[] => {
  const empty: { row: number; col: number }[] = [];
  for (let row = 0; row < GRID_SIZE; row++) {
    for (let col = 0; col < GRID_SIZE; col++) {
      if (!grid[row][col]) {
        empty.push({ row, col });
      }
    }
  }
  return empty;
};

// Add random tile (2 or 4)
export const addRandomTile = (grid: (Tile | null)[][]): Tile | null => {
  const emptyCells = getEmptyCells(grid);
  if (emptyCells.length === 0) return null;

  const randomCell = emptyCells[Math.floor(Math.random() * emptyCells.length)];
  const value = Math.random() < 0.9 ? 2 : 4;
  const tile: Tile = {
    id: `${Date.now()}-${Math.random()}`,
    value,
    position: randomCell,
    isNew: true,
  };

  grid[randomCell.row][randomCell.col] = tile;
  return tile;
};

// Initialize game
export const initializeGame = (): GameState => {
  const grid = createEmptyGrid();
  const tiles: Tile[] = [];

  // Add 2 starting tiles
  const tile1 = addRandomTile(grid);
  const tile2 = addRandomTile(grid);
  if (tile1) tiles.push(tile1);
  if (tile2) tiles.push(tile2);

  return {
    grid,
    score: 0,
    bestScore: 0,
    tiles,
    won: false,
    gameOver: false,
  };
};

// Move tiles in a direction
export const move = (
  state: GameState,
  direction: 'up' | 'down' | 'left' | 'right'
): GameState => {
  const newGrid = createEmptyGrid();
  const newTiles: Tile[] = [];
  let score = state.score;
  let moved = false;
  let won = state.won;

  // Get traversal order based on direction
  const getTraversals = () => {
    const rowTraversal = direction === 'down' ? [3, 2, 1, 0] : [0, 1, 2, 3];
    const colTraversal = direction === 'right' ? [3, 2, 1, 0] : [0, 1, 2, 3];
    return { rowTraversal, colTraversal };
  };

  // Get vector for direction
  const getVector = () => {
    const vectors = {
      up: { row: -1, col: 0 },
      down: { row: 1, col: 0 },
      left: { row: 0, col: -1 },
      right: { row: 0, col: 1 },
    };
    return vectors[direction];
  };

  const { rowTraversal, colTraversal } = getTraversals();
  const vector = getVector();

  // Track merged tiles to prevent double merging
  const merged: boolean[][] = Array(GRID_SIZE)
    .fill(null)
    .map(() => Array(GRID_SIZE).fill(false));

  // Traverse grid in correct order
  rowTraversal.forEach((row) => {
    colTraversal.forEach((col) => {
      const tile = state.grid[row][col];
      if (!tile) return;

      // Find farthest position
      let currentRow = row;
      let currentCol = col;
      let nextRow = currentRow + vector.row;
      let nextCol = currentCol + vector.col;

      // Move as far as possible
      while (
        nextRow >= 0 &&
        nextRow < GRID_SIZE &&
        nextCol >= 0 &&
        nextCol < GRID_SIZE &&
        !newGrid[nextRow][nextCol]
      ) {
        currentRow = nextRow;
        currentCol = nextCol;
        nextRow = currentRow + vector.row;
        nextCol = currentCol + vector.col;
      }

      // Check if we can merge
      if (
        nextRow >= 0 &&
        nextRow < GRID_SIZE &&
        nextCol >= 0 &&
        nextCol < GRID_SIZE &&
        newGrid[nextRow][nextCol]?.value === tile.value &&
        !merged[nextRow][nextCol]
      ) {
        // Merge tiles
        const mergedTile: Tile = {
          id: `${Date.now()}-${Math.random()}`,
          value: tile.value * 2,
          position: { row: nextRow, col: nextCol },
          mergedFrom: [tile, newGrid[nextRow][nextCol]!],
        };

        newGrid[nextRow][nextCol] = mergedTile;
        newTiles.push(mergedTile);
        merged[nextRow][nextCol] = true;
        score += mergedTile.value;
        moved = true;

        if (mergedTile.value === WIN_VALUE) {
          won = true;
        }
      } else {
        // Just move tile
        const movedTile: Tile = {
          ...tile,
          position: { row: currentRow, col: currentCol },
          isNew: false,
        };
        newGrid[currentRow][currentCol] = movedTile;
        newTiles.push(movedTile);

        if (currentRow !== row || currentCol !== col) {
          moved = true;
        }
      }
    });
  });

  // If no tiles moved, return original state
  if (!moved) {
    return state;
  }

  // Add new random tile
  const newTile = addRandomTile(newGrid);
  if (newTile) {
    newTiles.push(newTile);
  }

  // Check if game is over
  const gameOver = !canMove(newGrid);

  return {
    grid: newGrid,
    score,
    bestScore: Math.max(score, state.bestScore),
    tiles: newTiles,
    won,
    gameOver,
  };
};

// Check if any moves are possible
const canMove = (grid: (Tile | null)[][]): boolean => {
  // Check for empty cells
  if (getEmptyCells(grid).length > 0) {
    return true;
  }

  // Check for possible merges
  for (let row = 0; row < GRID_SIZE; row++) {
    for (let col = 0; col < GRID_SIZE; col++) {
      const tile = grid[row][col];
      if (!tile) continue;

      // Check right
      if (col < GRID_SIZE - 1 && grid[row][col + 1]?.value === tile.value) {
        return true;
      }

      // Check down
      if (row < GRID_SIZE - 1 && grid[row + 1][col]?.value === tile.value) {
        return true;
      }
    }
  }

  return false;
};

// Get tile color based on value
export const getTileColor = (value: number): string => {
  const colors: { [key: number]: string } = {
    2: '#eee4da',
    4: '#ede0c8',
    8: '#f2b179',
    16: '#f59563',
    32: '#f67c5f',
    64: '#f65e3b',
    128: '#edcf72',
    256: '#edcc61',
    512: '#edc850',
    1024: '#edc53f',
    2048: '#edc22e',
  };
  return colors[value] || '#3c3a32';
};

// Get text color based on value
export const getTextColor = (value: number): string => {
  return value <= 4 ? '#776e65' : '#f9f6f2';
};
