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

        // Events for UI updates
        public System.Action<int> OnScoreChanged;
        public System.Action<int> OnMovesChanged;
        public System.Action<bool, int, int> OnGameEnd; // won, score, time

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
            InitializeGame();
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

            OnScoreChanged?.Invoke(score);
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

            OnGameEnd?.Invoke(true, score, gameTime);
        }

        /// <summary>
        /// Update UI elements
        /// </summary>
        private void UpdateUI()
        {
            OnScoreChanged?.Invoke(score);
            OnMovesChanged?.Invoke(moves);
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
    }
}
