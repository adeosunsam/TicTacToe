namespace TicTacToe.Core
{
    public class GameBoard : IGameBoard
    {
        private readonly int[] board;
        private readonly int size;

        public GameBoard(int size = 9)
        {
            this.size = size;
            board = new int[size];
        }

        public int GetCell(int index)
        {
            return IsValidIndex(index) ? board[index] : 0;
        }

        public void SetCell(int index, int value)
        {
            if (IsValidIndex(index))
            {
                board[index] = value;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < size; i++)
            {
                board[i] = 0;
            }
        }

        public int[] GetBoardState()
        {
            return (int[])board.Clone();
        }

        public bool IsCellEmpty(int index)
        {
            return IsValidIndex(index) && board[index] == 0;
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < size;
        }
    }
}
