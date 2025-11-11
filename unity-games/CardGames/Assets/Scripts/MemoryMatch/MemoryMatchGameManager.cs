using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardGames.Core;

namespace MemoryMatch
{
    /// <summary>
    /// Main game manager for Memory Match
    /// Handles grid layout, card matching, scoring, and React communication
    /// </summary>
    public class MemoryMatchGameManager : MonoBehaviour
    {
        public enum Difficulty { Easy, Medium, Hard }

        [Header("Game Settings")]
        public Difficulty currentDifficulty = Difficulty.Medium;

        [Header("Grid Settings")]
        public int gridRows = 5;
        public int gridCols = 4;  // Default to Medium difficulty (5x4)
        public float cardSpacing = 1.5f;  // Spacing between cards
        public float cardScale = 0.85f;  // Scale multiplier for card size

        [Header("Card Prefab")]
        public GameObject cardPrefab;

        [Header("Card Sprites")]
        public Sprite cardBackSprite;
        public Sprite[] cardFrontSprites; // Will use first 8 for pairs

        [Header("Game State")]
        public int moves = 0;
        public int matches = 0;
        public float gameTime = 0f;
        public int score = 0;
        public bool gameWon = false;

        private MemoryCard[,] cardGrid;
        private List<MemoryCard> allCards = new List<MemoryCard>();
        private MemoryCard firstFlippedCard = null;
        private MemoryCard secondFlippedCard = null;
        private bool isCheckingMatch = false;
        private ReactBridge reactBridge;

        void Start()
        {
            reactBridge = ReactBridge.Instance;

            if (reactBridge != null)
            {
                ReactBridge.OnReactMessageReceived += OnReactMessage;
                Debug.Log("[MemoryMatch] Subscribed to React messages");
            }

            InitializeGame();
            StartCoroutine(UpdateTimerCoroutine());
        }

        void OnDestroy()
        {
            if (reactBridge != null)
            {
                ReactBridge.OnReactMessageReceived -= OnReactMessage;
            }
        }

        /// <summary>
        /// Update timer in React UI every second
        /// </summary>
        private IEnumerator UpdateTimerCoroutine()
        {
            Debug.Log("[Timer] Coroutine started");
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (!gameWon)
                {
                    SendGameStateToReact();
                }
            }
        }

