using UnityEngine;
using System.Runtime.InteropServices;
using CardGames.Solitaire;

namespace CardGames.Bridge
{
    /// <summary>
    /// Bridge for communication between Unity and React Native
    /// </summary>
    public class ReactNativeBridge : MonoBehaviour
    {
        private SolitaireGameManager gameManager;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        private void Start()
        {
            gameManager = FindObjectOfType<SolitaireGameManager>();

            if (gameManager != null)
            {
                // Subscribe to game events
                gameManager.OnScoreChanged += HandleScoreChanged;
                gameManager.OnMovesChanged += HandleMovesChanged;
                gameManager.OnGameEnd += HandleGameEnd;
            }

            // Notify React Native that game is ready
            SendMessageToReactNative("GameReady", "{}");
        }

        #region Messages FROM React Native

        /// <summary>
        /// Called by React Native to send messages to Unity
        /// </summary>
        public void ReceiveMessageFromRN(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[RN Bridge] Received message: {message}");

            try
            {
                MessageData data = JsonUtility.FromJson<MessageData>(message);

                switch (data.type)
                {
                    case "StartGame":
                        HandleStartGame(data.data);
                        break;

                    case "PauseGame":
                        HandlePauseGame();
                        break;

                    case "ResumeGame":
                        HandleResumeGame();
                        break;

                    case "QuitGame":
                        HandleQuitGame();
                        break;

                    case "SetLanguage":
                        HandleSetLanguage(data.data);
                        break;

                    default:
                        Debug.LogWarning($"[RN Bridge] Unknown message type: {data.type}");
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[RN Bridge] Error parsing message: {e.Message}");
            }
        }

        private void HandleStartGame(string data)
        {
            if (gameManager != null)
            {
                gameManager.InitializeGame();
            }
        }

        private void HandlePauseGame()
        {
            Time.timeScale = 0f;
            SendMessageToReactNative("GamePaused", "{}");
        }

        private void HandleResumeGame()
        {
            Time.timeScale = 1f;
            SendMessageToReactNative("GameResumed", "{}");
        }

        private void HandleQuitGame()
        {
            // Send quit confirmation
            SendMessageToReactNative("GameQuit", "{}");
        }

        private void HandleSetLanguage(string data)
        {
            // Language handling would go here
            // For now, just acknowledge
            if (enableDebugLogs)
                Debug.Log($"[RN Bridge] Language set to: {data}");
        }

        #endregion

        #region Messages TO React Native

        /// <summary>
        /// Send a message to React Native
        /// </summary>
        private void SendMessageToReactNative(string messageType, string data)
        {
            if (enableDebugLogs)
                Debug.Log($"[RN Bridge] Sending to RN: {messageType}");

#if UNITY_WEBGL && !UNITY_EDITOR
            // WebGL: Call JavaScript function
            SendMessageToJS(messageType, data);
#elif (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
            // Mobile: Use native bridge (implementation depends on react-native-unity-view)
            SendMessageToNative(messageType, data);
#else
            // Editor: Just log
            Debug.Log($"[RN Bridge] Would send: {messageType} | {data}");
#endif
        }

        private void HandleScoreChanged(int score)
        {
            var data = new ScoreUpdateData { score = score };
            SendMessageToReactNative("ScoreUpdate", JsonUtility.ToJson(data));
        }

        private void HandleMovesChanged(int moves)
        {
            var data = new MovesUpdateData { moves = moves };
            SendMessageToReactNative("MovesUpdate", JsonUtility.ToJson(data));
        }

        private void HandleGameEnd(bool won, int score, int timeInSeconds)
        {
            var data = new GameCompleteData
            {
                won = won,
                score = score,
                timeSeconds = timeInSeconds,
                coinsEarned = CalculateCoinsEarned(won, score, timeInSeconds)
            };

            SendMessageToReactNative("GameComplete", JsonUtility.ToJson(data));
        }

        private int CalculateCoinsEarned(bool won, int score, int timeInSeconds)
        {
            if (!won) return 0;

            int baseCoins = 100;
            int scoreBonus = score / 10;

            // Time bonus (faster = more coins)
            int timeBonus = 0;
            if (timeInSeconds < 180) timeBonus = 50;      // Under 3 minutes
            else if (timeInSeconds < 300) timeBonus = 30; // Under 5 minutes
            else if (timeInSeconds < 600) timeBonus = 10; // Under 10 minutes

            return baseCoins + scoreBonus + timeBonus;
        }

        #endregion

        #region Platform-Specific Implementations

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SendMessageToJS(string messageType, string data);
#else
        private static void SendMessageToJS(string messageType, string data)
        {
            // Stub for non-WebGL platforms
        }
#endif

        private void SendMessageToNative(string messageType, string data)
        {
            // For react-native-unity-view
            // The exact implementation depends on the bridge library used
            // Typically: UnitySendMessage or similar
            Debug.Log($"[RN Bridge] Native message: {messageType}");
        }

        #endregion

        #region Data Structures

        [System.Serializable]
        private class MessageData
        {
            public string type;
            public string data;
        }

        [System.Serializable]
        private class ScoreUpdateData
        {
            public int score;
        }

        [System.Serializable]
        private class MovesUpdateData
        {
            public int moves;
        }

        [System.Serializable]
        private class GameCompleteData
        {
            public bool won;
            public int score;
            public int timeSeconds;
            public int coinsEarned;
        }

        #endregion

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (gameManager != null)
            {
                gameManager.OnScoreChanged -= HandleScoreChanged;
                gameManager.OnMovesChanged -= HandleMovesChanged;
                gameManager.OnGameEnd -= HandleGameEnd;
            }
        }
    }
}
