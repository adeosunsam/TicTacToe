using System;
using TicTacToe.Core;

namespace TicTacToe.Game
{
    /// <summary>
    /// Extended game controller that supports AI opponents.
    /// Manages both player vs player and player vs bot game modes.
    /// </summary>
    public class AIGameController : GameController
    {
        #region Private Fields

        private readonly AI.IAIPlayer _aiPlayer;
        private readonly GameMode _gameMode;
        private readonly int _aiPlayerID;
        private readonly int _humanPlayerID;

        #endregion

        #region Events

        /// <summary>
        /// Event fired when AI is calculating a move.
        /// </summary>
        public event Action OnAIThinking;

        /// <summary>
        /// Event fired when AI has completed its move.
        /// </summary>
        public event Action<int> OnAIMoveCompleted;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new AI-enabled game controller.
        /// </summary>
        /// <param name="board">Game board</param>
        /// <param name="winChecker">Win condition checker</param>
        /// <param name="scoreManager">Score manager</param>
        /// <param name="gameState">Game state</param>
        /// <param name="gameMode">Current game mode</param>
        /// <param name="aiPlayer">AI player implementation (required for PlayerVsBot mode)</param>
        /// <param name="humanPlayerID">ID of the human player (default: 1 for X)</param>
        /// <param name="aiPlayerID">ID of the AI player (default: 2 for O)</param>
        public AIGameController(
            IGameBoard board,
            IWinChecker winChecker,
            IScoreManager scoreManager,
            GameState gameState,
            GameMode gameMode,
            AI.IAIPlayer aiPlayer = null,
            int humanPlayerID = 1,
            int aiPlayerID = 2)
            : base(board, winChecker, scoreManager, gameState)
        {
            _gameMode = gameMode;
            _aiPlayer = aiPlayer;
            _humanPlayerID = humanPlayerID;
            _aiPlayerID = aiPlayerID;

            if (_gameMode == GameMode.PlayerVsBot && _aiPlayer == null)
            {
                throw new ArgumentNullException(nameof(aiPlayer), "AI player is required for PlayerVsBot mode");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts to play a cell. If successful and it's AI's turn, triggers AI move.
        /// </summary>
        public override bool TryPlayCell(int cellIndex)
        {
            bool success = base.TryPlayCell(cellIndex);

            if (success && _gameMode == GameMode.PlayerVsBot && IsGameActive())
            {
                // Check if it's AI's turn
                if (GetCurrentPlayer() == _aiPlayerID)
                {
                    OnAIThinking?.Invoke();
                }
            }

            return success;
        }

        /// <summary>
        /// Executes the AI move calculation and plays the best move.
        /// </summary>
        /// <returns>True if AI successfully made a move, false otherwise</returns>
        public bool ExecuteAIMove()
        {
            if (_gameMode != GameMode.PlayerVsBot) return false;
            if (!IsGameActive()) return false;
            if (GetCurrentPlayer() != _aiPlayerID) return false;
            if (_aiPlayer == null) return false;

            // Calculate best move
            int move = _aiPlayer.CalculateMove(
                GetBoardForAI(),
                _aiPlayerID,
                _humanPlayerID);

            if (move < 0 || move > 8) return false;

            // Execute the move
            bool success = base.TryPlayCell(move);

            if (success)
            {
                OnAIMoveCompleted?.Invoke(move);
            }

            return success;
        }

        /// <summary>
        /// Gets the current game mode.
        /// </summary>
        public GameMode GetGameMode()
        {
            return _gameMode;
        }

        /// <summary>
        /// Checks if current player is AI.
        /// </summary>
        public bool IsAITurn()
        {
            return _gameMode == GameMode.PlayerVsBot && GetCurrentPlayer() == _aiPlayerID;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Provides access to the game board for AI calculations.
        /// </summary>
        protected IGameBoard GetBoardForAI()
        {
            return _board;
        }

        #endregion
    }
}
