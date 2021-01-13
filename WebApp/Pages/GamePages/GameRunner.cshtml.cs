using System.Threading.Tasks;
using GameEngine;
using Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.GamePages
{
    public class GameRunner : PageModel
    {
        public Game Game { get; set; }
        public int GameId { get; set; }
        public bool Flag { get; set; }
        
        public async Task<ActionResult> OnGet(int? gameId, int? col, int? row, bool flag)
        {
            if (gameId == null) {
                return RedirectToPage("./GameIndex");
            }
            GameId = gameId.Value;

            Flag = flag;
            
            Game = GameStateHandler.LoadGameFromDb(GameId);
            
            if (Game == null)
            {
                return RedirectToPage("./GameIndex");
            }
            
            if (col != null && row != null)
            {
                var lost = Game.Move(row.Value, col.Value, Flag);
                if (!lost)
                {
                    Game.CheckIfGameWon();
                }
                
                await GameStateHandler.SaveGameToDb(Game, Game.Name);
            }
            return Page();
        }

        public IActionResult OnGetStartNewGame(int? gameId)
        {
            if (gameId != null) GameId = gameId.Value;
            
            GameStateHandler.DeleteFromDbUsingId(GameId);
            return RedirectToPage("./GameIndex");
        }
        
        public IActionResult OnGetStartSameGame(int? gameId)
        {
            if (gameId != null) GameId = gameId.Value;
            
            Game = GameStateHandler.LoadGameFromDb(GameId);
            var game = new Game(Game.BoardHeight, Game.BoardWidth, Game.MinesAmount, Game.Name);
            
            GameStateHandler.DeleteFromDbUsingId(GameId);
            GameStateHandler.SaveGameToDb(game, game.Name);
            return RedirectToPage("./GameRunner", new {gameId = game.DbId});
        }
    }
}