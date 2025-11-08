using UnityEngine;
using System.Runtime.InteropServices;

namespace CardGames.Core
{
    /// <summary>
    /// Universal bridge for communicating between Unity and React Native WebView.
    /// Works with any game - sends events to React, receives actions from React.
    ///
    /// STANDARD PROTOCOL (works across all games and engines):
    ///
    /// Unity → React Events:
    ///   - gameReady: Game loaded and ready to play
    ///   - gameState: Game state updated (score, moves, balance, etc.)
    ///   - gameEnd: Game finished (won/lost)
    ///
    /// React → Unity Actions:
    ///   - newGame: Start a new game
    ///   - restart: Restart current game
    ///   - undo: Undo last move
    ///   - customAction: Game-specific action (bet, hit, stand, etc.)
    /// </summary>
    public class ReactBridge : MonoBehaviour
    {
        private static ReactBridge instance;
        public static ReactBridge Instance => instance;

        // Delegate for handling React messages
        public delegate void ReactMessageHandler(string json);
        public static event ReactMessageHandler OnReactMessageReceived;

        // Platform-specific communication
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SendMessageToReact(string message);
#endif

        private void Awake()
        {
            // Singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("[ReactBridge] Initialized");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Notify React that Unity is ready
            SendGameReady();
        }

        /// <summary>
        /// Send "gameReady" event - called when game finishes loading
        /// </summary>
        public void SendGameReady()
        {
            SendToReact("gameReady", "{}");
        }

        /// <summary>
        /// Send generic event to React Native
        /// </summary>
        /// <param name="eventType">Event type (gameReady, gameState, gameEnd, etc.)</param>
        /// <param name="payloadJson">JSON payload string</param>
        public void SendToReact(string eventType, string payloadJson)
        {
            // Construct message in standard format
            string message = $"{{\"type\":\"{eventType}\",\"payload\":{payloadJson}}}";

            Debug.Log($"[ReactBridge → React] {message}");

#if UNITY_WEBGL && !UNITY_EDITOR
            // Send to React Native via injected JavaScript function
            SendMessageToReact(message);
#else
            // In editor, just log (for testing)
            Debug.Log("[ReactBridge] (WebGL only, simulated in editor)");
#endif
        }

        /// <summary>
        /// Called by React Native WebView via:
        /// window.unityInstance.SendMessage('ReactBridge', 'ReceiveFromReact', jsonString)
        /// </summary>
        /// <param name="json">JSON string with format: {"action": "newGame", "data": {...}}</param>
        public void ReceiveFromReact(string json)
        {
            Debug.Log($"[ReactBridge ← React] {json}");

            // Invoke event - all subscribed game managers will receive this
            OnReactMessageReceived?.Invoke(json);

            Debug.Log($"[ReactBridge] Event invoked, subscribers: {OnReactMessageReceived?.GetInvocationList().Length ?? 0}");
        }
    }

    // ======================================
    // Standard Payload Classes
    // Reusable across all games
    // ======================================

    /// <summary>
    /// Standard game state payload (customize per game)
    /// </summary>
    [System.Serializable]
    public class GameStatePayload
    {
        public int score;
        public int moves;
        public string time;

        // Game-specific fields can be added:
        // public int balance;
        // public int bet;
        // public string[] hand;

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }

    /// <summary>
    /// Standard game end payload
    /// </summary>
    [System.Serializable]
    public class GameEndPayload
    {
        public bool won;
        public int finalScore;
        public string finalTime;

        // Game-specific fields:
        // public int finalBalance;
        // public int handsWon;

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }

    /// <summary>
    /// Incoming action from React Native
    /// </summary>
    [System.Serializable]
    public class ReactAction
    {
        public string action;
        public string data; // JSON string for additional data

        public static ReactAction FromJson(string json)
        {
            return JsonUtility.FromJson<ReactAction>(json);
        }
    }
}
