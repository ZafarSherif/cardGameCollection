using System.Collections.Generic;
using UnityEngine;

namespace CardGames.Core
{
    /// <summary>
    /// Manages card sprites - loads and caches them for performance
    /// Singleton pattern for easy access
    /// </summary>
    public class CardSpriteManager : MonoBehaviour
    {
        private static CardSpriteManager instance;
        public static CardSpriteManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("CardSpriteManager");
                    instance = go.AddComponent<CardSpriteManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        [Header("Card Sprites")]
        [SerializeField] private Sprite cardBackSprite;
        [SerializeField] private Sprite emptyPileSprite;

        [Header("Loading Method")]
        [SerializeField] private LoadMethod loadMethod = LoadMethod.Resources;

        public enum LoadMethod
        {
            Resources,      // Load from Resources folder at runtime
            Serialized,     // Manually assigned in Inspector
            Addressables    // Use Unity Addressables (advanced)
        }

        // Sprite cache for performance
        private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();

        // For serialized method: manually assign sprites in Inspector
        [System.Serializable]
        public class CardSpriteData
        {
            public Card.Suit suit;
            public Card.Rank rank;
            public Sprite sprite;
        }

        [Header("Serialized Sprites (if using Serialized method)")]
        [SerializeField] private List<CardSpriteData> cardSprites = new List<CardSpriteData>();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // Build sprite cache if using serialized method
            if (loadMethod == LoadMethod.Serialized)
            {
                BuildSpriteCache();
            }
        }

        /// <summary>
        /// Get sprite for a specific card
        /// </summary>
        public Sprite GetCardSprite(Card.Suit suit, Card.Rank rank)
        {
            string key = GetSpriteKey(suit, rank);

            // Check cache first
            if (spriteCache.ContainsKey(key))
            {
                return spriteCache[key];
            }

            // Load sprite based on method
            Sprite sprite = null;

            switch (loadMethod)
            {
                case LoadMethod.Resources:
                    sprite = LoadFromResources(suit, rank);
                    break;

                case LoadMethod.Serialized:
                    sprite = LoadFromSerialized(suit, rank);
                    break;

                case LoadMethod.Addressables:
                    // TODO: Implement Addressables loading
                    Debug.LogWarning("Addressables loading not implemented yet");
                    break;
            }

            // Cache the sprite
            if (sprite != null)
            {
                spriteCache[key] = sprite;
            }
            else
            {
                Debug.LogError($"Failed to load sprite for {suit} {rank}");
            }

            return sprite;
        }

        /// <summary>
        /// Get card back sprite
        /// </summary>
        public Sprite GetCardBackSprite()
        {
            if (cardBackSprite == null)
            {
                // Try to load from Resources
                cardBackSprite = Resources.Load<Sprite>("Cards/Backs/card_back_blue");

                if (cardBackSprite == null)
                {
                    Debug.LogWarning("Card back sprite not found! Using placeholder.");
                }
            }

            return cardBackSprite;
        }

        /// <summary>
        /// Get empty pile sprite
        /// </summary>
        public Sprite GetEmptyPileSprite()
        {
            return emptyPileSprite;
        }

        /// <summary>
        /// Load sprite from Resources folder
        /// Expected path: Resources/Cards/Faces/{suit}_{rank}
        /// </summary>
        private Sprite LoadFromResources(Card.Suit suit, Card.Rank rank)
        {
            string suitName = suit.ToString().ToLower();
            string rankName = GetRankName(rank).ToLower();
            string path = $"Cards/Faces/{suitName}_{rankName}";

            Sprite sprite = Resources.Load<Sprite>(path);

            if (sprite == null)
            {
                Debug.LogWarning($"Sprite not found at: Resources/{path}");
            }

            return sprite;
        }

        /// <summary>
        /// Load sprite from serialized list
        /// </summary>
        private Sprite LoadFromSerialized(Card.Suit suit, Card.Rank rank)
        {
            foreach (var data in cardSprites)
            {
                if (data.suit == suit && data.rank == rank)
                {
                    return data.sprite;
                }
            }

            Debug.LogWarning($"No serialized sprite found for {suit} {rank}");
            return null;
        }

