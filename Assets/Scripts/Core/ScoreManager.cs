using System.Collections.Generic;

namespace TicTacToe.Core
{
    public class ScoreManager : IScoreManager
    {
        private readonly Dictionary<int, int> scores = new ();

        public ScoreManager(int[] players)
        {
            foreach (int player in players)
            {
                scores[player] = 0;
            }
        }

        public int GetScore(int player)
        {
            return scores.ContainsKey(player) ? scores[player] : 0;
        }

        public void AddScore(int player)
        {
            if (scores.ContainsKey(player))
            {
                scores[player]++;
            }
        }

        public void ResetScores()
        {
            foreach (var key in new List<int>(scores.Keys))
            {
                scores[key] = 0;
            }
        }
    }
}
