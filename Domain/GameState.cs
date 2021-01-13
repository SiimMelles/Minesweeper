using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameState 
    {
        
        public int GameStateId { get; set; }
        [MinLength(2)]
        [MaxLength(128)]
        [Display(Name = "Gamesave name:")]
        public string Name { get; set; } = default!;
        public string BoardString { get; set; } = default!;

        public string MinesString { get; set; } = default!;

        [Display(Name = "Height of the board:")]
        [Range(3, 30)]
        public int BoardHeight { get; set; } = default!;

        [Display(Name = "Width of the board:")]
        [Range(3, 16)]
        public int BoardWidth { get; set; } = default!;
        
        [Display(Name = "Amount of mines on the board:")]
        public int MinesAmount { get; set; } = default!;
        
        public bool PlayerZeroMove { get; set; }

        public bool GameLost { get; set; } = false;
        public bool GameWon { get; set; } = false;



        public override string ToString()
        {
            return Name;
        }
    }
}