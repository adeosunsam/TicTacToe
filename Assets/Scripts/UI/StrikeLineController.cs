using UnityEngine;

namespace TicTacToe.UI
{
    [DisallowMultipleComponent]
    public class StrikeLineController : MonoBehaviour
    {
        [Header("Strike Line GameObjects")]
        [SerializeField]
        private GameObject horizontalLine;
        
        [SerializeField]
        private GameObject verticalLine;
        
        [SerializeField]
        private GameObject diagonalLine1;
        
        [SerializeField]
        private GameObject diagonalLine2;

        [Header("Layout Settings")]
        [SerializeField]
        [Range(50f, 500f)]
        private float cellSize = 180f;
        
        [SerializeField]
        [Range(150f, 1500f)]
        private float boardSize = 540f;
        
        [SerializeField]
        [Range(5f, 50f)]
        private float lineThickness = 20f;

        private const int HORIZONTAL_START = 0;
        private const int HORIZONTAL_END = 2;
        private const int VERTICAL_START = 3;
        private const int VERTICAL_END = 5;
        private const int DIAGONAL_MAIN = 6;
        private const int DIAGONAL_ANTI = 7;
        private const float DIAGONAL_LENGTH_MULTIPLIER = 1.414f;
        private const float DIAGONAL_MAIN_ROTATION = 45f;
        private const float DIAGONAL_ANTI_ROTATION = -45f;

        public void ShowLine(int winLine)
        {
            HideAll();

            GameObject lineToShow = GetLineObject(winLine);
            
            if (lineToShow != null)
            {
                lineToShow.SetActive(true);
                PositionLine(lineToShow, winLine);
            }
        }

        public void HideAll()
        {
            if (horizontalLine) horizontalLine.SetActive(false);
            if (verticalLine) verticalLine.SetActive(false);
            if (diagonalLine1) diagonalLine1.SetActive(false);
            if (diagonalLine2) diagonalLine2.SetActive(false);
        }

        private GameObject GetLineObject(int winLine)
        {
            if (winLine >= HORIZONTAL_START && winLine <= HORIZONTAL_END)
            {
                return horizontalLine;
            }
            else if (winLine >= VERTICAL_START && winLine <= VERTICAL_END)
            {
                return verticalLine;
            }
            else if (winLine == DIAGONAL_MAIN)
            {
                return diagonalLine1;
            }
            else if (winLine == DIAGONAL_ANTI)
            {
                return diagonalLine2;
            }

            return null;
        }

        private void PositionLine(GameObject line, int winLine)
        {
            RectTransform rectTransform = line.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                return;
            }

            if (winLine >= HORIZONTAL_START && winLine <= HORIZONTAL_END)
            {
                PositionHorizontalLine(rectTransform, winLine);
            }
            else if (winLine >= VERTICAL_START && winLine <= VERTICAL_END)
            {
                PositionVerticalLine(rectTransform, winLine);
            }
            else if (winLine == DIAGONAL_MAIN || winLine == DIAGONAL_ANTI)
            {
                PositionDiagonalLine(rectTransform, winLine);
            }
        }

        private void PositionHorizontalLine(RectTransform rectTransform, int winLine)
        {
            rectTransform.anchoredPosition = new Vector2(0f, -cellSize * (winLine - 1));
            rectTransform.sizeDelta = new Vector2(boardSize, lineThickness);
            rectTransform.localRotation = Quaternion.identity;
        }

        private void PositionVerticalLine(RectTransform rectTransform, int winLine)
        {
            rectTransform.anchoredPosition = new Vector2(cellSize * (winLine - 4), 0f);
            rectTransform.sizeDelta = new Vector2(lineThickness, boardSize);
            rectTransform.localRotation = Quaternion.identity;
        }

        private void PositionDiagonalLine(RectTransform rectTransform, int winLine)
        {
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(lineThickness, boardSize * DIAGONAL_LENGTH_MULTIPLIER);
            
            float rotation = winLine == DIAGONAL_MAIN ? DIAGONAL_MAIN_ROTATION : DIAGONAL_ANTI_ROTATION;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotation);
        }
    }
}
