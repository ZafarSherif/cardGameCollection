using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CardGames.Solitaire
{
    /// <summary>
    /// Manages the Solitaire game UI (score, moves, timer, buttons)
    /// </summary>
    public class SolitaireUIManager : MonoBehaviour
    {
        [Header("UI Text Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private TextMeshProUGUI timerText;

        [Header("Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button undoButton;

        [Header("Win Popup")]
        [SerializeField] private GameObject winPanel;
        [SerializeField] private TextMeshProUGUI winMessageText;
        [SerializeField] private TextMeshProUGUI winScoreText;
        [SerializeField] private TextMeshProUGUI winTimeText;
        [SerializeField] private Button winNewGameButton;
        [SerializeField] private Button winCloseButton;

        [Header("Game Manager Reference")]
        [SerializeField] private SolitaireGameManager gameManager;

        private float gameTime = 0f;
        private bool isGameRunning = false;

        private void Start()
        {
            // Find game manager if not assigned
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<SolitaireGameManager>();
            }

            // Setup button listeners
            if (newGameButton != null)
            {
                newGameButton.onClick.AddListener(OnNewGameClicked);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (undoButton != null)
            {
                undoButton.onClick.AddListener(OnUndoClicked);
            }

            // Setup win panel button listeners
            if (winNewGameButton != null)
            {
                winNewGameButton.onClick.AddListener(OnWinNewGameClicked);
            }

            if (winCloseButton != null)
            {
                winCloseButton.onClick.AddListener(OnWinCloseClicked);
            }

            // Hide win panel initially
            HideWinPanel();

            // Subscribe to game manager events
            if (gameManager != null)
            {
                gameManager.OnScoreChanged += UpdateScore;
                gameManager.OnMovesChanged += UpdateMoves;
                gameManager.OnGameEnd += OnGameEnd;
            }

            // Start timer
            StartGame();

            // Initial update
            UpdateScore(0);
            UpdateMoves(0);
        }

        private void Update()
        {
            // Update timer every frame
            if (isGameRunning)
            {
                gameTime += Time.deltaTime;
                UpdateTimer();
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (gameManager != null)
            {
                gameManager.OnScoreChanged -= UpdateScore;
                gameManager.OnMovesChanged -= UpdateMoves;
                gameManager.OnGameEnd -= OnGameEnd;
            }

            // Remove button listeners
            if (newGameButton != null) newGameButton.onClick.RemoveAllListeners();
            if (restartButton != null) restartButton.onClick.RemoveAllListeners();
            if (undoButton != null) undoButton.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// Start/restart the game timer
        /// </summary>
        public void StartGame()
        {
            gameTime = 0f;
            isGameRunning = true;
        }

        /// <summary>
        /// Stop the game timer
        /// </summary>
        public void StopGame()
        {
            isGameRunning = false;
        }

        /// <summary>
        /// Update score display
        /// </summary>
        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }

        /// <summary>
        /// Update moves display
        /// </summary>
        private void UpdateMoves(int moves)
        {
            if (movesText != null)
            {
                movesText.text = $"Moves: {moves}";
            }
        }

        /// <summary>
        /// Update timer display
        /// </summary>
        private void UpdateTimer()
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(gameTime / 60f);
                int seconds = Mathf.FloorToInt(gameTime % 60f);
                timerText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }

        /// <summary>
        /// Called when game ends
        /// </summary>
        private void OnGameEnd(bool won, int finalScore, int finalTime)
        {
            StopGame();

            if (won)
            {
                Debug.Log($"🎉 YOU WON! Score: {finalScore}, Time: {finalTime}s");
                ShowWinPanel(finalScore, finalTime);
            }
        }

        /// <summary>
        /// New Game button clicked
        /// </summary>
        private void OnNewGameClicked()
        {
            Debug.Log("[UI] New Game clicked");
            HideWinPanel();

            if (gameManager != null)
            {
                gameManager.InitializeGame();
                StartGame();
            }
        }

        /// <summary>
        /// Restart button clicked
        /// </summary>
        private void OnRestartClicked()
        {
            Debug.Log("[UI] Restart clicked");
            HideWinPanel();

            if (gameManager != null)
            {
                gameManager.RestartGame();
                StartGame();
            }
        }

        /// <summary>
        /// Undo button clicked
        /// </summary>
        private void OnUndoClicked()
        {
            Debug.Log("[UI] Undo clicked");

            if (gameManager != null)
            {
                bool undoSuccessful = gameManager.UndoLastMove();

                if (!undoSuccessful)
                {
                    Debug.Log("[UI] No moves to undo");
                }
            }
        }

        /// <summary>
        /// Enable/disable undo button
        /// </summary>
        public void SetUndoButtonEnabled(bool enabled)
        {
            if (undoButton != null)
            {
                undoButton.interactable = enabled;
            }
        }

        /// <summary>
        /// Show the win panel with final score and time
        /// </summary>
        private void ShowWinPanel(int finalScore, int finalTime)
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true);

                // Update win panel text
                if (winMessageText != null)
                {
                    winMessageText.text = "🎉 YOU WON! 🎉";
                }

                if (winScoreText != null)
                {
                    winScoreText.text = $"Final Score: {finalScore}";
                }

                if (winTimeText != null)
                {
                    int minutes = finalTime / 60;
                    int seconds = finalTime % 60;
                    winTimeText.text = $"Time: {minutes:00}:{seconds:00}";
                }
            }
        }

        /// <summary>
        /// Hide the win panel
        /// </summary>
        private void HideWinPanel()
        {
            if (winPanel != null)
            {
                winPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Win panel New Game button clicked
        /// </summary>
        private void OnWinNewGameClicked()
        {
            Debug.Log("[UI] Win panel New Game clicked");
            HideWinPanel();

            if (gameManager != null)
            {
                gameManager.InitializeGame();
                StartGame();
            }
        }

        /// <summary>
        /// Win panel Close button clicked
        /// </summary>
        private void OnWinCloseClicked()
        {
            Debug.Log("[UI] Win panel Close clicked");
            HideWinPanel();
        }
    }
}
