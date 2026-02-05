namespace TicTacToe.Core
{
    public interface IWinChecker
    {
        bool CheckWin(IGameBoard board, int player, out int winLine);
        bool CheckDraw(IGameBoard board);
    }
}
