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
        private GameObject showcasePanel;
        
        [SerializeField]
        private Transform achievementListContainer;
        
        [SerializeField]
        private GameObject achievementItemPrefab;
        
        [SerializeField]
        private TextMeshProUGUI progressText;
        
        [SerializeField]
        private Button closeButton;

        [Header("Colors")]
        [SerializeField]
        private Color unlockedColor = new Color(1f, 0.84f, 0f); // Gold
        
        [SerializeField]
        private Color lockedColor = new Color(0.5f, 0.5f, 0.5f); // Gray

        private AchievementManager achievementManager;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }
            
            if (showcasePanel != null)
            {
                showcasePanel.SetActive(false);
            }
            else
            {
                Debug.LogError("[AchievementShowcase] Showcase panel is not assigned in the Inspector!");
            }
        }
        
        [ContextMenu("Validate Setup")]
        private void ValidateSetup()
        {
            Debug.Log($"[AchievementShowcase] Validation:\n" +
                     $"  Showcase Panel: {(showcasePanel != null ? "✓" : "✗ MISSING")}\n" +
                     $"  List Container: {(achievementListContainer != null ? "✓" : "✗ MISSING")}\n" +
                     $"  Item Prefab: {(achievementItemPrefab != null ? "✓" : "✗ MISSING")}\n" +
                     $"  Progress Text: {(progressText != null ? "✓" : "✗ MISSING")}\n" +
                     $"  Close Button: {(closeButton != null ? "✓" : "✗ MISSING")}\n" +
                     $"  Achievement Manager Initialized: {(achievementManager != null ? "✓" : "✗ NOT INITIALIZED")}");
        }

        public void Initialize(AchievementManager manager)
        {
            achievementManager = manager;
            Debug.Log($"[AchievementShowcase] Initialize called with manager: {(manager != null ? "Valid" : "NULL")}");
        }

        public void Show()
        {
            if (showcasePanel == null)
            {
                Debug.LogError("[AchievementShowcase] Showcase panel is null!");
                return;
            }
            
            if (achievementManager == null)
            {
                Debug.LogError("[AchievementShowcase] Achievement manager is null! Did you call Initialize()?");
                return;
            }

            Debug.Log("[AchievementShowcase] Showing achievements showcase");
            
            // Activate all parents first
            Transform parent = showcasePanel.transform.parent;
            while (parent != null)
            {
                if (!parent.gameObject.activeSelf)
                {
                    Debug.Log($"[AchievementShowcase] Activating parent: {parent.name}");
                    parent.gameObject.SetActive(true);
                }
                parent = parent.parent;
            }
            
            // Activate panel before populating
            showcasePanel.SetActive(true);
            
            // Ensure panel is brought to front
            showcasePanel.transform.SetAsLastSibling();
            
            // Wait a frame then refresh to ensure UI is ready
            StartCoroutine(RefreshAfterActivation());
        }
        
        private System.Collections.IEnumerator RefreshAfterActivation()
        {
            yield return null; // Wait one frame for UI to initialize
            RefreshAchievementList();
            Debug.Log($"[AchievementShowcase] Panel active: {showcasePanel.activeSelf}, activeInHierarchy: {showcasePanel.activeInHierarchy}");
        }

        public void Hide()
        {
            if (showcasePanel != null)
            {
                showcasePanel.SetActive(false);
            }
        }

        private void RefreshAchievementList()
        {
            // Clear existing items
            foreach (Transform child in achievementListContainer)
            {
                Destroy(child.gameObject);
            }

            // Get all achievements
            List<Achievement> achievements = achievementManager.GetAllAchievements();
            int unlockedCount = achievementManager.GetUnlockedCount();

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
            
            // Find components (assuming structure: Title, Description, Icon/CheckMark)
            TextMeshProUGUI titleText = item.transform.Find("Title")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = item.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
            Image iconImage = item.transform.Find("Icon")?.GetComponent<Image>();
            Image backgroundImage = item.GetComponent<Image>();

            if (titleText != null)
            {
                titleText.text = achievement.Title;
                titleText.color = achievement.IsUnlocked ? unlockedColor : lockedColor;
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

            if (backgroundImage != null)
            {
                Color bgColor = backgroundImage.color;
                bgColor.a = achievement.IsUnlocked ? 1f : 0.5f;
                backgroundImage.color = bgColor;
            }
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
            }
        }
    }
}
