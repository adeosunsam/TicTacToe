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
        private GameMode gameMode = GameMode.PlayerVsPlayer;

        [SerializeField]
        private AIDifficulty aiDifficulty = AIDifficulty.Hard;

        private const int PLAYER_X = 1;
        private const int PLAYER_O = 2;

        private AIGameController gameController;
        private Coroutine resetCoroutine;
        private Coroutine aiMoveCoroutine;
        private bool isInitialized;

        private void OnDestroy()
        {
            UnsubscribeFromGameEvents();
            
            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }
        }

        private void InitializeGame()
        {
            var board = new GameBoard();
            var winChecker = new WinChecker();
            var scoreManager = new ScoreManager(new[] { PLAYER_X, PLAYER_O });
            var gameState = new GameState(new[] { PLAYER_X, PLAYER_O });

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
			bool isXTurn = player == PLAYER_X;

			if (gameMode == GameMode.PlayerVsBot)
			{
                if (isXTurn)
                {
                    gameView.UpdateStatusText("You win");
                }
                else
                {
                    gameView.UpdateStatusText("Bot wins");
                }
			}
            else if(gameMode == GameMode.PlayerVsPlayer)
            {
				if (isXTurn)
				{
					gameView.UpdateStatusText("Player 1 wins");
				}
				else
				{
					gameView.UpdateStatusText("Player 2 wins");
				}
			}
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
            
            bool isXTurn = player == PLAYER_X;

			if (gameMode == GameMode.PlayerVsBot)
			{
				if (isXTurn)
				{
					gameView.UpdateStatusText("Your turn");
                    return;
				}
				gameView.UpdateStatusText("Bot thinking");
			}
            else if(gameMode == GameMode.PlayerVsPlayer)
            {
				if (isXTurn)
				{
					gameView.UpdateStatusText("Player 1 turn");
					return;
				}
				gameView.UpdateStatusText("Player 2 turn");
			}
        }

        private void UpdateScores()
        {
            if (gameController == null) return;

            int xScore = gameController.GetScore(PLAYER_X);
            int oScore = gameController.GetScore(PLAYER_O);
            gameView.UpdateScoreText(xScore, oScore);
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
            if (!isInitialized)
            {
                Debug.LogWarning("[GamePresenter] Cannot reset - game not initialized yet!");
                return;
            }

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

        /// <summary>
        /// Initializes the game with specified mode and difficulty.
        /// Call this from StartPageController when starting a new game.
        /// </summary>
        public void InitializeWithSettings(GameMode mode, AIDifficulty difficulty)
        {
            UnsubscribeFromGameEvents();
            gameMode = mode;
            aiDifficulty = difficulty;
            InitializeGame();
            isInitialized = true;
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
