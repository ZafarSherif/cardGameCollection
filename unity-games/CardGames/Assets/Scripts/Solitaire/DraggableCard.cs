using UnityEngine;
using System.Collections.Generic;
using CardGames.Core;

namespace CardGames.Solitaire
{
    /// <summary>
    /// Handles card dragging and dropping for Solitaire
    /// </summary>
    [RequireComponent(typeof(Card))]
    [RequireComponent(typeof(Collider2D))]
    public class DraggableCard : MonoBehaviour
    {
        [Header("Drag Settings")]
        [SerializeField] private float dragZOffset = -2f; // Z position while dragging
        [SerializeField] private float pileDetectionRadius = 1f; // Radius for pile detection

        private Card card;
        private SolitaireGameManager gameManager;

        private bool isDragging = false;
        private Vector3 originalPosition;
        private Transform originalParent;
        private Pile originalPile;
        private List<Card> cardsBeingDragged;
        private Vector3 dragOffset; // Offset from card position to mouse position

        private Camera mainCamera;

        private void Awake()
        {
            card = GetComponent<Card>();
            mainCamera = Camera.main;
        }

        private void Start()
        {
            gameManager = FindObjectOfType<SolitaireGameManager>();
        }

        private void OnMouseDown()
        {
            Debug.Log($"[Card] OnMouseDown: {card.GetCardName()} | FaceUp: {card.IsFaceUp} | Pile: {card.CurrentPile?.Type} | Pile Index: {card.IndexInPile}");

            // Stock pile cards should forward clicks to the stock pile itself (even if face down)
            if (card.CurrentPile != null && card.CurrentPile.Type == Pile.PileType.Stock)
            {
                Debug.Log("[Card] On stock pile - forwarding click to ClickableStockPile");

                // Find and trigger the ClickableStockPile component
                ClickableStockPile stockPileClicker = card.CurrentPile.GetComponent<ClickableStockPile>();
                if (stockPileClicker != null)
                {
                    stockPileClicker.HandleClick();
                }
                else
                {
                    Debug.LogError("ClickableStockPile component not found on stock pile!");
                }
                return;
            }

            // Only allow dragging if card is face up
            if (!card.IsFaceUp)
            {
                Debug.Log($"✗ Card is face down - cannot drag {card.GetCardName()}");
                return;
            }

            // Waste pile: only allow dragging the TOP card
            if (card.CurrentPile != null && card.CurrentPile.Type == Pile.PileType.Waste)
            {
                Card topCard = card.CurrentPile.GetTopCard();
                if (card != topCard)
                {
                    Debug.Log("✗ Can only drag top card from waste pile");
                    return;
                }
            }

            // Colliders are now sized to only cover visible portions of cards,
            // so Unity's OnMouseDown naturally triggers on the correct card
            StartDragging();
        }

        private void OnMouseDrag()
        {
            if (isDragging)
            {
                Vector3 mousePosition = GetMouseWorldPosition();

                // Calculate target position using the offset (maintains grab position)
                Vector3 targetPosition = mousePosition - dragOffset;
                targetPosition.z = dragZOffset;

                // Move all cards being dragged
                if (cardsBeingDragged != null)
                {
                    Vector3 offset = targetPosition - transform.position;
                    foreach (Card dragCard in cardsBeingDragged)
                    {
                        dragCard.transform.position += offset;
                    }
                }
            }
        }

        private void OnMouseUp()
        {
            if (isDragging)
            {
                StopDragging();
            }
        }

        /// <summary>
        /// Start dragging this card and any cards on top of it
        /// </summary>
        private void StartDragging()
        {
            isDragging = true;
            originalPosition = transform.position;
            originalParent = transform.parent;
            originalPile = card.CurrentPile;

            // Calculate drag offset (difference between mouse position and card position)
            // This maintains the grab point relative to the card
            Vector3 mousePosition = GetMouseWorldPosition();
            dragOffset = mousePosition - transform.position;
            dragOffset.z = 0; // Ignore Z component
            Debug.Log($"[StartDrag] Drag offset: {dragOffset}");

            // Get all cards that should move with this one
            if (originalPile != null)
            {
                cardsBeingDragged = originalPile.GetMovableCards(card);
                Debug.Log($"[StartDrag] Got {cardsBeingDragged.Count} movable cards from {originalPile.Type}");

                // Log each card for debugging
                foreach (Card dragCard in cardsBeingDragged)
                {
                    Debug.Log($"  - {dragCard.GetCardName()} (FaceUp: {dragCard.IsFaceUp})");
                }
            }
            else
            {
                cardsBeingDragged = new List<Card> { card };
                Debug.Log($"[StartDrag] No pile, dragging single card: {card.GetCardName()}");
            }

            // Increase sorting order for cards being dragged
            int baseSortingOrder = 100; // High value to render on top
            for (int i = 0; i < cardsBeingDragged.Count; i++)
            {
                SetCardSortingOrder(cardsBeingDragged[i], baseSortingOrder + i);
            }
        }

