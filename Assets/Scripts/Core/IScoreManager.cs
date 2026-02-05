namespace TicTacToe.Core
{
    public interface IScoreManager
    {
        int GetScore(int player);
        void AddScore(int player);
        void ResetScores();
    }
}
