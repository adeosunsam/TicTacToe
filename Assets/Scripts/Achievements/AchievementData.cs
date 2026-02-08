using UnityEngine;

namespace TicTacToe.Achievements
{
    [CreateAssetMenu(menuName = "TicTacToe/Achievement Data")]
    public class AchievementData : ScriptableObject
    {
        [SerializeField]
        private AchievementType type;
        
        [SerializeField]
        private string title;
        
        [SerializeField]
        [TextArea(2, 4)]
        private string description;

        public AchievementType Type => type;
        public string Title => title;
        public string Description => description;
    }
}
