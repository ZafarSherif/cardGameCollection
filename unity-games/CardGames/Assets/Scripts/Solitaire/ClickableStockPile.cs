using UnityEngine;
using CardGames.Core;

namespace CardGames.Solitaire
{
    /// <summary>
    /// Handles clicking on the stock pile to draw cards
    /// Add this component to the Stock Pile GameObject
    /// </summary>
    [RequireComponent(typeof(Pile))]
    [RequireComponent(typeof(Collider2D))]
    public class ClickableStockPile : MonoBehaviour
    {
        private Pile pile;
        private SolitaireGameManager gameManager;

        private void Awake()
        {
            pile = GetComponent<Pile>();
        }

        private void Start()
        {
            gameManager = FindObjectOfType<SolitaireGameManager>();

            // Verify this is actually the stock pile
            if (pile.Type != Pile.PileType.Stock)
            {
                Debug.LogWarning($"ClickableStockPile component added to {pile.Type} pile. Should only be on Stock pile!");
            }
        }

        private void OnMouseDown()
        {
            HandleClick();
        }

        /// <summary>
        /// Public method to handle click - can be called by cards on the pile
        /// </summary>
        public void HandleClick()
        {
            Debug.Log("[StockPile] HandleClick - Stock pile clicked!");

            if (gameManager != null)
            {
                Debug.Log($"[StockPile] Calling DrawFromStock() - Stock has {pile.CardCount} cards");
                gameManager.DrawFromStock();
            }
            else
            {
                Debug.LogError("[StockPile] SolitaireGameManager not found! Cannot draw cards.");
            }
        }

        private void OnMouseEnter()
        {
            Debug.Log("[StockPile] Mouse entered stock pile area");
            // Could add highlight effect here
            // For example, change sprite color slightly
        }

        private void OnMouseExit()
        {
            // Could remove highlight effect here
        }
    }
}