        /// <summary>
        /// Stop dragging and try to place cards on a pile
        /// </summary>
        private void StopDragging()
        {
            isDragging = false;

            Debug.Log("Stop dragging - checking for target pile");

            // Find pile under mouse
            Pile targetPile = GetPileUnderMouse();

            bool validMove = false;

            if (targetPile != null && gameManager != null)
            {
                Debug.Log($"Attempting to move {cardsBeingDragged.Count} card(s) from {originalPile.Type} to {targetPile.Type}");

                // Try to move cards to the target pile
                validMove = gameManager.TryMoveCards(cardsBeingDragged, originalPile, targetPile);

                if (validMove)
                {
                    Debug.Log("✓ Move successful!");
                }
                else
                {
                    Debug.Log("✗ Move rejected by game manager");
                }
            }
            else
            {
                if (targetPile == null)
                {
                    Debug.Log("✗ No target pile found");
                }
                if (gameManager == null)
                {
                    Debug.LogError("✗ GameManager is null!");
                }
            }

            if (!validMove)
            {
                // Return cards to original position
                ReturnToOriginalPosition();
            }

            cardsBeingDragged = null;
        }

        /// <summary>
        /// Return cards to their original position with smooth animation
        /// </summary>
        private void ReturnToOriginalPosition()
        {
            Debug.Log("Returning cards to original position with animation");

            if (cardsBeingDragged != null && originalPile != null)
            {
                // Make a local copy since cardsBeingDragged will be set to null
                List<Card> cardsToAnimate = new List<Card>(cardsBeingDragged);
                Pile sourcePile = originalPile;

                // Start smooth return animation with local copies
                StartCoroutine(AnimateCardsBack(cardsToAnimate, sourcePile));
            }
        }

        /// <summary>
        /// Animate cards smoothly back to their original positions
        /// </summary>
        private System.Collections.IEnumerator AnimateCardsBack(List<Card> cardsToAnimate, Pile sourcePile)
        {
            // Safety check
            if (cardsToAnimate == null || cardsToAnimate.Count == 0 || sourcePile == null)
            {
                yield break;
            }

            // Store starting positions
            Dictionary<Card, Vector3> startPositions = new Dictionary<Card, Vector3>();
            foreach (Card dragCard in cardsToAnimate)
            {
                if (dragCard != null)
                {
                    startPositions[dragCard] = dragCard.transform.position;
                }
            }

            // Get target positions (refresh positions in pile)
            sourcePile.RefreshCardPositions();
            Dictionary<Card, Vector3> targetPositions = new Dictionary<Card, Vector3>();
            foreach (Card dragCard in cardsToAnimate)
            {
                if (dragCard != null)
                {
                    targetPositions[dragCard] = dragCard.transform.localPosition;
                }
            }

            // Animate over 0.3 seconds
            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Ease out cubic for nice feel
                t = 1f - Mathf.Pow(1f - t, 3f);

                foreach (Card dragCard in cardsToAnimate)
                {
                    if (dragCard != null && startPositions.ContainsKey(dragCard) && targetPositions.ContainsKey(dragCard))
                    {
                        Vector3 startPos = startPositions[dragCard];
                        Vector3 targetPos = targetPositions[dragCard];
                        // Convert target from local to world space
                        Vector3 worldTarget = sourcePile.transform.TransformPoint(targetPos);
                        dragCard.transform.position = Vector3.Lerp(startPos, worldTarget, t);
                    }
                }

                yield return null;
            }

            // Snap to final positions
            if (sourcePile != null)
            {
                sourcePile.RefreshCardPositions();
            }

