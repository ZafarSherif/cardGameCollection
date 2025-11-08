using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardGames.Core;

namespace CardGames.Solitaire
{
    /// <summary>
    /// Main game manager for Solitaire
    /// </summary>
    public class SolitaireGameManager : MonoBehaviour
    {
        [Header("Card Prefab")]
        [SerializeField] private GameObject cardPrefab;

        [Header("Piles")]
        [SerializeField] private Pile stockPile;
        [SerializeField] private Pile wastePile;
        [SerializeField] private Pile[] tableauPiles = new Pile[7];
        [SerializeField] private Pile[] foundationPiles = new Pile[4];

        [Header("Game Settings")]
        [SerializeField] private int drawCount = 3; // Draw 1 or 3 cards from stock

        [Header("Cheat Mode (Testing)")]
        [SerializeField] private bool cheatMode = false;
        public static bool CheatModeEnabled { get; private set; } = false;

        [Header("Scoring")]
        [SerializeField] private int score = 0;
        [SerializeField] private int moves = 0;

        private List<Card> deck = new List<Card>();
        private float gameStartTime;
        private bool gameWon = false;

        // Restart functionality - save initial deck order
        private List<Card> initialDeckOrder = new List<Card>();

        // Undo functionality
        private Stack<MoveRecord> moveHistory = new Stack<MoveRecord>();
        private const int maxUndoSteps = 10; // Limit undo history

        // React Native communication
        private ReactBridge reactBridge;

        /// <summary>
        /// Record of a move for undo functionality
        /// </summary>
        private class MoveRecord
        {
            public List<Card> cards;
            public Pile sourcePile;
            public Pile targetPile;
            public bool sourceCardWasFaceDown; // If we flipped a card
            public int previousScore;
            public int previousMoves;

            public MoveRecord(List<Card> cards, Pile source, Pile target, bool cardFlipped, int score, int moves)
            {
                this.cards = new List<Card>(cards);
                this.sourcePile = source;
                this.targetPile = target;
                this.sourceCardWasFaceDown = cardFlipped;
                this.previousScore = score;
                this.previousMoves = moves;
            }
        }

        private void Start()
        {
            // Get ReactBridge reference
            reactBridge = ReactBridge.Instance;

            // Subscribe to React messages
            ReactBridge.OnReactMessageReceived += OnReactMessage;

            // Position piles first, then initialize game
            PositionPilesInSquare();
            InitializeGame();
            CheatModeEnabled = cheatMode;

            // Start timer update coroutine (updates React UI every second)
            StartCoroutine(UpdateTimerCoroutine());
        }