        void Update()
        {
            if (!gameWon)
            {
                gameTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// Initialize new game
        /// </summary>
        void InitializeGame()
        {
            Debug.Log("[MemoryMatch] Initializing game");

            // Set grid size based on difficulty
            SetGridSizeForDifficulty();

            // Clear existing cards
            ClearCards();

            // Reset game state
            moves = 0;
            matches = 0;
            gameTime = 0f;
            score = 0;
            gameWon = false;
            firstFlippedCard = null;
            secondFlippedCard = null;

            // Create and shuffle cards
            CreateCardGrid();

            // Send initial state to React
            SendGameStateToReact();

            Debug.Log($"[MemoryMatch] Game initialized - Difficulty: {currentDifficulty}, Grid: {gridRows}x{gridCols}");
        }

        /// <summary>
        /// Set grid size and card scale based on difficulty
        /// </summary>
        void SetGridSizeForDifficulty()
        {
            switch (currentDifficulty)
            {
                case Difficulty.Easy:
                    gridRows = 4;
                    gridCols = 4;  // 16 cards, 8 pairs
                    cardScale = 0.80f;  // Bigger cards for easy mode
                    cardSpacing = 1.7f;
                    break;
                case Difficulty.Medium:
                    gridRows = 5;
                    gridCols = 4;  // 20 cards, 10 pairs
                    cardScale = 0.75f;  // Medium cards
                    cardSpacing = 1.6f;
                    break;
                case Difficulty.Hard:
                    gridRows = 6;
                    gridCols = 4;  // 24 cards, 12 pairs
                    cardScale = 0.70f;  // Smaller cards to fit more
                    cardSpacing = 1.5f;
                    break;
            }
        }

        /// <summary>
        /// Create grid of cards based on difficulty
        /// Easy: 4x4 (8 pairs), Medium: 5x4 (10 pairs), Hard: 6x4 (12 pairs)
        /// </summary>
        void CreateCardGrid()
        {
            if (cardPrefab == null)
            {
                Debug.LogError("[MemoryMatch] Card prefab not assigned!");
                return;
            }

            int totalCards = gridRows * gridCols;
            int pairsNeeded = totalCards / 2;

            // Check if we have enough unique sprites
            if (pairsNeeded > cardFrontSprites.Length)
            {
                Debug.LogError($"[MemoryMatch] Not enough card sprites! Need {pairsNeeded} unique sprites, but only have {cardFrontSprites.Length}. " +
                              $"Add more sprites to cardFrontSprites array in Inspector to avoid reusing sprites with different pairIds.");
            }

            // Create list of pair IDs (0 to pairsNeeded-1, each appearing twice)
            List<int> pairIds = new List<int>();
            for (int i = 0; i < pairsNeeded; i++)
            {
                pairIds.Add(i);
                pairIds.Add(i);
            }

            // Shuffle the pairs
            pairIds = pairIds.OrderBy(x => Random.value).ToList();

            // Calculate grid offset to center it
            float gridWidth = (gridCols - 1) * cardSpacing;
            float gridHeight = (gridRows - 1) * cardSpacing;
            Vector3 gridOffset = new Vector3(-gridWidth / 2f, gridHeight / 2f, 0);

            cardGrid = new MemoryCard[gridRows, gridCols];
            int cardIndex = 0;

            for (int row = 0; row < gridRows; row++)
            {
                for (int col = 0; col < gridCols; col++)
                {
                    // Calculate position
                    Vector3 position = gridOffset + new Vector3(
                        col * cardSpacing,
                        -row * cardSpacing,
                        0
                    );

                    // Instantiate card
                    GameObject cardObj = Instantiate(cardPrefab, position, Quaternion.identity, transform);
                    cardObj.name = $"Card_{row}_{col}";

                    // Apply card scale based on difficulty
                    cardObj.transform.localScale = Vector3.one * cardScale;

                    MemoryCard card = cardObj.GetComponent<MemoryCard>();
                    if (card == null)
                    {
                        card = cardObj.AddComponent<MemoryCard>();
                    }

                    // Get pair ID for this card
                    int pairId = pairIds[cardIndex];

                    // Assign sprite (use modulo to cycle through available sprites)
                    Sprite frontSprite = cardFrontSprites[pairId % cardFrontSprites.Length];

                    // Initialize card
                    card.Initialize("", "", pairId, frontSprite, cardBackSprite);

                    // Add to grid and list
                    cardGrid[row, col] = card;
                    allCards.Add(card);

                    cardIndex++;
                }
            }

            Debug.Log($"[MemoryMatch] Created {allCards.Count} cards with {pairsNeeded} pairs");
        }

        /// <summary>
        /// Handle card click
        /// </summary>
        public void OnCardClicked(MemoryCard card)
        {
            // Ignore if checking match or card already flipped
            if (isCheckingMatch || card.isFaceUp || card.isMatched)
            {
                Debug.Log($"[MemoryMatch] Ignoring click - checking:{isCheckingMatch}, faceUp:{card.isFaceUp}, matched:{card.isMatched}");
                return;
            }

            Debug.Log($"[MemoryMatch] Card clicked - pairId:{card.pairId}");

            // Flip the card
            card.FlipUp();

            if (firstFlippedCard == null)
            {
                // First card of pair
                firstFlippedCard = card;
                Debug.Log($"[MemoryMatch] First card selected - pairId:{card.pairId}");
            }
            else if (secondFlippedCard == null)
            {
                // Second card of pair - check for match
                secondFlippedCard = card;
                moves++;
                Debug.Log($"[MemoryMatch] Second card selected - pairId:{card.pairId}, moves:{moves}");

                StartCoroutine(CheckMatch());
            }
        }

        /// <summary>
        /// Check if two flipped cards match
        /// </summary>
        IEnumerator CheckMatch()
        {
            isCheckingMatch = true;

            // Wait for flip animation
            yield return new WaitForSeconds(0.3f);

            if (firstFlippedCard.IsMatch(secondFlippedCard))
            {
                // Match!
                Debug.Log($"[MemoryMatch] Match found! pairId:{firstFlippedCard.pairId}");

                firstFlippedCard.SetMatched();
                secondFlippedCard.SetMatched();

                matches++;
                score += 100; // +100 per match

                // Check if game is won
                if (matches == (gridRows * gridCols) / 2)
                {
                    gameWon = true;
                    int timeBonus = Mathf.Max(0, 300 - (int)gameTime);
                    int moveBonus = Mathf.Max(0, (100 - moves) * 10);
                    score += timeBonus + moveBonus;

                    Debug.Log($"[MemoryMatch] Game won! Score:{score}, Time:{gameTime:F1}s, Moves:{moves}");
                    SendGameEndToReact();
                }
            }
            else
            {
                // No match - flip back after delay
                Debug.Log($"[MemoryMatch] No match - {firstFlippedCard.pairId} != {secondFlippedCard.pairId}");

                yield return new WaitForSeconds(1.0f);

                firstFlippedCard.FlipDown();
                secondFlippedCard.FlipDown();
            }

            // Reset for next pair
            firstFlippedCard = null;
            secondFlippedCard = null;
            isCheckingMatch = false;

            SendGameStateToReact();
        }

        /// <summary>
        /// Clear all cards from scene
        /// </summary>
        void ClearCards()
        {
            foreach (MemoryCard card in allCards)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);
                }
            }
            allCards.Clear();
        }

        /// <summary>
        /// Handle messages from React Native
        /// </summary>
        void OnReactMessage(string json)
        {
            Debug.Log($"[MemoryMatch] OnReactMessage called with JSON: {json}");

            try
            {
                ReactAction action = ReactAction.FromJson(json);
                Debug.Log($"[MemoryMatch] Parsed action: {action.action}, data: {action.data}");

                switch (action.action)
                {
                    case "newGame":
                        Debug.Log("[MemoryMatch] Executing newGame action");
                        InitializeGame();
                        break;
                    case "restart":
                        Debug.Log("[MemoryMatch] Executing restart action");
                        InitializeGame();
                        break;
                    case "setDifficulty":
                        // Parse difficulty from action data
                        if (!string.IsNullOrEmpty(action.data))
                        {
                            try
                            {
                                DifficultyData diffData = JsonUtility.FromJson<DifficultyData>(action.data);
                                if (System.Enum.TryParse(diffData.difficulty, true, out Difficulty diff))
                                {
                                    currentDifficulty = diff;
                                    Debug.Log($"[MemoryMatch] Difficulty set to: {currentDifficulty}");
                                    // Restart game with new difficulty
                                    InitializeGame();
                                }
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError($"[MemoryMatch] Failed to parse difficulty: {e.Message}");
                            }
                        }
                        break;
                    default:
                        Debug.LogWarning($"[MemoryMatch] Unknown action: {action.action}");
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[MemoryMatch] Failed to parse React message: {e.Message}");
            }
        }

        /// <summary>
        /// Send current game state to React
        /// </summary>
        void SendGameStateToReact()
        {
            if (reactBridge == null) return;

            GameStatePayload payload = new GameStatePayload
            {
                score = score,
                moves = moves,
                matches = matches,
                time = FormatTime(gameTime)
            };

            reactBridge.SendToReact("gameState", JsonUtility.ToJson(payload));
        }

        /// <summary>
        /// Send game end event to React
        /// </summary>
        void SendGameEndToReact()
        {
            if (reactBridge == null) return;

            GameEndPayload payload = new GameEndPayload
            {
                won = true,
                finalScore = score,
                finalTime = FormatTime(gameTime),
                moves = moves
            };

            reactBridge.SendToReact("gameEnd", JsonUtility.ToJson(payload));
        }

        string FormatTime(float seconds)
        {
            int minutes = (int)(seconds / 60);
            int secs = (int)(seconds % 60);
            return $"{minutes:00}:{secs:00}";
        }

        // Payload classes for React communication
        [System.Serializable]
        class GameStatePayload
        {
            public int score;
            public int moves;
            public int matches;
            public string time;
        }

        [System.Serializable]
        class GameEndPayload
        {
            public bool won;
            public int finalScore;
            public string finalTime;
            public int moves;
        }

        [System.Serializable]
        class DifficultyData
        {
            public string difficulty;
        }
    }
}
