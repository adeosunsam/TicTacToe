using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;
using TicTacToe.Achievements;

namespace TicTacToe.UI
{
    /// <summary>
    /// Controls the start page menu where players select game mode.
    /// </summary>
    [DisallowMultipleComponent]
    public class StartPageController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI References")]
        [SerializeField]
        private GameObject _startPagePanel;

        [SerializeField]
        private GameObject _gamePanel;

        [SerializeField]
        private Button _playVsBotButton;

        [SerializeField]
        private Button _playVsFriendButton;

        [Header("Difficulty Selection (for Bot mode)")]
        [SerializeField]
        private GameObject _difficultyPanel;

        [SerializeField]
        private Slider _difficultySlider;

        [SerializeField]
        [Tooltip("Text showing selected difficulty")]
        private TextMeshProUGUI _difficultyText;

        [Header("Difficulty Colors")]
        [SerializeField]
        private Color _easyColor = Color.green;

        [SerializeField]
        private Color _mediumColor = Color.yellow;

        [SerializeField]
        private Color _hardColor = new Color(1f, 0.37f, 0.34f); // #FF5F57

        [SerializeField]
		[Range(0f, 20f)]
		private float darkenFactor = 0.5f;

		[Header("Game Reference")]
        [SerializeField]
        private GamePresenter _gamePresenter;

        [Header("Achievements")]
        [SerializeField]
        private AchievementShowcase _achievementShowcase;

        [SerializeField]
        private Button _achievementsButton;
        
        [SerializeField]
        private AchievementNotification _achievementNotification;

        #endregion

        #region Private Fields

