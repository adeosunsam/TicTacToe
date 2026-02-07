using System;

namespace TicTacToe.Achievements
{
    [Serializable]
    public class Achievement
    {
        public AchievementType Type { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool IsUnlocked { get; private set; }

        public Achievement(AchievementType type, string title, string description)
        {
            Type = type;
            Title = title;
            Description = description;
            IsUnlocked = false;
        }

        public void Unlock()
        {
            IsUnlocked = true;
        }

        public void Lock()
        {
            IsUnlocked = false;
        }
    }
}
