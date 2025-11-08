using UnityEngine;
using System.Collections;

namespace CardGames
{
    /// <summary>
    /// Manages responsive layout for card games across all screen sizes and orientations.
    /// Creates a square game area and positions UI panels around it.
    /// Works for portrait, landscape, desktop - any screen size!
    /// </summary>
    public class ResponsiveLayout : MonoBehaviour
    {
        [Header("Game Area Settings")]
        [SerializeField] private float gamePadding = 0.05f; // 5% padding around game area (more space for piles)

        [Header("Centering Adjustments")]
        [SerializeField] private float portraitVerticalShift = 0.15f; // Shift up in portrait (0.0 = no shift, 0.15 = 15% of screen)
        [SerializeField] private float landscapeVerticalShift = 0.15f; // Shift up in landscape (0.15 = 15% of screen)

        [Header("References")]
        [SerializeField] private Camera mainCamera;

        private float lastWidth;
        private float lastHeight;
        private float squareSize;
        private Vector2 gameAreaCenter;

        void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            CalculateGameArea();
            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }

        void Update()
        {
            // Detect screen size changes (rotation, resize, etc.)
            if (Screen.width != lastWidth || Screen.height != lastHeight)
            {
                Debug.Log($"[ResponsiveLayout] Screen changed: {Screen.width}x{Screen.height}");
                CalculateGameArea();
                lastWidth = Screen.width;
                lastHeight = Screen.height;
            }
        }

        /// <summary>
        /// Calculate the square game area based on screen dimensions
        /// </summary>
        void CalculateGameArea()
        {
            // Get screen dimensions in world units
            float screenHeight = mainCamera.orthographicSize * 2;
            float screenWidth = screenHeight * mainCamera.aspect;

            // Calculate the largest square that fits in the screen with padding
            float availableWidth = screenWidth * (1 - gamePadding * 2);
            float availableHeight = screenHeight * (1 - gamePadding * 2);

            // Square size is the smaller of the two dimensions
            squareSize = Mathf.Min(availableWidth, availableHeight);

            bool isLandscape = Screen.width > Screen.height;

            // Center the game area (shift for UI panels)
            // Use adjustable values that can be tweaked in Inspector
            float verticalShift;
            if (isLandscape)
            {
                verticalShift = screenHeight * landscapeVerticalShift;
            }
            else
            {
                verticalShift = screenHeight * portraitVerticalShift;
            }

            gameAreaCenter = new Vector2(0, verticalShift);

            Debug.Log($"[ResponsiveLayout] Screen: {screenWidth:F2} x {screenHeight:F2}");
            Debug.Log($"[ResponsiveLayout] Square size: {squareSize:F2}");
            Debug.Log($"[ResponsiveLayout] Orientation: {(isLandscape ? "Landscape" : "Portrait")}");

            // Notify game managers to reposition
            BroadcastMessage("OnGameAreaChanged", SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Get the boundaries of the square game area
        /// </summary>
        public Rect GetGameAreaBounds()
        {
            float halfSize = squareSize / 2;
            return new Rect(
                gameAreaCenter.x - halfSize,
                gameAreaCenter.y - halfSize,
                squareSize,
                squareSize
            );
        }

        /// <summary>
        /// Get the size of the square game area
        /// </summary>
        public float GetSquareSize()
        {
            return squareSize;
        }

        /// <summary>
        /// Get the center of the game area
        /// </summary>
        public Vector2 GetGameAreaCenter()
        {
            return gameAreaCenter;
        }

        /// <summary>
        /// Check if current orientation is landscape
        /// </summary>
        public bool IsLandscape()
        {
            return Screen.width > Screen.height;
        }

        /// <summary>
        /// Get UI panel positions based on orientation
        /// </summary>
        public Vector2 GetTopPanelPosition()
        {
            float screenHeight = mainCamera.orthographicSize * 2;
            float halfSquare = squareSize / 2;

            if (IsLandscape())
            {
                // In landscape, top panel goes to the left side
                float screenWidth = screenHeight * mainCamera.aspect;
                return new Vector2(-screenWidth / 2 + 2, 0);
            }
            else
            {
                // In portrait, top panel stays at top
                return new Vector2(0, screenHeight / 2 - 2);
            }
        }

        /// <summary>
        /// Get button panel position based on orientation
        /// </summary>
        public Vector2 GetBottomPanelPosition()
        {
            float screenHeight = mainCamera.orthographicSize * 2;
            float halfSquare = squareSize / 2;

            if (IsLandscape())
            {
                // In landscape, bottom panel goes to the right side
                float screenWidth = screenHeight * mainCamera.aspect;
                return new Vector2(screenWidth / 2 - 2, 0);
            }
            else
            {
                // In portrait, bottom panel stays at bottom
                return new Vector2(0, -screenHeight / 2 + 2);
            }
        }

        // Debug visualization
        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // Draw game area square
            Gizmos.color = Color.green;
            Rect bounds = GetGameAreaBounds();

            // Draw square outline
            Vector3[] corners = new Vector3[5];
            corners[0] = new Vector3(bounds.xMin, bounds.yMin, 0);
            corners[1] = new Vector3(bounds.xMax, bounds.yMin, 0);
            corners[2] = new Vector3(bounds.xMax, bounds.yMax, 0);
            corners[3] = new Vector3(bounds.xMin, bounds.yMax, 0);
            corners[4] = corners[0];

            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }

            // Draw center point
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(gameAreaCenter, 0.2f);
        }
    }
}
