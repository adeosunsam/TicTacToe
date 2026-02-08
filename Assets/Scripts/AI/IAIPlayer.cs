using TicTacToe.Core;

namespace TicTacToe.AI
{
    public interface IAIPlayer
    {
        int CalculateMove(IGameBoard board, int aiPlayer, int humanPlayer);
    }
}
