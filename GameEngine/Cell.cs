namespace GameEngine
{
    public class Cell
    {
        public bool IsRevealed { get; set; }

        public bool IsFlagged { get; set; }

        public bool IsMine { get; set; }

        public int MinesAround { get; set; }
    }
}