        private void OnDestroy()
        {
            // Unsubscribe from React messages
            ReactBridge.OnReactMessageReceived -= OnReactMessage;
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

        private void Update()
        {
            // Toggle cheat mode with C key
            if (Input.GetKeyDown(KeyCode.C))
            {
                cheatMode = !cheatMode;
                CheatModeEnabled = cheatMode;
                Debug.Log($"[CheatMode] {(cheatMode ? "ENABLED" : "DISABLED")} - Any card can go on empty tableau piles");
            }
        }

        /// <summary>
        /// Called by ResponsiveLayout when screen size changes
        /// </summary>
        private void OnGameAreaChanged()
        {
            PositionPilesInSquare();
        }

        /// <summary>
        /// Position all piles within the square game area
        /// </summary>
        private void PositionPilesInSquare()
        {
            ResponsiveLayout layout = GetComponent<ResponsiveLayout>();
            if (layout == null)
            {
                Debug.LogWarning("[SolitaireGameManager] ResponsiveLayout component not found!");
                return;
            }

            Rect gameArea = layout.GetGameAreaBounds();
            float squareSize = layout.GetSquareSize();

            Debug.Log($"[SolitaireGameManager] Positioning piles in square (size: {squareSize:F2})");

            // Calculate positions within the square
            float cardWidth = squareSize / 12f; // Card size reference for spacing
            float cardHeight = cardWidth * 1.4f; // Card aspect ratio

            // Tableau piles - 7 columns evenly spaced
            float tableauSpacing = squareSize / 7f; // Maximum spacing between columns
            float tableauStartX = gameArea.xMin + (squareSize - tableauSpacing * 6) / 2; // Center the 7 piles
            float tableauY = gameArea.yMin + squareSize * 0.47f; // Position 47% up from bottom (more room for card stacks)

            for (int i = 0; i < 7; i++)
            {
                if (tableauPiles[i] != null)
                {
                    float x = tableauStartX + (i * tableauSpacing);
                    tableauPiles[i].transform.position = new Vector3(x, tableauY, 0);
                }
            }

            // Foundation piles - 4 piles at top right
            float foundationSpacing = cardWidth * 1.8f; // More spacing between foundation piles
            float foundationStartX = gameArea.xMax - (foundationSpacing * 3.8f); // Moved to the right
            float foundationY = gameArea.yMax - squareSize * 0.18f; // Position 18% down from top (smaller gap)

            for (int i = 0; i < 4; i++)
            {
                if (foundationPiles[i] != null)
                {
                    float x = foundationStartX + (i * foundationSpacing);
                    foundationPiles[i].transform.position = new Vector3(x, foundationY, 0);
                }
            }

            // Stock and Waste piles - top left
            float stockX = gameArea.xMin + cardWidth * 0.8f;
            float stockY = foundationY; // Same Y as foundation

            if (stockPile != null)
                stockPile.transform.position = new Vector3(stockX, stockY, 0);

            if (wastePile != null)
                wastePile.transform.position = new Vector3(stockX + cardWidth * 2.0f, stockY, 0); // More space from stock pile

            // Scale all cards based on square size
            ScaleAllCards(cardWidth);

            Debug.Log($"[SolitaireGameManager] Piles positioned successfully");
        }

        /// <summary>
        /// Scale all cards proportionally to the square size
        /// </summary>
        private void ScaleAllCards(float targetCardWidth)
        {
            // Calculate scale based on target card width
            // Card prefab is manually scaled to 0.55, use larger multiplier to maintain good size
            float baseScale = targetCardWidth * 1.2f;

            Debug.Log($"[SolitaireGameManager] Scaling cards to {baseScale:F3}");

            // Scale all cards in all piles
            ScalePileCards(stockPile, baseScale);
            ScalePileCards(wastePile, baseScale);

            foreach (var pile in tableauPiles)
            {
                ScalePileCards(pile, baseScale);
            }

            foreach (var pile in foundationPiles)
            {
                ScalePileCards(pile, baseScale);
            }
        }

        /// <summary>
        /// Scale all cards in a specific pile
        /// </summary>
        private void ScalePileCards(Pile pile, float scale)
        {
            if (pile == null) return;

            int cardCount = 0;
            foreach (Card card in pile.Cards)
            {
                if (card != null)
                {
                    card.transform.localScale = new Vector3(scale, scale, 1f);
                    cardCount++;
                }
            }

            if (cardCount > 0)
            {
                Debug.Log($"[SolitaireGameManager] Scaled {cardCount} cards in {pile.Type} pile to {scale:F3}");
            }
        }

        /// <summary>
        /// Initialize and start a new game
        /// </summary>
        public void InitializeGame()
        {
            score = 0;
            moves = 0;
            gameWon = false;
            gameStartTime = Time.time;
            moveHistory.Clear();

            ClearGame();
            CreateDeck();
            ShuffleDeck();

            // Save the initial deck order for restart functionality
            initialDeckOrder.Clear();
            initialDeckOrder.AddRange(deck);

            DealCards();

            // Position and scale piles/cards after dealing
            PositionPilesInSquare();

            UpdateUI();
        }

        /// <summary>
        /// Clear all piles and destroy all cards
        /// </summary>
        private void ClearGame()
        {
            // Destroy all existing card objects
            foreach (Card card in deck)
            {
                if (card != null)
                    Destroy(card.gameObject);
            }
            deck.Clear();

            // Clear all piles
            if (stockPile != null) stockPile.Clear();
            if (wastePile != null) wastePile.Clear();
            foreach (var pile in tableauPiles)
            {
                if (pile != null) pile.Clear();
            }
            foreach (var pile in foundationPiles)
            {
                if (pile != null) pile.Clear();
            }
        }

        /// <summary>
        /// Create a standard 52-card deck
        /// </summary>
        private void CreateDeck()
        {
            for (int suit = 0; suit < 4; suit++)
            {
                for (int rank = 1; rank <= 13; rank++)
                {
                    GameObject cardObj = Instantiate(cardPrefab);
                    Card card = cardObj.GetComponent<Card>();

                    // Initialize card with suit and rank
                    // Sprites will be loaded automatically by CardSpriteManager
                    card.Initialize(
                        (Card.Suit)suit,
                        (Card.Rank)rank
                    );

                    card.SetFaceUp(false);
                    cardObj.name = card.GetCardName();

                    // Add DraggableCard component
                    if (cardObj.GetComponent<DraggableCard>() == null)
                    {
                        cardObj.AddComponent<DraggableCard>();
                    }

                    deck.Add(card);
                }
            }
        }

        /// <summary>
        /// Shuffle the deck using Fisher-Yates algorithm
        /// </summary>
        private void ShuffleDeck()
        {
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                Card temp = deck[i];
                deck[i] = deck[randomIndex];
                deck[randomIndex] = temp;
            }
        }

