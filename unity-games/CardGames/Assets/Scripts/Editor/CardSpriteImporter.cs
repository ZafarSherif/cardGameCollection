using UnityEngine;
using UnityEditor;

namespace CardGames.Editor
{
    /// <summary>
    /// Automatically configures import settings for card sprites
    /// This runs whenever sprites are imported into the Cards folder
    /// </summary>
    public class CardSpriteImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            // Check if this asset is in the Cards folder
            if (assetPath.Contains("Resources/Cards"))
            {
                TextureImporter importer = (TextureImporter)assetImporter;

                // Configure as Sprite
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 100;
                importer.filterMode = FilterMode.Bilinear;
                importer.mipmapEnabled = false;

                // Quality settings
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 512;

                // Alpha settings
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.alphaIsTransparency = true;

                Debug.Log($"[CardSpriteImporter] Configured sprite: {assetPath}");
            }
        }

        /// <summary>
        /// Menu item to manually reimport all card sprites
        /// </summary>
        [MenuItem("CardGames/Reimport All Card Sprites")]
        static void ReimportCardSprites()
        {
            string cardsPath = "Assets/Resources/Cards";

            if (!AssetDatabase.IsValidFolder(cardsPath))
            {
                Debug.LogWarning($"Cards folder not found at: {cardsPath}");
                return;
            }

            // Find all PNG files in Cards folder
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { cardsPath });

            int count = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                count++;
            }

            AssetDatabase.Refresh();
            Debug.Log($"[CardSpriteImporter] Reimported {count} card sprites");
        }

        /// <summary>
        /// Menu item to verify all card sprites are present
        /// </summary>
        [MenuItem("CardGames/Verify Card Sprites")]
        static void VerifyCardSprites()
        {
            string facesPath = "Assets/Resources/Cards/Faces";
            string backsPath = "Assets/Resources/Cards/Backs";

            int facesCount = 0;
            int backsCount = 0;
            int missingCount = 0;

            // Expected cards
            string[] suits = { "clubs", "diamonds", "hearts", "spades" };
            string[] ranks = { "ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "jack", "queen", "king" };

            Debug.Log("===== Card Sprite Verification =====");

            // Check all 52 cards
            foreach (string suit in suits)
            {
                foreach (string rank in ranks)
                {
                    string spritePath = $"{facesPath}/{suit}_{rank}";
                    Sprite sprite = Resources.Load<Sprite>($"Cards/Faces/{suit}_{rank}");

                    if (sprite != null)
                    {
                        facesCount++;
                    }
                    else
                    {
                        Debug.LogWarning($"Missing: {suit}_{rank}");
                        missingCount++;
                    }
                }
            }

            // Check card back
            Sprite cardBack = Resources.Load<Sprite>("Cards/Backs/card_back_blue");
            if (cardBack != null)
            {
                backsCount++;
            }
            else
            {
                Debug.LogWarning("Missing: card_back_blue");
                missingCount++;
            }

            // Summary
            Debug.Log($"Card Faces: {facesCount}/52");
            Debug.Log($"Card Backs: {backsCount}/1");

            if (missingCount == 0)
            {
                Debug.Log("<color=green>✓ All card sprites verified successfully!</color>");
            }
            else
            {
                Debug.LogWarning($"<color=yellow>⚠ Missing {missingCount} sprites</color>");
            }

            Debug.Log("====================================");
        }

        /// <summary>
        /// Menu item to test sprite loading
        /// </summary>
        [MenuItem("CardGames/Test Sprite Loading")]
        static void TestSpriteLoading()
        {
            Debug.Log("===== Testing Sprite Loading =====");

            // Test loading a few random cards
            string[] testCards = {
                "Cards/Faces/spades_ace",
                "Cards/Faces/hearts_king",
                "Cards/Faces/diamonds_10",
                "Cards/Faces/clubs_jack",
                "Cards/Backs/card_back_blue"
            };

            foreach (string path in testCards)
            {
                Sprite sprite = Resources.Load<Sprite>(path);

                if (sprite != null)
                {
                    Debug.Log($"✓ Loaded: {path} ({sprite.texture.width}x{sprite.texture.height})");
                }
                else
                {
                    Debug.LogError($"✗ Failed to load: {path}");
                }
            }

            Debug.Log("==================================");
        }
    }
}
