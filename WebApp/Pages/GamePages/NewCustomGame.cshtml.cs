using System.Threading.Tasks;
using Domain;
using GameEngine;
using Handler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.GamePages
{
    public class NewCustomGame : PageModel
    {
        
        [BindProperty]
        public GameState GameState { get; set; } = new GameState();

        public bool IllegalAmountOfMines { get; set; }
        
        public void OnGet()
        {}

        public async Task<IActionResult> OnPostAsync()
        {
            IllegalAmountOfMines = false;
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (GameState.BoardHeight * GameState.BoardWidth <= GameState.MinesAmount)
            {
                IllegalAmountOfMines = true;
                return Page();
            }
            
            var game = new Game(GameState.BoardHeight, GameState.BoardWidth, GameState.MinesAmount, GameState.Name);
            await GameStateHandler.SaveGameToDb(game, GameState.Name);
            return RedirectToPage("./GameRunner", new {gameId = game.DbId});
        }
    }
}