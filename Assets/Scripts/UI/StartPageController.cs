using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        private TextMeshProUGUI _difficultyText;

        [Header("Game Reference")]
        [SerializeField]
        private GamePresenter _gamePresenter;

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
