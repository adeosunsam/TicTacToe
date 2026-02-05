using System;
using System.Collections;
using UnityEngine;
using TicTacToe.Game;

namespace TicTacToe.UI
{
    [DisallowMultipleComponent]
    public class GamePresenter : MonoBehaviour
    {
        [Header("View")]
        [SerializeField]
        private GameView gameView;

        [Header("Player Symbols")]
        [SerializeField]
        private Sprite xSprite;
        
        [SerializeField]
        private Sprite oSprite;

        [Header("Settings")]
        [SerializeField]
        [Range(0.5f, 10f)]
        private float resetDelay = 2f;

        private const int PLAYER_X = 1;
        private const int PLAYER_O = 2;

        private GameController gameController;
        private Coroutine resetCoroutine;

        private void Awake()
        {
            ValidateReferences();
            InitializeGame();
        }

        private void OnDestroy()
        {
            UnsubscribeFromGameEvents();
            
            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }
        }

        private void ValidateReferences()
        {
            if (gameView == null)
            {
                Debug.LogError("[GamePresenter] GameView reference is missing!", this);
            }

            if (xSprite == null)
            {
                Debug.LogWarning("[GamePresenter] X Sprite is not assigned!", this);
            }

            if (oSprite == null)
            {
                Debug.LogWarning("[GamePresenter] O Sprite is not assigned!", this);
            }
        }

        private void InitializeGame()
        {
            var board = new Core.GameBoard();
            var winChecker = new Core.WinChecker();
            var scoreManager = new Core.ScoreManager(new[] { PLAYER_X, PLAYER_O });
            var gameState = new Core.GameState(new[] { PLAYER_X, PLAYER_O });

            gameController = new GameController(board, winChecker, scoreManager, gameState);

            SubscribeToGameEvents();
            gameView.Initialize(OnCellClicked);
            
            UpdateStatusForCurrentPlayer();
            UpdateScores();
        }

        private void SubscribeToGameEvents()
        {
            if (gameController == null) return;

            gameController.OnCellPlayed += HandleCellPlayed;
            gameController.OnWin += HandleWin;
            gameController.OnDraw += HandleDraw;
            gameController.OnPlayerChanged += HandlePlayerChanged;
            gameController.OnBoardReset += HandleBoardReset;
        }

        private void UnsubscribeFromGameEvents()
        {
            if (gameController == null) return;

            gameController.OnCellPlayed -= HandleCellPlayed;
            gameController.OnWin -= HandleWin;
            gameController.OnDraw -= HandleDraw;
            gameController.OnPlayerChanged -= HandlePlayerChanged;
            gameController.OnBoardReset -= HandleBoardReset;
        }

        private void OnCellClicked(int cellIndex)
        {
            gameController?.TryPlayCell(cellIndex);
        }

        private void HandleCellPlayed(int cellIndex, int player)
        {
            Sprite sprite = player == PLAYER_X ? xSprite : oSprite;
            gameView.UpdateCell(cellIndex, sprite);
        }

        private void HandleWin(int player, int winLine)
        {
            string playerName = GetPlayerName(player);
            gameView.UpdateStatusText($"Player {playerName} Wins!");
            gameView.ShowStrikeLine(winLine);
            UpdateScores();
            
            resetCoroutine = StartCoroutine(ResetAfterDelay());
        }

        private void HandleDraw()
        {
            gameView.UpdateStatusText("It's a Draw!");
            resetCoroutine = StartCoroutine(ResetAfterDelay());
        }

        private void HandlePlayerChanged(int currentPlayer)
        {
            UpdateStatusForCurrentPlayer();
        }

        private void HandleBoardReset()
        {
            gameView.ClearAllCells();
            gameView.HideStrikeLine();
            UpdateStatusForCurrentPlayer();
        }


        private void UpdateStatusForCurrentPlayer()
        {
            if (gameController == null) return;

            int player = gameController.GetCurrentPlayer();
            string playerName = GetPlayerName(player);
            gameView.UpdateStatusText($"Player {playerName}'s Turn");
        }

        private void UpdateScores()
        {
            if (gameController == null) return;

            int xScore = gameController.GetScore(PLAYER_X);
            int oScore = gameController.GetScore(PLAYER_O);
            gameView.UpdateScoreText($"X: {xScore}  |  O: {oScore}");
        }

        private string GetPlayerName(int player)
        {
            return player == PLAYER_X ? "X" : "O";
        }

        private IEnumerator ResetAfterDelay()
        {
            yield return new WaitForSeconds(resetDelay);
            resetCoroutine = null;
            gameController?.ResetBoard();
        }

        public void OnResetButtonClicked()
        {
            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
                resetCoroutine = null;
            }

            gameController?.ResetBoard();
        }
    }
}
