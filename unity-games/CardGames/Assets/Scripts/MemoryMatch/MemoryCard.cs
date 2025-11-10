using UnityEngine;

namespace MemoryMatch
{
    /// <summary>
    /// Individual card behavior for Memory Match game
    /// Simpler than Solitaire - just needs flip state and matching
    /// </summary>
    public class MemoryCard : MonoBehaviour
    {
        [Header("Card Properties")]
        public string suit;
        public string rank;
        public int pairId; // Cards with same pairId are a match

        [Header("Visual")]
        public SpriteRenderer spriteRenderer;
        public Sprite frontSprite;
        public Sprite backSprite;

        [Header("State")]
        public bool isFaceUp = false;
        public bool isMatched = false;
        public bool isFlipping = false;

        private MemoryMatchGameManager gameManager;
        private Vector3 originalScale;

        void Start()
        {
            originalScale = transform.localScale;
            gameManager = FindObjectOfType<MemoryMatchGameManager>();

            // Start face down
            ShowBack();
        }

        public void Initialize(string cardSuit, string cardRank, int cardPairId, Sprite front, Sprite back)
        {
            suit = cardSuit;
            rank = cardRank;
            pairId = cardPairId;
            frontSprite = front;
            backSprite = back;

            isFaceUp = false;
            isMatched = false;
            ShowBack();
        }

        void OnMouseDown()
        {
            if (gameManager != null && !isMatched && !isFlipping)
            {
                gameManager.OnCardClicked(this);
            }
        }

        /// <summary>
        /// Flip card to face up with animation
        /// </summary>
        public void FlipUp()
        {
            if (isFaceUp || isFlipping) return;

            StartCoroutine(FlipAnimation(true));
        }

        /// <summary>
        /// Flip card to face down with animation
        /// </summary>
        public void FlipDown()
        {
            if (!isFaceUp || isFlipping) return;

            StartCoroutine(FlipAnimation(false));
        }

        /// <summary>
        /// Animate card flip
        /// </summary>
        System.Collections.IEnumerator FlipAnimation(bool faceUp)
        {
            isFlipping = true;
            float duration = 0.3f;
            float elapsed = 0f;

            Vector3 startScale = transform.localScale;
            Vector3 midScale = new Vector3(0.1f, originalScale.y, originalScale.z);

            // Scale down to middle
            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2);
                transform.localScale = Vector3.Lerp(startScale, midScale, t);
                yield return null;
            }

            // Change sprite at halfway point
            isFaceUp = faceUp;
            spriteRenderer.sprite = faceUp ? frontSprite : backSprite;

            // Scale back up
            elapsed = 0f;
            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2);
                transform.localScale = Vector3.Lerp(midScale, originalScale, t);
                yield return null;
            }

            transform.localScale = originalScale;
            isFlipping = false;
        }

        /// <summary>
        /// Mark card as matched (stays face up)
        /// </summary>
        public void SetMatched()
        {
            isMatched = true;
            isFaceUp = true;

            // Optional: Fade or highlight matched cards
            Color color = spriteRenderer.color;
            color.a = 0.8f;
            spriteRenderer.color = color;
        }

        void ShowBack()
        {
            spriteRenderer.sprite = backSprite;
            isFaceUp = false;
        }

        void ShowFront()
        {
            spriteRenderer.sprite = frontSprite;
            isFaceUp = true;
        }

        public bool IsMatch(MemoryCard other)
        {
            return this.pairId == other.pairId && this != other;
        }
    }
}
