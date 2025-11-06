using UnityEngine;

namespace CardGames.Core
{
    /// <summary>
    /// Represents a playing card with suit, rank, and state
    /// </summary>
    public class Card : MonoBehaviour
    {
        public enum Suit
        {
            Spades = 0,
            Hearts = 1,
            Diamonds = 2,
            Clubs = 3
        }

        public enum Rank
        {
            Ace = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10,
            Jack = 11,
            Queen = 12,
            King = 13
        }

        [Header("Card Properties")]
        [SerializeField] private Suit suit;
        [SerializeField] private Rank rank;
        [SerializeField] private bool isFaceUp = false;

        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer cardFrontRenderer;
        [SerializeField] private SpriteRenderer cardBackRenderer;
        [SerializeField] private Sprite frontSprite;
        [SerializeField] private Sprite backSprite;

        // References
        private Pile currentPile;
        private int indexInPile;

        // Properties
        public Suit CardSuit => suit;
        public Rank CardRank => rank;
        public bool IsFaceUp => isFaceUp;
        public Pile CurrentPile => currentPile;
        public int IndexInPile => indexInPile;

        /// <summary>
        /// Get the color of the card (Red or Black)
        /// </summary>
        public CardColor Color
        {
            get
            {
                return (suit == Suit.Hearts || suit == Suit.Diamonds)
                    ? CardColor.Red
                    : CardColor.Black;
            }
        }

        /// <summary>
        /// Initialize card with suit and rank
        /// </summary>
        public void Initialize(Suit cardSuit, Rank cardRank)
        {
            suit = cardSuit;
            rank = cardRank;

            // Load sprites from CardSpriteManager
            LoadSprites();

            UpdateVisuals();
        }

        /// <summary>
        /// Load sprites using CardSpriteManager
        /// </summary>
        private void LoadSprites()
        {
            // Get front sprite for this card
            frontSprite = CardSpriteManager.Instance.GetCardSprite(suit, rank);

            // Get card back sprite
            backSprite = CardSpriteManager.Instance.GetCardBackSprite();

            // Apply sprites to renderers
            if (cardFrontRenderer != null && frontSprite != null)
            {
                cardFrontRenderer.sprite = frontSprite;
                // Debug.Log($"Card front sprite set: {rank} of {suit}");
            }
            else if (cardFrontRenderer != null)
            {
                Debug.LogWarning($"No sprite found for {rank} of {suit}");
            }

            if (cardBackRenderer != null && backSprite != null)
            {
                cardBackRenderer.sprite = backSprite;
            }
        }

        /// <summary>
        /// Flip the card face up or face down
        /// </summary>
        public void Flip()
        {
            isFaceUp = !isFaceUp;
            UpdateVisuals();
        }

        /// <summary>
        /// Set the card face up or face down
        /// </summary>
        public void SetFaceUp(bool faceUp)
        {
            bool wasFlipped = (isFaceUp != faceUp);
            isFaceUp = faceUp;
            UpdateVisuals();

            // Play flip sound when card is flipped face up
            if (wasFlipped && faceUp && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCardFlip();
            }
        }

        /// <summary>
        /// Update card visuals based on face up state
        /// </summary>
        private void UpdateVisuals()
        {
            if (cardFrontRenderer != null)
                cardFrontRenderer.enabled = isFaceUp;
            if (cardBackRenderer != null)
                cardBackRenderer.enabled = !isFaceUp;

            UpdateCollider();
        }

        /// <summary>
        /// Update collider enabled state based on if card is playable
        /// </summary>
        public void UpdateCollider()
        {
            Collider2D collider = GetComponent<Collider2D>();
            if (collider == null) return;

            // Face-down cards never have colliders
            if (!isFaceUp)
            {
                collider.enabled = false;
                return;
            }

            // Stock pile cards forward clicks, so enable collider
            if (currentPile != null && currentPile.Type == Pile.PileType.Stock)
            {
                collider.enabled = true;
                return;
            }

            // Waste pile: only TOP card should have collider enabled
            if (currentPile != null && currentPile.Type == Pile.PileType.Waste)
            {
                Card topCard = currentPile.GetTopCard();
                collider.enabled = (this == topCard);
                return;
            }

            // Tableau: ANY face-up card should have collider
            // (This allows clicking anywhere in a sequence to drag it)
            if (currentPile != null && currentPile.Type == Pile.PileType.Tableau)
            {
                collider.enabled = isFaceUp;
                return;
            }

            // Foundation: top card should have collider
            if (currentPile != null && currentPile.Type == Pile.PileType.Foundation)
            {
                Card topCard = currentPile.GetTopCard();
                collider.enabled = (this == topCard);
                return;
            }

            // Default: face-up cards have collider enabled
            collider.enabled = isFaceUp;
        }

        /// <summary>
        /// Check if this card can be stacked on another card (for tableau)
        /// Different color and one rank lower
        /// </summary>
        public bool CanStackOn(Card otherCard)
        {
            if (otherCard == null) return false;

            // Must be opposite color
            if (Color == otherCard.Color) return false;

            // Must be one rank lower (e.g., 6 can go on 7)
            return (int)rank == (int)otherCard.rank - 1;
        }

        /// <summary>
        /// Check if this card can be placed on foundation
        /// Same suit and one rank higher than current foundation card
        /// </summary>
        public bool CanPlaceOnFoundation(Card foundationCard)
        {
            // Ace can go on empty foundation
            if (foundationCard == null)
                return rank == Rank.Ace;

            // Must be same suit
            if (suit != foundationCard.suit) return false;

            // Must be one rank higher
            return (int)rank == (int)foundationCard.rank + 1;
        }

        /// <summary>
        /// Set the pile this card belongs to
        /// </summary>
        public void SetPile(Pile pile, int index)
        {
            currentPile = pile;
            indexInPile = index;
        }

        /// <summary>
        /// Get the rank value (Ace = 1, King = 13)
        /// </summary>
        public int GetRankValue()
        {
            return (int)rank;
        }

        /// <summary>
        /// Get card name for debugging
        /// </summary>
        public string GetCardName()
        {
            return $"{rank} of {suit}";
        }

        public override string ToString()
        {
            return GetCardName();
        }
    }

    public enum CardColor
    {
        Red,
        Black
    }
}
