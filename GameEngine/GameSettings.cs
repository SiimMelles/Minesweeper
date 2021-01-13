namespace GameEngine
{
    public class GameSettings
    {
        public string GameName { get; set; } = "Minesweeper";
        
        public int BoardHeight { get; set; } = 9;
        
        public int BoardWidth { get; set; } = 9;

        public int MinesAmount { get; set; } = 10;
    }
}