            // Reset sorting order
            if (cardsToAnimate != null && sourcePile != null)
            {
                for (int i = 0; i < cardsToAnimate.Count; i++)
                {
                    Card dragCard = cardsToAnimate[i];
                    if (dragCard != null)
                    {
                        int indexInPile = sourcePile.Cards.IndexOf(dragCard);
                        if (indexInPile >= 0)
                        {
                            SetCardSortingOrder(dragCard, indexInPile);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the best target pile for dropping cards using rect-based overlap detection
        /// </summary>
        private Pile GetPileUnderMouse()
        {
            // Get the 2D rect of the card being dragged
            Rect cardRect = GetCardRect();

            // Find all piles that overlap with the card
            List<Pile> overlappingPiles = new List<Pile>();
            Pile[] allPiles = FindObjectsOfType<Pile>();

            foreach (Pile pile in allPiles)
            {
                // Skip the original pile
                if (pile == originalPile)
                    continue;

                // Get pile rect
                Rect pileRect = GetPileRect(pile);

                // Check if card rect overlaps with pile rect (2D overlap, ignoring Z)
                if (cardRect.Overlaps(pileRect))
                {
                    // Check if this would be a valid move
                    if (gameManager != null && gameManager.CanMoveCards(cardsBeingDragged, originalPile, pile))
                    {
                        overlappingPiles.Add(pile);
                        Debug.Log($"Card overlaps with {pile.Type} - valid move!");
                    }
                    else
                    {
                        Debug.Log($"Card overlaps with {pile.Type} - but not a valid move");
                    }
                }
            }

            // If we found overlapping valid piles, pick the closest one
            if (overlappingPiles.Count > 0)
            {
                Pile bestPile = GetClosestPile(overlappingPiles, new Vector3(cardRect.center.x, cardRect.center.y, 0));
                Debug.Log($"✓ Best target pile: {bestPile.Type} (from {overlappingPiles.Count} valid options)");
                return bestPile;
            }

            // Fallback: Check if mouse is over a pile (for edge cases)
            Vector3 mousePos = GetMouseWorldPosition();
            Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);

            foreach (Collider2D collider in colliders)
            {
                Pile pile = collider.GetComponent<Pile>();
                if (pile != null && pile != originalPile)
                {
                    Debug.Log($"Fallback: Found pile at mouse position: {pile.Type}");
                    return pile;
                }
            }

            Debug.Log("✗ No valid target pile found");
            return null;
        }

        /// <summary>
        /// Get the 2D rect of the card being dragged (X and Y only, ignoring Z)
        /// </summary>
        private Rect GetCardRect()
        {
            // Use the card's sprite renderer to get accurate bounds
            SpriteRenderer renderer = card.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = card.GetComponentInChildren<SpriteRenderer>();
            }

            if (renderer != null)
            {
                Bounds bounds = renderer.bounds;
                return new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
            }

            // Fallback: use collider bounds
            Collider2D collider = card.GetComponent<Collider2D>();
            if (collider != null)
            {
                Bounds bounds = collider.bounds;
                return new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
            }

            // Last resort: create rect from position with standard card size
            Vector3 pos = card.transform.position;
            return new Rect(pos.x - 0.315f, pos.y - 0.44f, 0.63f, 0.88f);
        }

        /// <summary>
        /// Get the 2D rect of a pile (including all cards in it, X and Y only, ignoring Z)
        /// </summary>
        private Rect GetPileRect(Pile pile)
        {
            // Start with the pile's own collider bounds
            Collider2D pileCollider = pile.GetComponent<Collider2D>();

            Rect rect;
            if (pileCollider != null)
            {
                Bounds bounds = pileCollider.bounds;
                rect = new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
            }
            else
            {
                // Fallback: use pile position with standard size
                Vector3 pos = pile.transform.position;
                rect = new Rect(pos.x - 0.315f, pos.y - 0.44f, 0.63f, 0.88f);
            }

            // Expand to include all cards in the pile
            foreach (Card pileCard in pile.Cards)
            {
                SpriteRenderer renderer = pileCard.GetComponent<SpriteRenderer>();
                if (renderer != null && renderer.enabled)
                {
                    Bounds cardBounds = renderer.bounds;
                    Rect cardRect = new Rect(cardBounds.min.x, cardBounds.min.y, cardBounds.size.x, cardBounds.size.y);

                    // Expand rect to include this card
                    float minX = Mathf.Min(rect.xMin, cardRect.xMin);
                    float minY = Mathf.Min(rect.yMin, cardRect.yMin);
                    float maxX = Mathf.Max(rect.xMax, cardRect.xMax);
                    float maxY = Mathf.Max(rect.yMax, cardRect.yMax);
                    rect = new Rect(minX, minY, maxX - minX, maxY - minY);
                }
            }

            return rect;
        }

        /// <summary>
        /// Find the closest pile from a list of piles
        /// </summary>
        private Pile GetClosestPile(List<Pile> piles, Vector3 cardPosition)
        {
            if (piles.Count == 0)
                return null;

            if (piles.Count == 1)
                return piles[0];

            // Find pile with center closest to card center
            Pile closest = piles[0];
            float closestDistance = Vector3.Distance(cardPosition, piles[0].transform.position);

            for (int i = 1; i < piles.Count; i++)
            {
                float distance = Vector3.Distance(cardPosition, piles[i].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = piles[i];
                }
            }

            return closest;
        }

        /// <summary>
        /// Get mouse position in world space
        /// </summary>
        private Vector3 GetMouseWorldPosition()
        {
            if (mainCamera == null) return Vector3.zero;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
            return mainCamera.ScreenToWorldPoint(mousePos);
        }

        /// <summary>
        /// Set sorting order for a card's renderers
        /// </summary>
        private void SetCardSortingOrder(Card targetCard, int order)
        {
            SpriteRenderer[] renderers = targetCard.GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.sortingOrder = order;
            }
        }

        /// <summary>
        /// Double-click to auto-move card to foundation
        /// </summary>
        private float lastClickTime = 0f;
        private float doubleClickThreshold = 0.3f;

        private void OnMouseUpAsButton()
        {
            if (Time.time - lastClickTime < doubleClickThreshold)
            {
                // Double click detected
                TryAutoMoveToFoundation();
            }

            lastClickTime = Time.time;
        }

        /// <summary>
        /// Try to automatically move this card to a foundation pile
        /// </summary>
        private void TryAutoMoveToFoundation()
        {
            if (gameManager != null)
            {
                gameManager.TryAutoMoveToFoundation(card);
            }
        }
    }
}
