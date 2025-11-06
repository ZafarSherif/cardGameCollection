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

            StartDragging();
        }

        private void OnMouseDrag()
        {
            if (isDragging)
            {
                Vector3 mousePosition = GetMouseWorldPosition();
                mousePosition.z = dragZOffset;

                // Move all cards being dragged
                if (cardsBeingDragged != null)
                {
                    Vector3 offset = mousePosition - transform.position;
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
        /// Return cards to their original position
        /// </summary>
        private void ReturnToOriginalPosition()
        {
            Debug.Log("Returning cards to original position");

            if (cardsBeingDragged != null && originalPile != null)
            {
                // Cards are still logically in the original pile
                // Just need to refresh their visual positions
                originalPile.RefreshCardPositions();

                // Reset sorting order
                for (int i = 0; i < cardsBeingDragged.Count; i++)
                {
                    Card dragCard = cardsBeingDragged[i];
                    int indexInPile = originalPile.Cards.IndexOf(dragCard);
                    if (indexInPile >= 0)
                    {
                        SetCardSortingOrder(dragCard, indexInPile);
                    }
                }
            }
        }

        /// <summary>
        /// Get the pile under the mouse cursor
        /// </summary>
        private Pile GetPileUnderMouse()
        {
            Vector3 mousePos = GetMouseWorldPosition();

            // Method 1: Check if dropping on a card (cards are easier to hit than piles)
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null)
            {
                // First check if we hit a card
                Card hitCard = hit.collider.GetComponent<Card>();
                if (hitCard != null && hitCard.CurrentPile != null)
                {
                    // IMPORTANT: Don't count cards we're currently dragging!
                    if (cardsBeingDragged != null && cardsBeingDragged.Contains(hitCard))
                    {
                        Debug.Log($"Hit card we're dragging ({hitCard.GetCardName()}), ignoring...");
                    }
                    else
                    {
                        Debug.Log($"Found card: {hitCard.GetCardName()} → using its pile: {hitCard.CurrentPile.Type}");
                        return hitCard.CurrentPile;
                    }
                }
                else
                {
                    // Check if we hit a pile directly
                    Pile pile = hit.collider.GetComponent<Pile>();
                    if (pile != null)
                    {
                        Debug.Log($"Found pile via raycast: {pile.Type}");
                        return pile;
                    }
                }
            }

            // Method 2: Try OverlapPoint (check all colliders at position)
            Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);

            // First pass: look for cards (but not the ones we're dragging)
            foreach (Collider2D collider in colliders)
            {
                Card hitCard = collider.GetComponent<Card>();
                if (hitCard != null && hitCard.CurrentPile != null)
                {
                    // Skip cards we're currently dragging
                    if (cardsBeingDragged != null && cardsBeingDragged.Contains(hitCard))
                    {
                        continue;
                    }

                    Debug.Log($"Found card via overlap: {hitCard.GetCardName()} → using its pile: {hitCard.CurrentPile.Type}");
                    return hitCard.CurrentPile;
                }
            }

            // Second pass: look for piles
            foreach (Collider2D collider in colliders)
            {
                Pile pile = collider.GetComponent<Pile>();
                if (pile != null)
                {
                    Debug.Log($"Found pile via overlap: {pile.Type}");
                    return pile;
                }
            }

            // Method 3: Find closest pile (fallback)
            Pile closestPile = null;
            float closestDistance = pileDetectionRadius;

            Pile[] allPiles = FindObjectsOfType<Pile>();
            foreach (Pile pile in allPiles)
            {
                float distance = Vector3.Distance(mousePos, pile.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPile = pile;
                }
            }

            if (closestPile != null)
            {
                Debug.Log($"Found pile via proximity: {closestPile.Type}");
            }
            else
            {
                Debug.Log("No pile found at drop position");
            }

            return closestPile;
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
