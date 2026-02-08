using System;
using TicTacToe.Core;

namespace TicTacToe.AI
{
    public class MinimaxAI : IAIPlayer
    {
        private const int WIN_SCORE = 10;
        private const int LOSE_SCORE = -10;
        private const int DRAW_SCORE = 0;
        
        // Strategic position weights for tie-breaking
        private static readonly int[] POSITION_WEIGHTS = new int[]
        {
            3, 2, 3,// Corners are valuable
            2, 4, 2,// Center is most valuable
            3, 2, 3// Corners are valuable
        };

        private readonly IWinChecker _winChecker;
        private readonly Random _random;
        private readonly AIDifficulty _difficulty;
        public MinimaxAI(IWinChecker winChecker, AIDifficulty difficulty = AIDifficulty.Hard)
        {
            _winChecker = winChecker;
            _difficulty = difficulty;
            _random = new Random();
        }

        public int CalculateMove(IGameBoard board, int aiPlayer, int humanPlayer)
        {
            if (board == null) return -1;

            // Apply difficulty-based randomization
            if (ShouldMakeRandomMove())
            {
                return GetRandomMove(board);
            }

            // First move optimization - always take center or corner
            if (IsFirstMove(board))
            {
                // If center is available, take it (optimal opening)
                if (board.IsCellEmpty(4))
                {
                    return 4;
                }
                // Otherwise take a corner (also strong)
                int[] corners = { 0, 2, 6, 8 };
                foreach (int corner in corners)
                {
                    if (board.IsCellEmpty(corner))
                    {
                        return corner;
                    }
                }
            }

            int bestMove = -1;
            int bestScore = int.MinValue;
            var bestMoves = new System.Collections.Generic.List<int>();

            // Try all possible moves
            for (int i = 0; i < 9; i++)
            {
                if (board.IsCellEmpty(i))
                {
                    // Make move
                    board.SetCell(i, aiPlayer);

                    // Calculate score
                    int score = Minimax(board, 0, false, aiPlayer, humanPlayer, int.MinValue, int.MaxValue);

                    // Undo move
                    board.SetCell(i, 0);

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = i;
                        bestMoves.Clear();
                        bestMoves.Add(i);
                    }
                    else if (score == bestScore)
                    {
                        bestMoves.Add(i);
                    }
                }
            }

            if (bestMoves.Count > 1)
            {
                bestMove = GetBestStrategicMove(bestMoves);
            }

            return bestMove;
        }

        private int Minimax(IGameBoard board, int depth, bool isMaximizing, int aiPlayer, int humanPlayer, int alpha, int beta)
        {
            if (_winChecker.CheckWin(board, aiPlayer, out _))
            {
                return WIN_SCORE - depth;// Prefer faster wins
            }

            if (_winChecker.CheckWin(board, humanPlayer, out _))
            {
                return LOSE_SCORE + depth;// Delay losses
            }

            if (_winChecker.CheckDraw(board))
            {
                return DRAW_SCORE;
            }

            if (isMaximizing)
            {
                int maxScore = int.MinValue;

                for (int i = 0; i < 9; i++)
                {
                    if (board.IsCellEmpty(i))
                    {
                        board.SetCell(i, aiPlayer);
                        int score = Minimax(board, depth + 1, false, aiPlayer, humanPlayer, alpha, beta);
                        board.SetCell(i, 0);

                        maxScore = Math.Max(maxScore, score);
                        alpha = Math.Max(alpha, score);

                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

                return maxScore;
            }
            else
            {
                int minScore = int.MaxValue;

                for (int i = 0; i < 9; i++)
                {
                    if (board.IsCellEmpty(i))
                    {
                        board.SetCell(i, humanPlayer);
                        int score = Minimax(board, depth + 1, true, aiPlayer, humanPlayer, alpha, beta);
                        board.SetCell(i, 0);

                        minScore = Math.Min(minScore, score);
                        beta = Math.Min(beta, score);

                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }

                return minScore;
            }
        }

        private bool ShouldMakeRandomMove()
        {
            double randomChance = _difficulty switch
            {
                AIDifficulty.Easy => 0.6,// 60% random moves
                AIDifficulty.Medium => 0.3,// 30% random moves
                AIDifficulty.Hard => 0.1, // 10% random moves
                _ => 0.0
            };

            return _random.NextDouble() < randomChance;
        }

        private int GetRandomMove(IGameBoard board)
        {
            var availableMoves = new System.Collections.Generic.List<int>();

            for (int i = 0; i < 9; i++)
            {
                if (board.IsCellEmpty(i))
                {
                    availableMoves.Add(i);
                }
            }

            if (availableMoves.Count == 0) return -1;

            int randomIndex = _random.Next(availableMoves.Count);
            return availableMoves[randomIndex];
        }

        private bool IsFirstMove(IGameBoard board)
        {
            for (int i = 0; i < 9; i++)
            {
                if (!board.IsCellEmpty(i))
                {
                    return false;
                }
            }
            return true;
        }

        private int GetBestStrategicMove(System.Collections.Generic.List<int> moves)
        {
            int bestMove = moves[0];
            int bestWeight = POSITION_WEIGHTS[bestMove];

            foreach (int move in moves)
            {
                if (POSITION_WEIGHTS[move] > bestWeight)
                {
                    bestWeight = POSITION_WEIGHTS[move];
                    bestMove = move;
                }
            }

            return bestMove;
        }
    }

    public enum AIDifficulty
    {
        Easy,
        Medium,
        Hard
    }
}
