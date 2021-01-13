using System.Threading.Tasks;
using Domain;
using GameEngine;
using Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.GamePages
{
    public class NewPresetGame : PageModel
    {
        [BindProperty]
        public GameState GameState { get; set; } = new GameState();
        
        [BindProperty]
        public int Difficulty { get; set; }
        
        public void OnGet(int? difficulty)
        {
            if (difficulty != null) Difficulty = difficulty.Value;
        }
        
        public async Task<IActionResult> OnPostAsync(int? difficulty)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (difficulty == null)
            {
                return RedirectToPage("./GameIndex");
            }
            
            if (difficulty == 1)
            {
                var game = new Game(9, 9, 10, GameState.Name);
                await GameStateHandler.SaveGameToDb(game, GameState.Name);
                return RedirectToPage("./GameRunner", new {gameId = game.DbId});
            }
            if (difficulty == 2)
            {
                var game = new Game(16, 16, 40, GameState.Name); 
                await GameStateHandler.SaveGameToDb(game, GameState.Name);
                return RedirectToPage("./GameRunner", new {gameId = game.DbId});
            }
            if (difficulty == 3)
            {
                var game = new Game(16, 30, 99, GameState.Name);
                await GameStateHandler.SaveGameToDb(game, GameState.Name);
                return RedirectToPage("./GameRunner", new {gameId = game.DbId});
            }
            return RedirectToPage("./GameIndex");
        }
    }
}