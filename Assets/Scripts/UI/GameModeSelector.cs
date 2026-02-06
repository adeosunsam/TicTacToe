using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TicTacToe.AI;
using TicTacToe.Core;
using System.Collections.Generic;

namespace TicTacToe.UI
{
    /// <summary>
    /// UI component for selecting game mode and AI difficulty.
    /// </summary>
    [DisallowMultipleComponent]
    public class GameModeSelector : MonoBehaviour
    {
        #region Serialized Fields

        [Header("References")]
        [SerializeField]
        private GamePresenter gamePresenter;

        [Header("Mode Selection")]
        [SerializeField]
        private Button pvpButton;

        [SerializeField]
        private Button pvbButton;

        [Header("Difficulty Selection")]
        [SerializeField]
        private TMP_Dropdown difficultyDropdown;

        [SerializeField]
        private GameObject difficultyPanel;

        [Header("Visual Feedback")]
        [SerializeField]
        private Color selectedColor = new Color(0.2f, 0.8f, 0.2f);

        [SerializeField]
        private Color unselectedColor = new Color(0.8f, 0.8f, 0.8f);

        #endregion

        #region Private Fields

        private GameMode currentMode = GameMode.PlayerVsPlayer;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            SetupListeners();
            InitializeUI();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        #endregion

        #region Initialization

        private void SetupListeners()
        {
            if (pvpButton != null)
            {
                pvpButton.onClick.AddListener(OnPvPButtonClicked);
            }

            if (pvbButton != null)
            {
                pvbButton.onClick.AddListener(OnPvBButtonClicked);
            }

            if (difficultyDropdown != null)
            {
                difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
            }
        }

        private void RemoveListeners()
        {
            if (pvpButton != null)
            {
                pvpButton.onClick.RemoveListener(OnPvPButtonClicked);
            }

            if (pvbButton != null)
            {
                pvbButton.onClick.RemoveListener(OnPvBButtonClicked);
            }

            if (difficultyDropdown != null)
            {
                difficultyDropdown.onValueChanged.RemoveListener(OnDifficultyChanged);
            }
        }

        private void InitializeUI()
        {
            UpdateButtonColors();
            UpdateDifficultyPanelVisibility();

            // Setup dropdown options if present
            if (difficultyDropdown != null)
            {
                difficultyDropdown.ClearOptions();
                difficultyDropdown.AddOptions(new List<string>
                {
                    "Easy",
                    "Medium",
                    "Hard"
                });
                difficultyDropdown.value = 2;
            }
        }

        #endregion

        private void OnPvPButtonClicked()
        {
            SetGameMode(GameMode.PlayerVsPlayer);
        }

        private void OnPvBButtonClicked()
        {
            SetGameMode(GameMode.PlayerVsBot);
        }

        private void OnDifficultyChanged(int index)
        {
            AIDifficulty difficulty = index switch
            {
                0 => AIDifficulty.Easy,
                1 => AIDifficulty.Medium,
                2 => AIDifficulty.Hard,
                _ => AIDifficulty.Hard
            };

            gamePresenter?.SetAIDifficulty(difficulty);
        }

        #region Private Methods

        private void SetGameMode(GameMode mode)
        {
            if (currentMode == mode) return;

            currentMode = mode;
            gamePresenter?.SetGameMode(mode);

            UpdateButtonColors();
            UpdateDifficultyPanelVisibility();
        }

        private void UpdateButtonColors()
        {
            if (pvpButton != null)
            {
                var colors = pvpButton.colors;
                colors.normalColor = currentMode == GameMode.PlayerVsPlayer ? selectedColor : unselectedColor;
                pvpButton.colors = colors;
            }

            if (pvbButton != null)
            {
                var colors = pvbButton.colors;
                colors.normalColor = currentMode == GameMode.PlayerVsBot ? selectedColor : unselectedColor;
                pvbButton.colors = colors;
            }
        }

        private void UpdateDifficultyPanelVisibility()
        {
            if (difficultyPanel != null)
            {
                difficultyPanel.SetActive(currentMode == GameMode.PlayerVsBot);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the game mode programmatically.
        /// </summary>
        public void SetMode(GameMode mode)
        {
            SetGameMode(mode);
        }

        /// <summary>
        /// Gets the current game mode.
        /// </summary>
        public GameMode GetCurrentMode()
        {
            return currentMode;
        }

        #endregion
    }
}
