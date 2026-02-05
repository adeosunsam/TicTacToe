using System;
using TicTacToe.Core;

namespace TicTacToe.Game
{
    /// <summary>
    /// Core game controller that manages game flow and state.
    /// Can be extended for AI gameplay support.
    /// </summary>
    public class GameController
    {
        #region Protected Fields

        protected readonly IGameBoard _board;
        protected readonly IWinChecker _winChecker;
        protected readonly IScoreManager _scoreManager;
        protected readonly GameState _gameState;

        #endregion

        #region Events

        #endregion

        #region Events

        public event Action<int, int> OnCellPlayed; // cellIndex, player
        public event Action<int, int> OnWin; // player, winLine
        public event Action OnDraw;
        public event Action<int> OnPlayerChanged; // currentPlayer
        public event Action OnBoardReset;

        #endregion

        #region Constructor

        public GameController(IGameBoard board, IWinChecker winChecker, IScoreManager scoreManager, GameState gameState)
        {
            _board = board ?? throw new ArgumentNullException(nameof(board));
            _winChecker = winChecker ?? throw new ArgumentNullException(nameof(winChecker));
            _scoreManager = scoreManager ?? throw new ArgumentNullException(nameof(scoreManager));
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
        }

        #endregion

        #region Public Methods

        public virtual bool TryPlayCell(int cellIndex)
        {
            if (!_gameState.IsActive) return false;
            if (!_board.IsCellEmpty(cellIndex)) return false;

            int player = _gameState.CurrentPlayer;
            _board.SetCell(cellIndex, player);
            OnCellPlayed?.Invoke(cellIndex, player);

            if (_winChecker.CheckWin(_board, player, out int winLine))
            {
                HandleWin(player, winLine);
                return true;
            }

            if (_winChecker.CheckDraw(_board))
            {
                HandleDraw();
                return true;
            }

            _gameState.SwitchPlayer();
            OnPlayerChanged?.Invoke(_gameState.CurrentPlayer);
            return true;
        }

        public void ResetBoard()
        {
            _board.Clear();
            _gameState.Reset();
            OnBoardReset?.Invoke();
            OnPlayerChanged?.Invoke(_gameState.CurrentPlayer);
        }

        public int GetScore(int player)
        {
            return _scoreManager.GetScore(player);
        }

        public int GetCurrentPlayer()
        {
            return _gameState.CurrentPlayer;
        }

        public bool IsGameActive()
        {
            return _gameState.IsActive;
        }

        public IGameBoard GetBoard()
        {
            return _board;
        }

        #endregion

        #region Protected Methods

        protected void HandleWin(int player, int winLine)
        {
            _gameState.SetWin();
            _scoreManager.AddScore(player);
            OnWin?.Invoke(player, winLine);
        }

        protected void HandleDraw()
        {
            _gameState.SetDraw();
            OnDraw?.Invoke();
        }

        #endregion
    }
}