        private AI.AIDifficulty _selectedDifficulty = AI.AIDifficulty.Easy;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            SetupButtons();
            SetupSlider();
            ShowStartPage();
            SetDifficulty(AI.AIDifficulty.Easy); // Default to Easy
        }
        
        private void Start()
        {
            Debug.Log("[StartPageController] Initializing achievement showcase...");
            
            // Initialize achievement showcase with the achievement manager from game presenter
            if (_gamePresenter != null && _achievementShowcase != null)
            {
                // Ensure game presenter has initialized its achievement manager
                if (_gamePresenter.AchievementManager == null)
                {
                    Debug.Log("[StartPageController] Initializing game presenter to create achievement manager");
                    // Initialize a temporary game to ensure achievement manager exists
                    _gamePresenter.InitializeWithSettings(Core.GameMode.PlayerVsPlayer, AI.AIDifficulty.Easy);
                }
                
                if (_gamePresenter.AchievementManager != null)
                {
                    _achievementShowcase.Initialize(_gamePresenter.AchievementManager);
                    Debug.Log("[StartPageController] Achievement showcase initialized successfully");
                }
                else
                {
                    Debug.LogError("[StartPageController] Failed to create achievement manager!");
                }
            }
            else
            {
                Debug.LogWarning($"[StartPageController] Cannot initialize showcase: GamePresenter={_gamePresenter != null}, Showcase={_achievementShowcase != null}");
            }
        }
        
        /// <summary>
        /// Resets all achievements for testing. Right-click this component in Inspector and select "Reset All Achievements".
        /// </summary>
        [ContextMenu("Reset All Achievements")]
        private void ResetAchievements()
        {
            if (_gamePresenter?.AchievementManager != null)
            {
                _gamePresenter.AchievementManager.ResetAllAchievements();
                Debug.Log("[StartPageController] All achievements have been reset!");
                
                // Refresh showcase if it's open
                if (_achievementShowcase != null)
                {
                    _achievementShowcase.Hide();
                }
            }
        }

        private void OnDestroy()
        {
            RemoveButtonListeners();
        }

        #endregion

        #region Initialization

        private void SetupButtons()
        {
            if (_playVsBotButton != null)
            {
                _playVsBotButton.onClick.AddListener(OnPlayVsBotClicked);
            }

            if (_playVsFriendButton != null)
            {
                _playVsFriendButton.onClick.AddListener(OnPlayVsFriendClicked);
            }

            if (_achievementsButton != null)
            {
                _achievementsButton.onClick.AddListener(OnAchievementsClicked);
            }
        }

        private void SetupSlider()
        {
            if (_difficultySlider != null)
            {
                _difficultySlider.minValue = 0;
                _difficultySlider.maxValue = 2;
                _difficultySlider.wholeNumbers = true;
                _difficultySlider.value = 0; // Default to Easy
                _difficultySlider.onValueChanged.AddListener(OnDifficultySliderChanged);
            }
        }

        private void RemoveButtonListeners()
        {
            if (_playVsBotButton != null)
            {
                _playVsBotButton.onClick.RemoveListener(OnPlayVsBotClicked);
            }

            if (_playVsFriendButton != null)
            {
                _playVsFriendButton.onClick.RemoveListener(OnPlayVsFriendClicked);
            }

            if (_achievementsButton != null)
            {
                _achievementsButton.onClick.RemoveListener(OnAchievementsClicked);
            }

            if (_difficultySlider != null)
            {
                _difficultySlider.onValueChanged.RemoveListener(OnDifficultySliderChanged);
            }
        }

        #endregion

        #region Button Handlers

        private void OnPlayVsBotClicked()
        {
            StartGame(Core.GameMode.PlayerVsBot, _selectedDifficulty);
        }

        private void OnPlayVsFriendClicked()
        {
            StartGame(Core.GameMode.PlayerVsPlayer, AI.AIDifficulty.Easy); // Difficulty doesn't matter for PvP
        }

        #endregion

        #region Difficulty Management

        private void OnDifficultySliderChanged(float value)
        {
            int difficultyValue = Mathf.RoundToInt(value);
            AI.AIDifficulty difficulty = (AI.AIDifficulty)difficultyValue;
            SetDifficulty(difficulty);
        }

        private void SetDifficulty(AI.AIDifficulty difficulty)
        {
            _selectedDifficulty = difficulty;
            UpdateDifficultyUI();
        }

        private void UpdateDifficultyUI()
        {
            if (_difficultyText != null)
            {
                string difficultyName = _selectedDifficulty switch
                {
                    AI.AIDifficulty.Easy => "EASY",
                    AI.AIDifficulty.Medium => "MEDIUM",
                    AI.AIDifficulty.Hard => "HARD",
                    _ => "EASY"
                };
                _difficultyText.text = difficultyName;
            }

            // Update colors based on difficulty
            UpdateDifficultyColors();
        }

        private void UpdateDifficultyColors()
        {
            Color selectedColor = _selectedDifficulty switch
            {
                AI.AIDifficulty.Easy => _easyColor,
                AI.AIDifficulty.Medium => _mediumColor,
                AI.AIDifficulty.Hard => _hardColor,
                _ => _easyColor
            };

            // Update Play vs Bot button color
            if (_playVsBotButton != null)
            {
                var image = _playVsBotButton.GetComponent<Image>();
                if (image != null)
                {
                    image.color = selectedColor;
                }
            }

            // Update slider colors
            if (_difficultySlider != null)
            {
                // Disable slider transition to prevent color override
                _difficultySlider.transition = Selectable.Transition.None;

                // Update fill color
                var fillRect = _difficultySlider.fillRect;
                if (fillRect != null)
                {
                    if (fillRect.TryGetComponent<Image>(out var fillImage))
                    {
                        fillImage.color = selectedColor;
                    }
                }

                var handleRect = _difficultySlider.handleRect;
                if (handleRect != null)
                {
					var handleImage = handleRect.GetComponentsInChildren<Image>().Last();

					if (handleImage != null)
                    {
						//Color darkened = selectedColor * darkenFactor;
      //                  Debug.Log($"Selected Color: {selectedColor}");
      //                  Debug.Log($"Darkened Color: {darkened}");
						//darkened.a = selectedColor.a;
						//handleImage.color = darkened;
						handleImage.color = selectedColor;
                    }
                    else
                    {
                        Debug.LogWarning("[StartPageController] No Image found on slider handle!");
                    }
                }
            }

            // Update difficulty text color
            if (_difficultyText != null)
            {
                _difficultyText.color = selectedColor;
            }
        }

        #endregion

        #region Game Flow

        private void StartGame(Core.GameMode mode, AI.AIDifficulty difficulty)
        {
            if (_gamePresenter == null)
            {
                return;
            }


            // Initialize game with selected settings
            _gamePresenter.InitializeWithSettings(mode, difficulty);

            // Hide start page, show game
            ShowGamePage();
        }

        private void ShowStartPage()
        {
            if (_startPagePanel != null)
            {
                _startPagePanel.SetActive(true);
            }

            if (_gamePanel != null)
            {
                _gamePanel.SetActive(false);
            }

            if (_difficultyPanel != null)
            {
                _difficultyPanel.SetActive(true); // Show difficulty selection
            }
            
            // Show any pending achievement notifications
            ShowPendingAchievementNotifications();
        }
        
        private void ShowPendingAchievementNotifications()
        {
            Debug.Log("[StartPageController] Checking for pending achievement notifications...");
            
            if (_gamePresenter == null || _achievementNotification == null)
            {
                Debug.LogWarning($"[StartPageController] Missing references: GamePresenter={_gamePresenter != null}, Notification={_achievementNotification != null}");
                return;
            }
            
            var achievementManager = _gamePresenter.AchievementManager;
            if (achievementManager == null)
            {
                Debug.LogWarning("[StartPageController] Achievement manager is null");
                return;
            }
            
            if (!achievementManager.HasPendingNotifications())
            {
                Debug.Log("[StartPageController] No pending notifications");
                return;
            }
            
            // Start coroutine to show all pending notifications sequentially
            StartCoroutine(ShowPendingNotificationsSequentially(achievementManager));
        }
        
        private System.Collections.IEnumerator ShowPendingNotificationsSequentially(AchievementManager achievementManager)
        {
            while (achievementManager.HasPendingNotifications())
            {
                var achievement = achievementManager.GetNextPendingNotification();
                if (achievement != null)
                {
                    Debug.Log($"[StartPageController] Showing pending notification: {achievement.Title}");
                    _achievementNotification.ShowAchievement(achievement);
                    
                    // Wait for notification to complete (0.5s slide in + 3s display + 0.3s slide out = 3.8s)
                    yield return new WaitForSeconds(4.0f);
                }
            }
        }

        private void ShowGamePage()
        {
            if (_startPagePanel != null)
            {
                _startPagePanel.SetActive(false);
            }

            if (_gamePanel != null)
            {
                _gamePanel.SetActive(true);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns to the start page and resets the game state.
        /// Call this from the back button in the gameplay panel.
        /// </summary>
        public void ReturnToStartPage()
        {
            // Reset the game board if game was initialized
            if (_gamePresenter != null)
            {
                _gamePresenter.OnResetButtonClicked();
            }

            // Return to start page
            ShowStartPage();
        }

        /// <summary>
        /// Opens the achievements showcase.
        /// </summary>
        private void OnAchievementsClicked()
        {
            Debug.Log("[StartPageController] Achievements button clicked");
            
            // Ensure achievement showcase is initialized
            if (_achievementShowcase == null)
            {
                Debug.LogError("[StartPageController] Achievement showcase reference is null!");
                return;
            }
            
            // Try to initialize if not already done
            if (_gamePresenter != null && _gamePresenter.AchievementManager != null)
            {
                _achievementShowcase.Initialize(_gamePresenter.AchievementManager);
            }
            
            _achievementShowcase.Show();
        }

        /// <summary>
        /// Shows/hides the difficulty panel.
        /// </summary>
        public void ShowDifficultyPanel(bool show)
        {
            if (_difficultyPanel != null)
            {
                _difficultyPanel.SetActive(show);
            }
        }

        #endregion
    }
}
