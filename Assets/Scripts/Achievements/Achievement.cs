using System;

namespace TicTacToe.Achievements
{
	[Serializable]
	public class Achievement
	{
		private AchievementData data;

		public AchievementType Type => data.Type;
		public string Title => data.Title;
		public string Description => data.Description;
		public bool IsUnlocked { get; private set; }

		public Achievement(AchievementData achievementData)
		{
			data = achievementData;
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
