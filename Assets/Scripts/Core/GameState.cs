namespace TicTacToe.Core
{
    public enum GameStatus
    {
        Playing,
        Win,
        Draw
    }

    public class GameState
    {
        public int CurrentPlayer { get; private set; }
        public GameStatus Status { get; private set; }
        public bool IsActive => Status == GameStatus.Playing;

        private readonly int[] players;
        private int currentPlayerIndex;

        public GameState(int[] players)
        {
            this.players = players;
            Reset();
        }

        public void Reset()
        {
            currentPlayerIndex = 0;
            CurrentPlayer = players[0];
            Status = GameStatus.Playing;
        }

        public void SwitchPlayer()
        {
            if (Status == GameStatus.Playing)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
                CurrentPlayer = players[currentPlayerIndex];
            }
        }

        public void SetWin()
        {
            Status = GameStatus.Win;
        }

        public void SetDraw()
        {
            Status = GameStatus.Draw;
        }
    }
}
