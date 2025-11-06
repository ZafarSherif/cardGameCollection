using UnityEngine;

namespace CardGames.Core
{
    /// <summary>
    /// Manages all audio playback for card games
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance => instance;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;

        [Header("Card Sounds")]
        private AudioClip[] cardFlipSounds;
        private AudioClip[] cardPlaceSounds;

        [Header("Settings")]
        [SerializeField] private bool soundEnabled = true;
        [SerializeField] [Range(0f, 1f)] private float volume = 0.7f;

        private void Awake()
        {
            // Singleton pattern
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Create audio source if not assigned
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }

            sfxSource.volume = volume;

            LoadSounds();
        }

        /// <summary>
        /// Load all sound files from Resources/Sounds
        /// </summary>
        private void LoadSounds()
        {
            // Load flip sounds
            cardFlipSounds = new AudioClip[]
            {
                Resources.Load<AudioClip>("Sounds/cardFlip1"),
                Resources.Load<AudioClip>("Sounds/cardFlip2"),
                Resources.Load<AudioClip>("Sounds/cardFlip3")
            };

            // Load place sounds
            cardPlaceSounds = new AudioClip[]
            {
                Resources.Load<AudioClip>("Sounds/cardPlace1"),
                Resources.Load<AudioClip>("Sounds/cardPlace2"),
                Resources.Load<AudioClip>("Sounds/cardPlace3")
            };

            // Verify all sounds loaded
            int loadedFlips = 0;
            foreach (var clip in cardFlipSounds)
            {
                if (clip != null) loadedFlips++;
            }

            int loadedPlaces = 0;
            foreach (var clip in cardPlaceSounds)
            {
                if (clip != null) loadedPlaces++;
            }

            Debug.Log($"[AudioManager] Loaded {loadedFlips}/3 flip sounds, {loadedPlaces}/3 place sounds");
        }

        /// <summary>
        /// Play a random card flip sound
        /// </summary>
        public void PlayCardFlip()
        {
            if (!soundEnabled || cardFlipSounds == null || cardFlipSounds.Length == 0) return;

            // Pick random flip sound
            AudioClip clip = cardFlipSounds[Random.Range(0, cardFlipSounds.Length)];
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Play a random card place sound
        /// </summary>
        public void PlayCardPlace()
        {
            if (!soundEnabled || cardPlaceSounds == null || cardPlaceSounds.Length == 0) return;

            // Pick random place sound
            AudioClip clip = cardPlaceSounds[Random.Range(0, cardPlaceSounds.Length)];
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Toggle sound on/off
        /// </summary>
        public void ToggleSound()
        {
            soundEnabled = !soundEnabled;
            Debug.Log($"[AudioManager] Sound {(soundEnabled ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Set volume (0-1)
        /// </summary>
        public void SetVolume(float newVolume)
        {
            volume = Mathf.Clamp01(newVolume);
            if (sfxSource != null)
            {
                sfxSource.volume = volume;
            }
        }

        /// <summary>
        /// Check if sound is enabled
        /// </summary>
        public bool IsSoundEnabled()
        {
            return soundEnabled;
        }
    }
}