        /// <summary>
        /// Build cache from serialized sprites
        /// </summary>
        private void BuildSpriteCache()
        {
            spriteCache.Clear();

            foreach (var data in cardSprites)
            {
                string key = GetSpriteKey(data.suit, data.rank);
                spriteCache[key] = data.sprite;
            }

            Debug.Log($"Built sprite cache with {spriteCache.Count} sprites");
        }

        /// <summary>
        /// Get cache key for a card
        /// </summary>
        private string GetSpriteKey(Card.Suit suit, Card.Rank rank)
        {
            return $"{suit}_{rank}";
        }

        /// <summary>
        /// Get rank name as string
        /// </summary>
        private string GetRankName(Card.Rank rank)
        {
            switch (rank)
            {
                case Card.Rank.Ace: return "ace";
                case Card.Rank.Two: return "2";
                case Card.Rank.Three: return "3";
                case Card.Rank.Four: return "4";
                case Card.Rank.Five: return "5";
                case Card.Rank.Six: return "6";
                case Card.Rank.Seven: return "7";
                case Card.Rank.Eight: return "8";
                case Card.Rank.Nine: return "9";
                case Card.Rank.Ten: return "10";
                case Card.Rank.Jack: return "jack";
                case Card.Rank.Queen: return "queen";
                case Card.Rank.King: return "king";
                default: return "unknown";
            }
        }

        /// <summary>
        /// Preload all card sprites (optional, for performance)
        /// </summary>
        public void PreloadAllSprites()
        {
            Debug.Log("Preloading all card sprites...");

            foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
            {
                foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank)))
                {
                    GetCardSprite(suit, rank);
                }
            }

            Debug.Log($"Preloaded {spriteCache.Count} card sprites");
        }

        /// <summary>
        /// Clear sprite cache (useful for memory management)
        /// </summary>
        public void ClearCache()
        {
            spriteCache.Clear();
            Debug.Log("Sprite cache cleared");
        }

        #region Editor Helpers

#if UNITY_EDITOR
        /// <summary>
        /// Auto-populate serialized sprites from Project assets
        /// (Can be called from custom editor or context menu)
        /// </summary>
        [ContextMenu("Auto-Populate Serialized Sprites")]
        private void AutoPopulateSerializedSprites()
        {
            cardSprites.Clear();

            // Search for sprites in Assets/Art/Cards/Faces
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Art/Cards/Faces" });

            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);

                if (sprite != null)
                {
                    // Parse suit and rank from filename
                    // Expected format: clubs_ace.png, hearts_10.png, etc.
                    string filename = System.IO.Path.GetFileNameWithoutExtension(path);
                    string[] parts = filename.Split('_');

                    if (parts.Length == 2)
                    {
                        if (TryParseSuit(parts[0], out Card.Suit suit) &&
                            TryParseRank(parts[1], out Card.Rank rank))
                        {
                            cardSprites.Add(new CardSpriteData
                            {
                                suit = suit,
                                rank = rank,
                                sprite = sprite
                            });
                        }
                    }
                }
            }

            Debug.Log($"Auto-populated {cardSprites.Count} card sprites");
            UnityEditor.EditorUtility.SetDirty(this);
        }

        private bool TryParseSuit(string suitStr, out Card.Suit suit)
        {
            return System.Enum.TryParse(suitStr, true, out suit);
        }

        private bool TryParseRank(string rankStr, out Card.Rank rank)
        {
            // Handle number ranks
            if (int.TryParse(rankStr, out int rankNum))
            {
                switch (rankNum)
                {
                    case 2: rank = Card.Rank.Two; return true;
                    case 3: rank = Card.Rank.Three; return true;
                    case 4: rank = Card.Rank.Four; return true;
                    case 5: rank = Card.Rank.Five; return true;
                    case 6: rank = Card.Rank.Six; return true;
                    case 7: rank = Card.Rank.Seven; return true;
                    case 8: rank = Card.Rank.Eight; return true;
                    case 9: rank = Card.Rank.Nine; return true;
                    case 10: rank = Card.Rank.Ten; return true;
                }
            }

            // Handle face cards and ace
            return System.Enum.TryParse(rankStr, true, out rank);
        }
#endif

        #endregion
    }
}
