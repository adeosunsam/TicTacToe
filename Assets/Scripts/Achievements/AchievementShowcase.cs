using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace TicTacToe.Achievements
{
    public class AchievementShowcase : MonoBehaviour
    {
        [Header("UI References")]
        
        [SerializeField]
        private Transform achievementListContainer;
        
        [SerializeField]
        private GameObject achievementItemPrefab;

        [SerializeField]
        private Sprite unlockedSprite;
        
        [SerializeField]
        private TextMeshProUGUI progressText;
        
        [SerializeField]
        private Button closeButton;

        [Header("Colors")]
        [SerializeField]
        private Color unlockedColor = new (1f, 0.84f, 0f); // Gold
        
        [SerializeField]
        private Color lockedColor = new (0.5f, 0.5f, 0.5f); // Gray

        public void OnEnable()
        {
            StartCoroutine(RefreshAfterActivation());
        }
        
        private System.Collections.IEnumerator RefreshAfterActivation()
        {
            yield return null;
            RefreshAchievementList();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void RefreshAchievementList()
        {
            // Clear existing items
            foreach (Transform child in achievementListContainer)
            {
                Destroy(child.gameObject);
            }

            // Get all achievements
            List<Achievement> achievements = GameManager.Instance.AchievementManager.GetAllAchievements();
            int unlockedCount = GameManager.Instance.AchievementManager.GetUnlockedCount();

            // Update progress text
            if (progressText != null)
            {
                progressText.text = $"Achievements: {unlockedCount}/{achievements.Count}";
            }

            // Create achievement items
            foreach (Achievement achievement in achievements)
            {
                CreateAchievementItem(achievement);
            }
        }

        private void CreateAchievementItem(Achievement achievement)
        {
            if (achievementItemPrefab == null) return;

            GameObject item = Instantiate(achievementItemPrefab, achievementListContainer);
            
            TextMeshProUGUI titleText = item.transform.Find("AchievementText")?.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();

            TextMeshProUGUI descriptionText = item.transform.Find("AchievementText")?.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();

            Image iconImage = item.transform.Find("Icon")?.GetComponent<Image>();
            Image backgroundImage = item.GetComponent<Image>();
            
            Image checker = item.transform.Find("Checker")?.GetComponent<Image>();

            if (titleText != null)
            {
                titleText.text = achievement.Title;
				titleText.color = achievement.IsUnlocked ? Color.black : lockedColor;
			}

            if (descriptionText != null)
            {
                descriptionText.text = achievement.Description;
                descriptionText.color = achievement.IsUnlocked ? Color.black : lockedColor;
            }

            if (iconImage != null)
            {
                iconImage.color = achievement.IsUnlocked ? unlockedColor : lockedColor;
            }

            if(checker != null)
            {
                if (achievement.IsUnlocked)
                {
					checker.sprite = unlockedSprite;
				}
                else
                {
					checker.color = !achievement.IsUnlocked ? lockedColor : checker.color;
				}
            }

            if (backgroundImage != null)
            {
                Color bgColor = backgroundImage.color;
                bgColor.a = achievement.IsUnlocked ? 1f : 0.5f;
                backgroundImage.color = bgColor;
            }
        }
    }
}
