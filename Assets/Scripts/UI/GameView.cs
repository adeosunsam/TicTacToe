using System;
using UnityEngine;
using TMPro;

namespace TicTacToe.UI
{
    [DisallowMultipleComponent]
    public class GameView : MonoBehaviour, IGameView
    {
        [Header("Cells")]
        [SerializeField]
        private TicTacToeCell[] cells = new TicTacToeCell[9];

        [Header("UI Elements")]
        [SerializeField]
        private TextMeshProUGUI statusText;
        
        [SerializeField]
        private TextMeshProUGUI hostScoreText, visitorScoreText;

        [Header("Strike Lines")]
        [SerializeField]
        private StrikeLineController strikeLineController;

        public void Initialize(Action<int> onCellClicked)
        {
            if (cells == null || cells.Length == 0)
            {
                return;
            }

            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i] == null)
                {
                    Debug.LogError($"[GameView] Cell at index {i} is null!", this);
                    continue;
                }

                int index = i;
                cells[i].Initialize(index, onCellClicked);
            }
        }

        public void UpdateCell(int cellIndex, Sprite sprite)
        {
            if (!IsValidCellIndex(cellIndex))
            {
                return;
            }

            cells[cellIndex].SetSymbol(sprite);
        }

        public void ClearCell(int cellIndex)
        {
            if (!IsValidCellIndex(cellIndex))
            {
                return;
            }

            cells[cellIndex].Clear();
        }

        public void ClearAllCells()
        {
            if (cells == null) return;

            foreach (var cell in cells)
            {
                cell?.Clear();
            }
        }

        public void ShowStrikeLine(int winLine)
        {
            if (strikeLineController != null)
            {
                strikeLineController.ShowLine(winLine);
            }
        }

        public void HideStrikeLine()
        {
            strikeLineController?.HideAll();
        }

        public void UpdateStatusText(string text)
        {
            if (statusText != null)
            {
                statusText.text = text;
            }
        }

        public void UpdateScoreText(int xScore, int oScore)
        {
            if(hostScoreText != null)
            {
                hostScoreText.text = $"{xScore}";
            }
            if(visitorScoreText != null)
            {
				visitorScoreText.text = $"{oScore}";
			}
        }

        private bool IsValidCellIndex(int index)
        {
            return cells != null && index >= 0 && index < cells.Length;
        }
    }
}
