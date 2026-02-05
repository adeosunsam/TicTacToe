using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;
    [SerializeField] private Color xColor = Color.red;
    [SerializeField] private Color oColor = Color.blue;

    [Header("UI References")]
    [SerializeField] private Text statusText;
    [SerializeField] private Button resetButton;
    [SerializeField] private CellButton[] cells;

    private bool isXTurn = true;
    private int moveCount = 0;
    private bool gameEnded = false;
    private int[,] board = new int[3, 3]; // 0 = empty, 1 = X, 2 = O

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
        resetButton.onClick.AddListener(ResetGame);
    }

    private void InitializeGame()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            int row = i / 3;
            int col = i % 3;
            cells[i].Initialize(row, col);
        }
        UpdateStatusText();
    }

    public void OnCellClicked(int row, int col, CellButton cell)
    {
        if (gameEnded || board[row, col] != 0) return;

        // Update board state
        board[row, col] = isXTurn ? 1 : 2;
        moveCount++;

        // Update cell visual
        Sprite currentSprite = isXTurn ? xSprite : oSprite;
        Color currentColor = isXTurn ? xColor : oColor;
        cell.SetSymbol(currentSprite, currentColor);

        // Check for win
        if (CheckWin(row, col))
        {
            EndGame(false);
            return;
        }

        // Check for draw
        if (moveCount >= 9)
        {
            EndGame(true);
            return;
        }

        // Switch turns
        isXTurn = !isXTurn;
        UpdateStatusText();
    }

    private bool CheckWin(int lastRow, int lastCol)
    {
        int player = board[lastRow, lastCol];

        // Check row
        if (board[lastRow, 0] == player && board[lastRow, 1] == player && board[lastRow, 2] == player)
            return true;

        // Check column
        if (board[0, lastCol] == player && board[1, lastCol] == player && board[2, lastCol] == player)
            return true;

        // Check diagonal (top-left to bottom-right)
        if (lastRow == lastCol)
        {
            if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
                return true;
        }

        // Check anti-diagonal (top-right to bottom-left)
        if (lastRow + lastCol == 2)
        {
            if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
                return true;
        }

        return false;
    }

    private void EndGame(bool isDraw)
    {
        gameEnded = true;
        if (isDraw)
        {
            statusText.text = "It's a Draw!";
        }
        else
        {
            string winner = isXTurn ? "X" : "O";
            statusText.text = $"Player {winner} Wins!";
        }
    }

    private void UpdateStatusText()
    {
        if (!gameEnded)
        {
            string currentPlayer = isXTurn ? "X" : "O";
            statusText.text = $"Player {currentPlayer}'s Turn";
        }
    }

    public void ResetGame()
    {
        // Reset game state
        isXTurn = true;
        moveCount = 0;
        gameEnded = false;
        board = new int[3, 3];

        // Reset all cells
        foreach (var cell in cells)
        {
            cell.ResetCell();
        }

        UpdateStatusText();
    }
}
