using System.Collections.Generic;
using UnityEngine;

namespace CardGames.Core
{
    /// <summary>
    /// Represents a pile of cards (stock, waste, tableau, or foundation)
    /// </summary>
    public class Pile : MonoBehaviour
    {
        public enum PileType
        {
            Stock,      // Draw pile
            Waste,      // Discard pile (after drawing from stock)
            Tableau,    // Main playing area (7 piles)
            Foundation  // Goal piles (4 piles, one per suit)
        }

        [Header("Pile Settings")]
        [SerializeField] private PileType type;
        [SerializeField] private int pileIndex; // For tableau (0-6) and foundation (0-3)

        [Header("Card Layout")]
        [SerializeField] private float cardSpacing = 0.3f; // Vertical spacing for tableau
        [SerializeField] private float faceDownSpacing = 0.2f;
        [SerializeField] private bool fanCards = true; // For tableau piles

        // Card storage
        private List<Card> cards = new List<Card>();

        // Properties
        public PileType Type => type;
        public int PileIndex => pileIndex;
        public int CardCount => cards.Count;
        public List<Card> Cards => cards;

        /// <summary>
        /// Get the top card in the pile
        /// </summary>
        public Card GetTopCard()
        {
            return cards.Count > 0 ? cards[cards.Count - 1] : null;
        }

        /// <summary>
        /// Check if pile is empty
        /// </summary>
        public bool IsEmpty()
        {
            return cards.Count == 0;
        }

        /// <summary>
        /// Check if a card can be added to this pile
        /// </summary>
        public bool CanAcceptCard(Card card)
        {
            switch (type)
            {
                case PileType.Stock:
                    return false; // Can't add to stock manually

                case PileType.Waste:
                    return false; // Waste only accepts from stock

                case PileType.Tableau:
                    return CanAcceptToTableau(card);

                case PileType.Foundation:
                    return CanAcceptToFoundation(card);

                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if card can be placed on tableau pile
        /// </summary>
        private bool CanAcceptToTableau(Card card)
        {
            // Empty tableau can only accept King
            if (IsEmpty())
                return card.CardRank == Card.Rank.King;

            Card topCard = GetTopCard();

            // Top card must be face up
            if (!topCard.IsFaceUp)
                return false;

            // Card must be able to stack on top card
            return card.CanStackOn(topCard);
        }

        /// <summary>
        /// Check if card can be placed on foundation pile
        /// </summary>
        private bool CanAcceptToFoundation(Card card)
        {
            Card topCard = GetTopCard();
            return card.CanPlaceOnFoundation(topCard);
        }

        /// <summary>
        /// Add a card to the pile
        /// </summary>
        public void AddCard(Card card)
        {
            if (card == null) return;

            cards.Add(card);
            card.SetPile(this, cards.Count - 1);
            card.transform.SetParent(transform);

            UpdateCardPositions();
        }

        /// <summary>
        /// Add multiple cards to the pile
        /// </summary>
        public void AddCards(List<Card> cardsToAdd)
        {
            foreach (Card card in cardsToAdd)
            {
                AddCard(card);
            }
        }

        /// <summary>
        /// Remove the top card from the pile
        /// </summary>
        public Card RemoveTopCard()
        {
            if (IsEmpty()) return null;

            Card card = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);

            UpdateCardPositions();
            return card;
        }

        /// <summary>
        /// Remove multiple cards starting from a specific card
        /// </summary>
        public List<Card> RemoveCardsFrom(Card startCard)
        {
            List<Card> removedCards = new List<Card>();
            int startIndex = cards.IndexOf(startCard);

            if (startIndex < 0) return removedCards;

            // Remove all cards from startCard to the end
            for (int i = cards.Count - 1; i >= startIndex; i--)
            {
                removedCards.Insert(0, cards[i]);
                cards.RemoveAt(i);
            }

            UpdateCardPositions();
            return removedCards;
        }

        /// <summary>
        /// Flip the top card face up
        /// </summary>
        public void FlipTopCard()
        {
            Card topCard = GetTopCard();
            if (topCard != null && !topCard.IsFaceUp)
            {
                topCard.SetFaceUp(true);
            }
        }

        /// <summary>
        /// Update visual positions of all cards in the pile
        /// </summary>
        private void UpdateCardPositions()
        {
            int faceDownCount = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                Card card = cards[i];
                if (!card.IsFaceUp)
                    faceDownCount++;
                Vector3 position = GetCardPosition(i, faceDownCount, card.IsFaceUp);

                // Smooth movement (can be replaced with animation)
                card.transform.localPosition = position;

                // Set sorting order (higher cards render on top)
                SpriteRenderer[] renderers = card.GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in renderers)
                {
                    renderer.sortingOrder = i;
                }

                // Update collider state (only top cards in waste/foundation should be clickable)
                card.UpdateCollider();
            }

            // Update pile's own collider (only enabled when empty)
            UpdatePileCollider();
        }

        /// <summary>
        /// Update pile collider - only enabled when pile is empty
        /// This prevents pile colliders from blocking card clicks
        /// </summary>
        private void UpdatePileCollider()
        {
            Collider2D pileCollider = GetComponent<Collider2D>();
            if (pileCollider != null)
            {
                // Stock pile always needs collider enabled (for dealing cards)
                if (type == PileType.Stock)
                {
                    pileCollider.enabled = true;
                    return;
                }

                // For other piles: only enable when there are no cards
                // This makes empty piles clickable for Kings/Aces
                pileCollider.enabled = (cards.Count == 0);
            }
        }

        /// <summary>
        /// Calculate position for a card at a specific index
        /// </summary>
        private Vector3 GetCardPosition(int index, int faceDownCount, bool isFaceUp)
        {
            Vector3 position = Vector3.zero;

            switch (type)
            {
                case PileType.Stock:
                case PileType.Waste:
                case PileType.Foundation:
                    // Cards stack on top of each other (no fanning)
                    position.y = index * 0.01f; // Slight offset for depth
                    break;

                case PileType.Tableau:
                    // Fan cards vertically
                    if (fanCards)
                    {
                        if (isFaceUp)
                            position.y = -faceDownCount * faceDownSpacing - (index - faceDownCount) * cardSpacing;
                        else
                            position.y = -index * faceDownSpacing;
                    }
                    break;
            }

            return position;
        }

        /// <summary>
        /// Get all face-up cards starting from a specific card
        /// </summary>
        public List<Card> GetMovableCards(Card startCard)
        {
            List<Card> movableCards = new List<Card>();
            int startIndex = cards.IndexOf(startCard);

            if (startIndex < 0) return movableCards;

            // Can only move if startCard is face up
            if (!startCard.IsFaceUp) return movableCards;

            // Get all cards from startCard to the end (if all are face up)
            for (int i = startIndex; i < cards.Count; i++)
            {
                if (!cards[i].IsFaceUp)
                    break; // Stop if we hit a face-down card

                movableCards.Add(cards[i]);
            }

            return movableCards;
        }

        /// <summary>
        /// Clear all cards from the pile
        /// </summary>
        public void Clear()
        {
            cards.Clear();
        }

        /// <summary>
        /// Refresh card positions manually (useful after drag operations)
        /// </summary>
        public void RefreshCardPositions()
        {
            UpdateCardPositions();
        }

        /// <summary>
        /// Debug info
        /// </summary>
        public string GetDebugInfo()
        {
            return $"{type} Pile {pileIndex}: {cards.Count} cards";
        }
    }
}
