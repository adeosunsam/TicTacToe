using UnityEngine;

namespace TicTacToe.UI
{
    public interface IGameView
    {
        void UpdateCell(int cellIndex, Sprite sprite);
        void ClearCell(int cellIndex);
        void ClearAllCells();
        void ShowStrikeLine(int winLine);
        void HideStrikeLine();
        void UpdateStatusText(string text);
        void UpdateScoreText(int xScore, int oScore);
    }
}
