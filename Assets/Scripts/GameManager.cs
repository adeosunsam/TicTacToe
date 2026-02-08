using System.Collections.Generic;
using TicTacToe.Achievements;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private List<AchievementData> achievementDataList;
	public static GameManager Instance { get; private set; }

	private AchievementManager achievementManager;

	public AchievementManager AchievementManager => achievementManager;
	void Awake()
    {
        if(Instance == null)
            Instance = this;

		achievementManager = new AchievementManager(achievementDataList ?? new List<AchievementData>());
	}
}
