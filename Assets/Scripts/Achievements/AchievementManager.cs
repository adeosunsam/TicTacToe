using System;
using System.Collections.Generic;
using UnityEngine;
using TicTacToe.Core;
using TicTacToe.AI;

namespace TicTacToe.Achievements
{
    public class AchievementManager
    {
        private readonly Dictionary<AchievementType, Achievement> achievements;
        private const string ACHIEVEMENT_KEY_PREFIX = "Achievement_";
        
        // Stats tracking
        private int currentWinStreak;
        private int friendGamesPlayed;
        private bool opponentGotTwoInRow;
        
        // Pending notifications for achievements unlocked in the previous session
        private Queue<Achievement> pendingNotifications = new Queue<Achievement>();

        public event Action<Achievement> OnAchievementUnlocked;

        public AchievementManager()
        {
            achievements = new Dictionary<AchievementType, Achievement>
            {
                { AchievementType.FirstVictory, new Achievement(
                    AchievementType.FirstVictory, 
                    "First Victory", 
                    "Win your first game") },
                    
                { AchievementType.AIConqueror, new Achievement(
                    AchievementType.AIConqueror, 
                    "AI Conqueror", 
                    "Defeat the Hard AI") },
                    
                { AchievementType.WinStreak3, new Achievement(
                    AchievementType.WinStreak3, 
                    "Hat Trick", 
                    "Win 3 games in a row") },
                    
                { AchievementType.FriendlyRivalry, new Achievement(
                    AchievementType.FriendlyRivalry, 
                    "Friendly Rivalry", 
                    "Play 10 games against a friend") },
                    
                { AchievementType.PerfectVictory, new Achievement(
                    AchievementType.PerfectVictory, 
                    "Perfect Victory", 
                    "Win without opponent getting 2 in a row") }
            };

            LoadAchievements();
            LoadStats();
        }

        public void OnGameStarted(GameMode gameMode)
        {
            opponentGotTwoInRow = false;
            
            if (gameMode == GameMode.PlayerVsPlayer)
            {
                friendGamesPlayed++;
                SaveStats();
                CheckFriendlyRivalry();
            }
        }

        public void OnPlayerWin(int player, GameMode gameMode, AIDifficulty aiDifficulty, IGameBoard board)
        {
            Debug.Log($"[AchievementManager] OnPlayerWin called: player={player}, gameMode={gameMode}");
            
            // Only count player wins (player is always X in bot mode)
            bool isPlayerWin = gameMode == GameMode.PlayerVsBot ? player == 1 : true;
            
            if (isPlayerWin)
            {
                currentWinStreak++;
                SaveStats();
                
                // Check achievements
                CheckFirstVictory();
                CheckWinStreak();
                
                if (gameMode == GameMode.PlayerVsBot && aiDifficulty == AIDifficulty.Hard)
                {
                    CheckAIConqueror();
                }
                
                if (!opponentGotTwoInRow)
                {
                    CheckPerfectVictory();
                }
            }
            else
            {
                currentWinStreak = 0;
                SaveStats();
            }
        }

        public void OnPlayerLoss()
        {
            currentWinStreak = 0;
            SaveStats();
        }

        public void OnDraw()
        {
            currentWinStreak = 0;
            SaveStats();
        }

        public void OnCellPlayed(IGameBoard board)
        {
            // Check if opponent has 2 in a row
            if (!opponentGotTwoInRow)
            {
                opponentGotTwoInRow = HasTwoInRow(board);
            }
        }

