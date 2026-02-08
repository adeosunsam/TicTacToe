using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace TicTacToe.Achievements
{
    public class AchievementNotification : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        private GameObject notificationPanel;
        
        [SerializeField]
        private TextMeshProUGUI titleText;
        
        [SerializeField]
        private TextMeshProUGUI descriptionText;
        
        [SerializeField]
        private Image iconImage;

        [Header("Animation Settings")]
        [SerializeField]
        private float displayDuration = 3f;
        
        [SerializeField]
        private float slideInDuration = 0.5f;
        
        [SerializeField]
        private float slideOutDuration = 0.3f;

        [Header("Audio")]
        [SerializeField]
        private AudioSource audioSource;

        private RectTransform panelRect;
        private Vector2 hiddenPosition;
        private Vector2 visiblePosition;
        private Coroutine displayCoroutine;

        private void Awake()
        {
            if (notificationPanel != null)
            {
                panelRect = notificationPanel.GetComponent<RectTransform>();
                
                visiblePosition = panelRect.anchoredPosition;
                hiddenPosition = new Vector2(visiblePosition.x, visiblePosition.y + panelRect.rect.height + 100);
                
                panelRect.anchoredPosition = hiddenPosition;
                notificationPanel.SetActive(false);
            }
        }

        public void ShowAchievement(Achievement achievement)
        {
            if (notificationPanel == null || achievement == null)
            {
                return;
            }

            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }

            //activate gameobject before starting coroutine
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }
            
            notificationPanel.SetActive(true);
            displayCoroutine = StartCoroutine(DisplayNotification(achievement));
        }

        private IEnumerator DisplayNotification(Achievement achievement)
        {
            // Update text
            if (titleText != null)
            {
                titleText.text = achievement.Title;
            }
            
            if (descriptionText != null)
            {
                descriptionText.text = achievement.Description;
            }

            // Play sound effect
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Show panel
            notificationPanel.SetActive(true);

            // Slide in
            float elapsed = 0f;
            while (elapsed < slideInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / slideInDuration;
                t = EaseOutBack(t);
                panelRect.anchoredPosition = Vector2.Lerp(hiddenPosition, visiblePosition, t);
                yield return null;
            }
            panelRect.anchoredPosition = visiblePosition;

            // Wait
            yield return new WaitForSeconds(displayDuration);

            // Slide out
            elapsed = 0f;
            while (elapsed < slideOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / slideOutDuration;
                t = EaseInBack(t);
                panelRect.anchoredPosition = Vector2.Lerp(visiblePosition, hiddenPosition, t);
                yield return null;
            }
            panelRect.anchoredPosition = hiddenPosition;

            // Hide panel
            notificationPanel.SetActive(false);
            displayCoroutine = null;
        }

        private float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }

        private float EaseInBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return c3 * t * t * t - c1 * t * t;
        }
    }
}
