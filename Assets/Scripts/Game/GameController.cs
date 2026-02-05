using System;
using TicTacToe.Core;

namespace TicTacToe.Game
{
    public class GameController
    {
        private readonly IGameBoard _board;
        private readonly IWinChecker _winChecker;
        private readonly IScoreManager _scoreManager;
        private readonly GameState _gameState;

        public event Action<int, int> OnCellPlayed; // cellIndex, player
        public event Action<int, int> OnWin; // player, winLine
        public event Action OnDraw;
        public event Action<int> OnPlayerChanged; // currentPlayer
        public event Action OnBoardReset;

        public GameController(IGameBoard board, IWinChecker winChecker, IScoreManager scoreManager, GameState gameState)
        {
            _board = board;
            _winChecker = winChecker;
            _scoreManager = scoreManager;
            _gameState = gameState;
        }

        public bool TryPlayCell(int cellIndex)
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

        private void HandleWin(int player, int winLine)
        {
            _gameState.SetWin();
            _scoreManager.AddScore(player);
            OnWin?.Invoke(player, winLine);
        }

        private void HandleDraw()
        {
            _gameState.SetDraw();
            OnDraw?.Invoke();
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
    }
}
