using System;
using System.Collections.Generic;
using Domain;
using GameEngine;
using Handler;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.GamePages
{
    public class LoadGame : PageModel
    {
        public List<GameState> GameStates { get; set; } = new List<GameState>();
        public void OnGet()
        {
            GameStates = GameStateHandler.ShowGameSavesInDb();
            for (var index = 0; index < GameStates.Count; index++)
            {
                var gameState = GameStates[index];
                if (gameState.GameLost || gameState.GameWon)
                {
                    var id = gameState.GameStateId;
                    GameStates.RemoveAll(x => x.GameStateId == id);
                    GameStateHandler.DeleteFromDbUsingId(id);
                }
            }
        }
    }
}