        /// <summary>
        /// Deal cards to tableau and stock piles
        /// </summary>
        private void DealCards()
        {
            int cardIndex = 0;

            // Deal to tableau piles
            for (int pileIndex = 0; pileIndex < 7; pileIndex++)
            {
                Pile pile = tableauPiles[pileIndex];
                if (pile == null) continue;

                // Deal (pileIndex + 1) cards to each pile
                for (int cardNum = 0; cardNum <= pileIndex; cardNum++)
                {
                    if (cardIndex >= deck.Count) break;

                    Card card = deck[cardIndex];
                    cardIndex++;

                    // Last card in each pile is face up
                    bool faceUp = (cardNum == pileIndex);
                    card.SetFaceUp(faceUp);

                    pile.AddCard(card);
                }
            }

            // Remaining cards go to stock pile
            while (cardIndex < deck.Count)
            {
                Card card = deck[cardIndex];
                cardIndex++;
                card.SetFaceUp(false);
                stockPile.AddCard(card);
            }
        }

        /// <summary>
        /// Draw cards from stock to waste
        /// </summary>
        public void DrawFromStock()
        {
            if (stockPile.IsEmpty())
            {
                // Reset stock from waste
                RecycleWasteToStock();
                return;
            }

            // Draw drawCount cards (or remaining cards if less)
            int cardsToDraw = Mathf.Min(drawCount, stockPile.CardCount);

            for (int i = 0; i < cardsToDraw; i++)
            {
                Card card = stockPile.RemoveTopCard();
                if (card != null)
                {
                    card.SetFaceUp(true);
                    wastePile.AddCard(card);
                }
            }

            // Play card place sound for stock draw
            if (CardGames.Core.AudioManager.Instance != null)
            {
                CardGames.Core.AudioManager.Instance.PlayCardPlace();
            }

            moves++;
            UpdateUI();
        }

        /// <summary>
        /// Recycle waste pile back to stock with shuffling
        /// </summary>
        private void RecycleWasteToStock()
        {
            // Collect all waste cards into a temporary list
            List<Card> cardsToRecycle = new List<Card>();

            while (!wastePile.IsEmpty())
            {
                Card card = wastePile.RemoveTopCard();
                card.SetFaceUp(false);
                cardsToRecycle.Add(card);
            }

            // Shuffle the cards before adding them to stock (Fisher-Yates algorithm)
            for (int i = cardsToRecycle.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                Card temp = cardsToRecycle[i];
                cardsToRecycle[i] = cardsToRecycle[randomIndex];
                cardsToRecycle[randomIndex] = temp;
            }

            // Add shuffled cards to stock pile
            foreach (Card card in cardsToRecycle)
            {
                stockPile.AddCard(card);
            }

            Debug.Log($"[RecycleWasteToStock] Recycled and shuffled {cardsToRecycle.Count} cards back to stock");
        }

        /// <summary>
        /// Try to move cards from one pile to another
        /// </summary>
        public bool TryMoveCards(List<Card> cards, Pile sourcePile, Pile targetPile)
        {
            if (cards == null || cards.Count == 0) return false;
            if (sourcePile == null || targetPile == null) return false;

            Card bottomCard = cards[0];

            // Foundation can only accept ONE card at a time
            if (targetPile.Type == Pile.PileType.Foundation && cards.Count > 1)
            {
                Debug.Log("✗ Foundation only accepts single cards");
                return false;
            }

            // Check if target pile can accept the bottom card
            if (!targetPile.CanAcceptCard(bottomCard))
                return false;

            // Check if we'll flip a card (for undo)
            bool willFlipCard = false;
            if (sourcePile.Type == Pile.PileType.Tableau && sourcePile.CardCount > cards.Count)
            {
                Card cardThatWillBeOnTop = sourcePile.Cards[sourcePile.Cards.IndexOf(bottomCard) - 1];
                if (cardThatWillBeOnTop != null && !cardThatWillBeOnTop.IsFaceUp)
                {
                    willFlipCard = true;
                }
            }

            // Record move for undo (before making changes)
            RecordMove(cards, sourcePile, targetPile, willFlipCard);

            // Remove cards from source pile
            sourcePile.RemoveCardsFrom(bottomCard);

            // Add cards to target pile
            targetPile.AddCards(cards);

            // Play card place sound
            if (CardGames.Core.AudioManager.Instance != null)
            {
                CardGames.Core.AudioManager.Instance.PlayCardPlace();
            }

            // Flip top card of source pile if needed
            if (sourcePile.Type == Pile.PileType.Tableau)
            {
                sourcePile.FlipTopCard();
            }

            // Update score and moves
            moves++;
            UpdateScore(sourcePile.Type, targetPile.Type);

            // Check win condition
            CheckWinCondition();

            UpdateUI();

            return true;
        }

