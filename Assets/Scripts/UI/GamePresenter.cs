using System;
using System.Collections;
using UnityEngine;
using TicTacToe.Game;
using TicTacToe.Core;
using TicTacToe.AI;

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

        [SerializeField]
        [Range(0.1f, 3f)]
        private float aiMoveDelay = 0.5f;

        [Header("Game Mode")]
        [SerializeField]
        private Core.GameMode gameMode = Core.GameMode.PlayerVsPlayer;

        [SerializeField]
        private AI.AIDifficulty aiDifficulty = AI.AIDifficulty.Hard;

        private const int PLAYER_X = 1;
        private const int PLAYER_O = 2;

        private AIGameController gameController;
        private Coroutine resetCoroutine;
        private Coroutine aiMoveCoroutine;

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

            // Create AI player if in bot mode
            AI.IAIPlayer aiPlayer = null;
            if (gameMode == Core.GameMode.PlayerVsBot)
            {
                aiPlayer = new AI.MinimaxAI(winChecker, aiDifficulty);
            }

            gameController = new AIGameController(
                board,
                winChecker,
                scoreManager,
                gameState,
                gameMode,
                aiPlayer,
                PLAYER_X,
                PLAYER_O);

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
            gameController.OnAIThinking += HandleAIThinking;
            gameController.OnAIMoveCompleted += HandleAIMoveCompleted;
        }

        private void UnsubscribeFromGameEvents()
        {
            if (gameController == null) return;

            gameController.OnCellPlayed -= HandleCellPlayed;
            gameController.OnWin -= HandleWin;
            gameController.OnDraw -= HandleDraw;
            gameController.OnPlayerChanged -= HandlePlayerChanged;
            gameController.OnBoardReset -= HandleBoardReset;
            gameController.OnAIThinking -= HandleAIThinking;
            gameController.OnAIMoveCompleted -= HandleAIMoveCompleted;
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

            // Check if AI should start (if it's AI's turn after reset)
            if (gameController.IsAITurn())
            {
                aiMoveCoroutine = StartCoroutine(ExecuteAIMoveWithDelay());
            }
        }

        private void HandleAIThinking()
        {
            // AI is thinking, trigger move after delay
            aiMoveCoroutine = StartCoroutine(ExecuteAIMoveWithDelay());
        }

        private void HandleAIMoveCompleted(int cellIndex)
        {
            // AI move completed, UI already updated through OnCellPlayed event
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
            gameView.UpdateScoreText(xScore, oScore);
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

        private IEnumerator ExecuteAIMoveWithDelay()
        {
            yield return new WaitForSeconds(aiMoveDelay);
            aiMoveCoroutine = null;
            gameController?.ExecuteAIMove();
        }

        public void OnResetButtonClicked()
        {
            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
                resetCoroutine = null;
            }

            if (aiMoveCoroutine != null)
            {
                StopCoroutine(aiMoveCoroutine);
                aiMoveCoroutine = null;
            }

            gameController?.ResetBoard();
        }

        public void SetGameMode(GameMode mode)
        {
            UnsubscribeFromGameEvents();
            gameMode = mode;
            InitializeGame();
        }

        public void SetAIDifficulty(AIDifficulty difficulty)
        {
            aiDifficulty = difficulty;
            if (gameMode == GameMode.PlayerVsBot)
            {
                // Reinitialize to apply new difficulty
                SetGameMode(gameMode);
            }
        }
    }
}
