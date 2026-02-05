namespace TicTacToe.AI
{
    /// <summary>
    /// Interface for AI players that can calculate moves.
    /// </summary>
    public interface IAIPlayer
    {
        /// <summary>
        /// Calculates the best move for the AI player.
        /// </summary>
        /// <param name="board">Current board state</param>
        /// <param name="aiPlayer">AI player ID</param>
        /// <param name="humanPlayer">Human player ID</param>
        /// <returns>Index of the cell to play (0-8), or -1 if no valid move</returns>
        int CalculateMove(Core.IGameBoard board, int aiPlayer, int humanPlayer);
    }
}