        /// <summary>
        /// Try to auto-move a card to foundation
        /// </summary>
        public void TryAutoMoveToFoundation(Card card)
        {
            if (card == null) return;

            // Find appropriate foundation pile for this card's suit
            int suitIndex = (int)card.CardSuit;
            if (suitIndex < 0 || suitIndex >= foundationPiles.Length) return;

            Pile targetFoundation = foundationPiles[suitIndex];
            if (targetFoundation == null) return;

            // Check if card can be placed on foundation
            if (!targetFoundation.CanAcceptCard(card)) return;

            // Get the cards to move (just this card for foundation)
            List<Card> cardsToMove = new List<Card> { card };

            // Move the card
            TryMoveCards(cardsToMove, card.CurrentPile, targetFoundation);
        }

        /// <summary>
        /// Update score based on move type
        /// </summary>
        private void UpdateScore(Pile.PileType fromType, Pile.PileType toType)
        {
            // Add points for moving to foundation
            if (toType == Pile.PileType.Foundation)
            {
                score += 10;
            }

            // Add points for revealing card
            if (fromType == Pile.PileType.Tableau)
            {
                score += 5;
            }

            // Subtract points for moving from foundation
            if (fromType == Pile.PileType.Foundation)
            {
                score -= 15;
            }

            SendGameStateToReact();
        }

        /// <summary>
        /// Check if player has won
        /// </summary>
        private void CheckWinCondition()
        {
            // Win if all foundation piles have 13 cards each
            foreach (Pile foundation in foundationPiles)
            {
                if (foundation.CardCount != 13)
                    return;
            }

            // Player won!
            gameWon = true;
            int gameTime = Mathf.RoundToInt(Time.time - gameStartTime);

            // Bonus for completing game
            score += 100;

            SendGameEndToReact(true, score, gameTime);
        }

        /// <summary>
        /// Update UI elements
        /// </summary>
        private void UpdateUI()
        {
            SendGameStateToReact();
        }

        /// <summary>
        /// Get game state for saving/debugging
        /// </summary>
        public GameState GetGameState()
        {
            return new GameState
            {
                score = score,
                moves = moves,
                gameTime = Time.time - gameStartTime,
                isWon = gameWon
            };
        }

        [System.Serializable]
        public struct GameState
        {
            public int score;
            public int moves;
            public float gameTime;
            public bool isWon;
        }

        /// <summary>
        /// Restart the current game with the same initial deck order
        /// </summary>
        public void RestartGame()
        {
            Debug.Log("[GameManager] Restarting game with same deck order");

            // Check if we have an initial deck to restart from
            if (initialDeckOrder == null || initialDeckOrder.Count == 0)
            {
                Debug.LogWarning("[GameManager] No initial deck saved, starting new game instead");
                InitializeGame();
                return;
            }

            // Reset game state
            score = 0;
            moves = 0;
            gameWon = false;
            gameStartTime = Time.time;
            moveHistory.Clear();

            // Clear all piles (but don't destroy cards)
            if (stockPile != null) stockPile.Clear();
            if (wastePile != null) wastePile.Clear();
            foreach (var pile in tableauPiles)
            {
                if (pile != null) pile.Clear();
            }
            foreach (var pile in foundationPiles)
            {
                if (pile != null) pile.Clear();
            }

            // Restore deck to initial order
            deck.Clear();
            deck.AddRange(initialDeckOrder);

            // Re-deal cards from saved order
            DealCards();

            UpdateUI();
        }

