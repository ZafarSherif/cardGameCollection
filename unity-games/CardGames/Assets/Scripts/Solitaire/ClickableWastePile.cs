using UnityEngine;
using CardGames.Core;

namespace CardGames.Solitaire
{
    /// <summary>
    /// Handles clicking on the waste pile when stock is empty
    /// This recycles the waste back to stock
    /// NOTE: This script is OPTIONAL and currently doesn't do anything.
    /// It's here for future functionality if needed.
    /// REMOVE THIS COMPONENT FROM WASTE PILE - it can block card clicks!
    /// </summary>
    [RequireComponent(typeof(Pile))]
    public class ClickableWastePile : MonoBehaviour
    {
        private Pile pile;
        private SolitaireGameManager gameManager;

        private void Awake()
        {
            pile = GetComponent<Pile>();

            // Disable this component by default - it's not needed
            enabled = false;
        }

        private void Start()
        {
            gameManager = FindObjectOfType<SolitaireGameManager>();

            // Verify this is actually the waste pile
            if (pile.Type != Pile.PileType.Waste)
            {
                Debug.LogWarning($"ClickableWastePile component added to {pile.Type} pile. Should only be on Waste pile!");
            }

            Debug.LogWarning("ClickableWastePile is not needed! Remove this component from Waste Pile.");
        }

        private void OnMouseDown()
        {
            // This component is disabled by default
            // It's not needed for normal Solitaire gameplay
            Debug.Log("Waste pile area clicked - but this shouldn't happen as component is disabled");
        }
    }
}
