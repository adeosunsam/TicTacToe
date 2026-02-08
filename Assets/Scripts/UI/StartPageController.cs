using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;
using TicTacToe.Achievements;
using TicTacToe.Core;
using TicTacToe.AI;

namespace TicTacToe.UI
{
    [DisallowMultipleComponent]
    public class StartPageController : MonoBehaviour
    {
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

        [Header("Difficulty Colors")]
        [SerializeField]
        private Color _easyColor = Color.green;

        [SerializeField]
        private Color _mediumColor = Color.yellow;

        [SerializeField]
        private Color _hardColor = new Color(1f, 0.37f, 0.34f); // #FF5F57

		[Header("Game Reference")]
        [SerializeField]
        private GamePresenter _gamePresenter;

        [Header("Achievements")]
        [SerializeField]
        private GameObject _achievementShowcase;

        [SerializeField]
        private AchievementNotification _achievementNotification;

        private AIDifficulty _selectedDifficulty = AIDifficulty.Easy;

        private void Awake()
        {
            SetupButtons();
            SetupSlider();
            ShowStartPage();
            SetDifficulty(AIDifficulty.Easy); // Default to Easy
        }
       
        private void OnDestroy()
        {
            RemoveButtonListeners();
        }

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

        private void OnPlayVsBotClicked()
        {
            StartGame(GameMode.PlayerVsBot, _selectedDifficulty);
        }

        private void OnPlayVsFriendClicked()
        {
            StartGame(GameMode.PlayerVsPlayer);
        }

        private void OnDifficultySliderChanged(float value)
        {
            int difficultyValue = Mathf.RoundToInt(value);
            AIDifficulty difficulty = (AIDifficulty)difficultyValue;
            SetDifficulty(difficulty);
        }

        private void SetDifficulty(AIDifficulty difficulty)
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
                    AIDifficulty.Easy => "EASY",
                    AIDifficulty.Medium => "MEDIUM",
                    AIDifficulty.Hard => "HARD",
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
                AIDifficulty.Easy => _easyColor,
                AIDifficulty.Medium => _mediumColor,
                AIDifficulty.Hard => _hardColor,
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
						handleImage.color = selectedColor;
                    }
                }
            }

            // Update difficulty text color
            if (_difficultyText != null)
            {
                _difficultyText.color = selectedColor;
            }
        }

        private void StartGame(GameMode mode, AIDifficulty? difficulty = null)
        {
            if (_gamePresenter == null)
            {
                return;
            }

            // Initialize game with selected settings
            _gamePresenter.InitializeWithSettings(mode, difficulty);

            // Hide start page, show gamePage
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
                _difficultyPanel.SetActive(true);
            }
            
            ShowPendingAchievementNotifications();
        }
        
        private void ShowPendingAchievementNotifications()
        {
            if (_gamePresenter == null || _achievementNotification == null)
            {
                return;
            }
            
            var achievementManager = GameManager.Instance.AchievementManager;
            if (achievementManager == null || !achievementManager.HasPendingNotifications())
            {
                return;
            }
            
            StartCoroutine(ShowPendingNotificationsSequentially(achievementManager));
        }
        
        private IEnumerator ShowPendingNotificationsSequentially(AchievementManager achievementManager)
        {
            while (achievementManager.HasPendingNotifications())
            {
                var achievement = achievementManager.GetNextPendingNotification();
                if (achievement != null)
                {
                    _achievementNotification.ShowAchievement(achievement);
                    
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

        public void ReturnToStartPage()
        {
            if (_gamePresenter != null)
            {
                _gamePresenter.OnResetButtonClicked();
            }

            // Return to start page
            ShowStartPage();
        }

        public void AchievementClicked()
        {
            _achievementShowcase.SetActive(true);
		}

        public void ShowDifficultyPanel(bool show)
        {
            if (_difficultyPanel != null)
            {
                _difficultyPanel.SetActive(show);
            }
        }
    }
}
