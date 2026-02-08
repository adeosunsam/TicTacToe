using System;
using TicTacToe.AI;
using TicTacToe.Core;

namespace TicTacToe.Game
{
    public class AIGameController : GameController
    {
        private readonly IAIPlayer _aiPlayer;
        private readonly GameMode _gameMode;
        private readonly int _aiPlayerID;
        private readonly int _humanPlayerID;

        public event Action OnAIThinking;

        public event Action<int> OnAIMoveCompleted;

        public AIGameController(IGameBoard board, IWinChecker winChecker, IScoreManager scoreManager,
            GameState gameState, GameMode gameMode, IAIPlayer aiPlayer = null,
            int humanPlayerID = 1, int aiPlayerID = 2) : base(board, winChecker, scoreManager, gameState)
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

        public GameMode GetGameMode()
        {
            return _gameMode;
        }

        public bool IsAITurn()
        {
            return _gameMode == GameMode.PlayerVsBot && GetCurrentPlayer() == _aiPlayerID;
        }

        public IGameBoard GetBoard()
        {
            return _board;
        }

        protected IGameBoard GetBoardForAI()
        {
            return _board;
        }
    }
}
