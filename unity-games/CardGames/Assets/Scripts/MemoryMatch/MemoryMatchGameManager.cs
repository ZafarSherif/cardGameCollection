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
        [Header("Grid Settings")]
        public int gridRows = 4;
        public int gridCols = 4;
        public float cardSpacing = 1.5f;

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
        }

        void OnDestroy()
        {
            if (reactBridge != null)
            {
                ReactBridge.OnReactMessageReceived -= OnReactMessage;
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

            Debug.Log("[MemoryMatch] Game initialized");
        }

        /// <summary>
        /// Create 4x4 grid of cards with 8 pairs
        /// </summary>
        void CreateCardGrid()
        {
            if (cardPrefab == null)
            {
                Debug.LogError("[MemoryMatch] Card prefab not assigned!");
                return;
            }

            int totalCards = gridRows * gridCols; // 16 cards
            int pairsNeeded = totalCards / 2; // 8 pairs

            // Create list of pair IDs (0-7, each appearing twice)
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
            try
            {
                ReactAction action = ReactAction.FromJson(json);
                Debug.Log($"[MemoryMatch] Received action: {action.action}");

                switch (action.action)
                {
                    case "newGame":
                        InitializeGame();
                        break;
                    case "restart":
                        InitializeGame();
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

            string json = JsonUtility.ToJson(new GameStateMessage
            {
                type = "gameState",
                score = score,
                moves = moves,
                matches = matches,
                time = FormatTime(gameTime)
            });

            reactBridge.SendToReact("gameState", json);
        }

        /// <summary>
        /// Send game end event to React
        /// </summary>
        void SendGameEndToReact()
        {
            if (reactBridge == null) return;

            string json = JsonUtility.ToJson(new GameEndMessage
            {
                type = "gameEnd",
                won = true,
                finalScore = score,
                finalTime = FormatTime(gameTime),
                moves = moves
            });

            reactBridge.SendToReact("gameEnd", json);
        }

        string FormatTime(float seconds)
        {
            int minutes = (int)(seconds / 60);
            int secs = (int)(seconds % 60);
            return $"{minutes:00}:{secs:00}";
        }

        // Message classes for React communication
        [System.Serializable]
        class GameStateMessage
        {
            public string type;
            public int score;
            public int moves;
            public int matches;
            public string time;
        }

        [System.Serializable]
        class GameEndMessage
        {
            public string type;
            public bool won;
            public int finalScore;
            public string finalTime;
            public int moves;
        }
    }
}
