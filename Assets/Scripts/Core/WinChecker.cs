namespace TicTacToe.Core
{
    public class WinChecker : IWinChecker
    {
        private readonly int[,] winPatterns = {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Rows
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Columns
            {0, 4, 8}, {2, 4, 6}             // Diagonals
        };

        public bool CheckWin(IGameBoard board, int player, out int winLine)
        {
            for (int i = 0; i < winPatterns.GetLength(0); i++)
            {
                int a = winPatterns[i, 0];
                int b = winPatterns[i, 1];
                int c = winPatterns[i, 2];

                if (board.GetCell(a) == player &&
                    board.GetCell(b) == player &&
                    board.GetCell(c) == player)
                {
                    winLine = i;
                    return true;
                }
            }

            winLine = -1;
            return false;
        }

        public bool CheckDraw(IGameBoard board)
        {
            int[] state = board.GetBoardState();
            foreach (int cell in state)
            {
                if (cell == 0) return false;
            }
            return true;
        }
    }
}
