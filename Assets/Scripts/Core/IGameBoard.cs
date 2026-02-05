namespace TicTacToe.Core
{
    public interface IGameBoard
    {
        int GetCell(int index);
        void SetCell(int index, int value);
        void Clear();
        int[] GetBoardState();
        bool IsCellEmpty(int index);
    }
}