        private bool HasTwoInRow(IGameBoard board)
        {
            // Check rows, columns, and diagonals for 2 in a row
            int[,] lines = {
                // Rows
                {0, 1, 2}, {3, 4, 5}, {6, 7, 8},
                // Columns
                {0, 3, 6}, {1, 4, 7}, {2, 5, 8},
                // Diagonals
                {0, 4, 8}, {2, 4, 6}
            };

            for (int i = 0; i < lines.GetLength(0); i++)
            {
                int pos1 = lines[i, 0];
                int pos2 = lines[i, 1];
                int pos3 = lines[i, 2];

                int cell1 = board.GetCell(pos1);
                int cell2 = board.GetCell(pos2);
                int cell3 = board.GetCell(pos3);

                // Count non-empty cells of the same player
                if (cell1 != 0 && (cell1 == cell2 || cell1 == cell3))
                {
                    int count = (cell1 == cell2 ? 1 : 0) + (cell1 == cell3 ? 1 : 0) + 1;
                    if (count >= 2)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void CheckFirstVictory()
        {
            Debug.Log("[AchievementManager] Checking FirstVictory achievement");
            TryUnlockAchievement(AchievementType.FirstVictory);
        }

        private void CheckAIConqueror()
        {
            TryUnlockAchievement(AchievementType.AIConqueror);
        }

        private void CheckWinStreak()
        {
            if (currentWinStreak >= 3)
            {
                TryUnlockAchievement(AchievementType.WinStreak3);
            }
        }

        private void CheckFriendlyRivalry()
        {
            if (friendGamesPlayed >= 10)
            {
                TryUnlockAchievement(AchievementType.FriendlyRivalry);
            }
        }

        private void CheckPerfectVictory()
        {
            TryUnlockAchievement(AchievementType.PerfectVictory);
        }

        private void TryUnlockAchievement(AchievementType type)
        {
            if (achievements.TryGetValue(type, out Achievement achievement))
            {
                if (!achievement.IsUnlocked)
                {
                    Debug.Log($"[AchievementManager] Unlocking achievement: {type}");
                    achievement.Unlock();
                    SaveAchievement(type);
                    pendingNotifications.Enqueue(achievement);
                    Debug.Log($"[AchievementManager] Pending notifications count: {pendingNotifications.Count}");
                    OnAchievementUnlocked?.Invoke(achievement);
                }
                else
                {
                    Debug.Log($"[AchievementManager] Achievement already unlocked: {type}");
                }
            }
        }
        
        public bool HasPendingNotifications()
        {
            bool hasPending = pendingNotifications.Count > 0;
            Debug.Log($"[AchievementManager] HasPendingNotifications: {hasPending}, Count: {pendingNotifications.Count}");
            return hasPending;
        }
        
        public Achievement GetNextPendingNotification()
        {
            var achievement = pendingNotifications.Count > 0 ? pendingNotifications.Dequeue() : null;
            Debug.Log($"[AchievementManager] GetNextPendingNotification: {achievement?.Title ?? "null"}, Remaining: {pendingNotifications.Count}");
            return achievement;
        }
        
        public void ClearPendingNotifications()
        {
            pendingNotifications.Clear();
        }

        public Achievement GetAchievement(AchievementType type)
        {
            return achievements.TryGetValue(type, out Achievement achievement) ? achievement : null;
        }

        public List<Achievement> GetAllAchievements()
        {
            return new List<Achievement>(achievements.Values);
        }

        public int GetUnlockedCount()
        {
            int count = 0;
            foreach (var achievement in achievements.Values)
            {
                if (achievement.IsUnlocked) count++;
            }
            return count;
        }

        public void ResetAllAchievements()
        {
            Debug.Log("[AchievementManager] Resetting all achievements...");
            
            foreach (var achievement in achievements.Values)
            {
                achievement.Lock();
                SaveAchievement(achievement.Type);
            }
            
            currentWinStreak = 0;
            friendGamesPlayed = 0;
            opponentGotTwoInRow = false;
            pendingNotifications.Clear();
            SaveStats();
            
            Debug.Log("[AchievementManager] All achievements reset complete!");
        }

        private void SaveAchievement(AchievementType type)
        {
            string key = ACHIEVEMENT_KEY_PREFIX + type.ToString();
            PlayerPrefs.SetInt(key, achievements[type].IsUnlocked ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadAchievements()
        {
            foreach (var kvp in achievements)
            {
                string key = ACHIEVEMENT_KEY_PREFIX + kvp.Key.ToString();
                if (PlayerPrefs.GetInt(key, 0) == 1)
                {
                    kvp.Value.Unlock();
                }
            }
        }

        private void SaveStats()
        {
            PlayerPrefs.SetInt("WinStreak", currentWinStreak);
            PlayerPrefs.SetInt("FriendGamesPlayed", friendGamesPlayed);
            PlayerPrefs.Save();
        }

        private void LoadStats()
        {
            currentWinStreak = PlayerPrefs.GetInt("WinStreak", 0);
            friendGamesPlayed = PlayerPrefs.GetInt("FriendGamesPlayed", 0);
        }
    }
}
