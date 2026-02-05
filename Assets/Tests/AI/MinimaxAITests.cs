using NUnit.Framework;
using TicTacToe.Core;
using TicTacToe.AI;

namespace TicTacToe.Tests.AI
{
    [TestFixture]
    public class MinimaxAITests
    {
        private IGameBoard _board;
        private IWinChecker _winChecker;
        private IAIPlayer _ai;

        [SetUp]
        public void SetUp()
        {
            _board = new GameBoard();
            _winChecker = new WinChecker();
            _ai = new MinimaxAI(_winChecker, AIDifficulty.Hard);
        }

        [Test]
        public void CalculateMove_EmptyBoard_ReturnsValidMove()
        {
            int move = _ai.CalculateMove(_board, 2, 1);

            Assert.GreaterOrEqual(move, 0);
            Assert.LessOrEqual(move, 8);
        }

        [Test]
        public void CalculateMove_CanWin_TakesWinningMove()
        {
            // O O -
            // - - -
            // - - -
            _board.SetCell(0, 2); // AI
            _board.SetCell(1, 2); // AI

            int move = _ai.CalculateMove(_board, 2, 1);

            Assert.AreEqual(2, move, "AI should complete its winning row");
        }

        [Test]
        public void CalculateMove_OpponentCanWin_BlocksWinningMove()
        {
            // X X -
            // - - -
            // - - -
            _board.SetCell(0, 1); // Human
            _board.SetCell(1, 1); // Human

            int move = _ai.CalculateMove(_board, 2, 1);

            Assert.AreEqual(2, move, "AI should block human's winning move");
        }

        [Test]
        public void CalculateMove_ForkScenario_HandlesCorrectly()
        {
            // X - -
            // - X -
            // - - O
            _board.SetCell(0, 1); // Human
            _board.SetCell(4, 1); // Human
            _board.SetCell(8, 2); // AI

            int move = _ai.CalculateMove(_board, 2, 1);

            // AI should make a defensive move (multiple valid options)
            Assert.GreaterOrEqual(move, 0);
            Assert.LessOrEqual(move, 8);
            Assert.IsTrue(_board.IsCellEmpty(move));
        }

        [Test]
        public void CalculateMove_FullBoard_ReturnsInvalidMove()
        {
            // Fill the board
            for (int i = 0; i < 9; i++)
            {
                _board.SetCell(i, i % 2 + 1);
            }

            int move = _ai.CalculateMove(_board, 2, 1);

            Assert.AreEqual(-1, move, "Should return -1 when board is full");
        }

        [Test]
        public void CalculateMove_CenterAvailable_PrefersCenter()
        {
            // AI often prefers center on first move
            int move = _ai.CalculateMove(_board, 2, 1);

            // Center (4) is a strong opening move
            // Note: Minimax might choose corner too, both are valid
            Assert.IsTrue(move == 4 || move == 0 || move == 2 || move == 6 || move == 8,
                "AI should choose center or corner as opening move");
        }

        [Test]
        public void EasyAI_MakesMistakesSometimes()
        {
            var easyAI = new MinimaxAI(_winChecker, AIDifficulty.Easy);

            // Test multiple games to verify randomness
            bool madeSuboptimalMove = false;
            for (int game = 0; game < 20; game++)
            {
                var testBoard = new GameBoard();
                testBoard.SetCell(0, 1);
                testBoard.SetCell(1, 1);

                int move = easyAI.CalculateMove(testBoard, 2, 1);

                // Sometimes easy AI should NOT block
                if (move != 2)
                {
                    madeSuboptimalMove = true;
                    break;
                }
            }

            Assert.IsTrue(madeSuboptimalMove, "Easy AI should occasionally make suboptimal moves");
        }

        [Test]
        public void HardAI_AlwaysPlaysOptimally()
        {
            // Test 10 games where hard AI should always block
            for (int game = 0; game < 10; game++)
            {
                var testBoard = new GameBoard();
                testBoard.SetCell(0, 1);
                testBoard.SetCell(1, 1);

                int move = _ai.CalculateMove(testBoard, 2, 1);

                Assert.AreEqual(2, move, $"Hard AI should always block on game {game}");
            }
        }

        [Test]
        public void CalculateMove_ComplexPosition_FindsBestMove()
        {
            // X O X
            // O X -
            // - - -
            _board.SetCell(0, 1); // Human
            _board.SetCell(1, 2); // AI
            _board.SetCell(2, 1); // Human
            _board.SetCell(3, 2); // AI
            _board.SetCell(4, 1); // Human

            int move = _ai.CalculateMove(_board, 2, 1);

            // AI should make a defensive/winning move
            Assert.GreaterOrEqual(move, 0);
            Assert.LessOrEqual(move, 8);
            Assert.IsTrue(_board.IsCellEmpty(move));
        }
    }
}