        /// <summary>
        /// Record a move for undo functionality
        /// </summary>
        private void RecordMove(List<Card> cards, Pile source, Pile target, bool cardWasFlipped)
        {
            // Limit undo history
            if (moveHistory.Count >= maxUndoSteps)
            {
                // Remove oldest move (at bottom of stack)
                Stack<MoveRecord> temp = new Stack<MoveRecord>();
                while (moveHistory.Count > maxUndoSteps - 1)
                {
                    temp.Push(moveHistory.Pop());
                }
                moveHistory.Clear();
                while (temp.Count > 0)
                {
                    moveHistory.Push(temp.Pop());
                }
            }

            MoveRecord record = new MoveRecord(cards, source, target, cardWasFlipped, score, moves);
            moveHistory.Push(record);

            Debug.Log($"[Undo] Recorded move: {cards.Count} card(s) from {source.Type} to {target.Type}");
        }

        /// <summary>
        /// Undo the last move
        /// </summary>
        public bool UndoLastMove()
        {
            if (moveHistory.Count == 0)
            {
                Debug.Log("[Undo] No moves to undo");
                return false;
            }

            MoveRecord lastMove = moveHistory.Pop();

            Debug.Log($"[Undo] Undoing move: {lastMove.cards.Count} card(s) from {lastMove.targetPile.Type} back to {lastMove.sourcePile.Type}");

            // Move cards back from target to source
            foreach (Card card in lastMove.cards)
            {
                lastMove.targetPile.RemoveCardsFrom(card);
            }
            lastMove.sourcePile.AddCards(lastMove.cards);

            // If we flipped a card during the original move, flip it back
            if (lastMove.sourceCardWasFaceDown && lastMove.sourcePile.Type == Pile.PileType.Tableau)
            {
                Card topCard = lastMove.sourcePile.GetTopCard();
                if (topCard != null && topCard.IsFaceUp)
                {
                    // Find the card that was flipped (before the moved cards)
                    int movedCardIndex = lastMove.sourcePile.Cards.IndexOf(lastMove.cards[0]);
                    if (movedCardIndex > 0)
                    {
                        Card flippedCard = lastMove.sourcePile.Cards[movedCardIndex - 1];
                        if (flippedCard != null)
                        {
                            flippedCard.SetFaceUp(false);
                        }
                    }
                }
            }

            // Restore score and moves
            score = lastMove.previousScore;
            moves = lastMove.previousMoves;

            // Refresh UI
            UpdateUI();

            return true;
        }

        /// <summary>
        /// Check if undo is available
        /// </summary>
        public bool CanUndo()
        {
            return moveHistory.Count > 0;
        }

        // ==========================================
        // React Native Communication
        // ==========================================

        /// <summary>
        /// Send current game state to React Native
        /// </summary>
        private void SendGameStateToReact()
        {
            if (reactBridge == null) return;

            GameStatePayload payload = new GameStatePayload
            {
                score = this.score,
                moves = this.moves,
                time = GetFormattedTime()
            };

            reactBridge.SendToReact("gameState", payload.ToJson());
        }

        /// <summary>
        /// Send game end event to React Native
        /// </summary>
        private void SendGameEndToReact(bool won, int finalScore, int gameTime)
        {
            if (reactBridge == null) return;

            GameEndPayload payload = new GameEndPayload
            {
                won = won,
                finalScore = finalScore,
                finalTime = FormatTime(gameTime)
            };

            reactBridge.SendToReact("gameEnd", payload.ToJson());
        }

        /// <summary>
        /// Receive actions from React Native
        /// Called by ReactBridge via BroadcastMessage
        /// </summary>
        private void OnReactMessage(string json)
        {
            try
            {
                ReactAction action = ReactAction.FromJson(json);

                Debug.Log($"[SolitaireGameManager] Received action: {action.action}");

                switch (action.action)
                {
                    case "newGame":
                        InitializeGame();
                        break;

                    case "restart":
                        RestartGame();
                        break;

                    case "undo":
                        UndoLastMove();
                        break;

                    default:
                        Debug.LogWarning($"[SolitaireGameManager] Unknown action: {action.action}");
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SolitaireGameManager] Failed to parse React message: {e.Message}");
            }
        }

        /// <summary>
        /// Format time as MM:SS
        /// </summary>
        private string GetFormattedTime()
        {
            int elapsed = Mathf.FloorToInt(Time.time - gameStartTime);
            return FormatTime(elapsed);
        }

        /// <summary>
        /// Format seconds as MM:SS
        /// </summary>
        private string FormatTime(int totalSeconds)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }
    }
}
