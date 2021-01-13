using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using GameEngine;
using Newtonsoft.Json;


namespace Handler
{
    public class GameStateHandler
    {
        
        private const string FileName = "123.json";
        private static readonly string PathBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
        private static string _saveFolder = PathBase;

        public static void SaveGame(Game game, string fileName = FileName)
        {
            
            fileName = fileName + ".json";
            CheckSaveGameFolder();
            string[] paths = new string[2] {_saveFolder, fileName};   
            using (var writer = System.IO.File.CreateText(Path.Combine(paths)))
            {
                var jsonString = JsonConvert.SerializeObject(game);
                writer.Write(jsonString);
            }
        }

        public static Game LoadGame(string fileName = FileName)
        {
            var paths = new string[2] {_saveFolder, fileName};
            if (!System.IO.File.Exists(Path.Combine(paths))) throw new Exception("Save file not found!");
            var jsonString = System.IO.File.ReadAllText(Path.Combine(paths));
            var res = JsonConvert.DeserializeObject<Game>(jsonString);
            return res;

        }
        
        public static void DeleteSave(string fileName)
        {
            fileName += ".json";
            var paths = new string[2] {_saveFolder, fileName};
            File.Delete(Path.Combine(paths));
        }
        public static string[] AllSavedGames()
        {
            CheckSaveGameFolder();
            var d = new DirectoryInfo(_saveFolder);
            var files = d.GetFiles("*.json");
            var filesList = new List<string>();
            foreach (var file in files)
            {
                filesList.Add(file.ToString());            
            }
            return filesList.ToArray();
        }

        public static void CheckSaveGameFolder()
        {
            var paths = new string[2] {PathBase, "gamesaves"};
            if (!Directory.Exists(Path.Combine(paths)))
            {
                Directory.CreateDirectory(Path.Combine(paths));
            }

            if (_saveFolder != Path.Combine(paths))
            {
             _saveFolder = Path.Combine(paths);   
            }
        }

        public static async Task SaveGameToDb(Game game, string gameName)
        {
            using (var ctx = new AppDbContext())
            {
                if (ctx.GameStates.Find(game.DbId) != null)
                {
                    var entity = ctx.GameStates.Find(game.DbId);
                    entity.PlayerZeroMove = game.PlayerZeroMove;
                    entity.BoardString = JsonConvert.SerializeObject(game.Board);
                    entity.MinesString = JsonConvert.SerializeObject(game.Mines);
                    entity.GameLost = game.GameLost;
                    entity.GameWon = game.GameWon;
                    ctx.GameStates.Update(entity);
                    await ctx.SaveChangesAsync();
                }
                else
                {
                    var gameState = new GameState()
                    {
                        Name = game.Name,
                        BoardHeight = game.BoardHeight,
                        BoardWidth = game.BoardWidth,
                        MinesAmount = game.MinesAmount,
                        BoardString = JsonConvert.SerializeObject(game.Board),
                        MinesString = JsonConvert.SerializeObject(game.Mines),
                        PlayerZeroMove = game.PlayerZeroMove,
                        GameWon = game.GameWon,
                        GameLost = game.GameLost
                    };
                    
                    ctx.Add(gameState);
                    await ctx.SaveChangesAsync();
                    var id = ctx.GameStates.Max(x => x.GameStateId);
                    game.DbId = id;  
                }
                
            }
        }

        public static Game LoadGameFromDb(int gameId)
        {
            using (var ctx = new AppDbContext())
            {
                if (!ctx.GameStates.Any(x => x.GameStateId == gameId))
                {
                    return null;
                }
                var entity = ctx.GameStates.Find(gameId);
                
                var game = new Game()
                {
                    Name = entity.Name,
                    BoardHeight = entity.BoardHeight,
                    BoardWidth = entity.BoardWidth,
                    MinesAmount = entity.MinesAmount,
                    DbId = entity.GameStateId,
                    Board = JsonConvert.DeserializeObject<Cell[,]>(entity.BoardString),
                    Mines = JsonConvert.DeserializeObject<char[,]>(entity.MinesString),
                    PlayerZeroMove = entity.PlayerZeroMove,
                    GameLost = entity.GameLost,
                    GameWon = entity.GameWon

                };
                 
                return game;
            }
        }
        
        public static void DeleteFromDbUsingId(int id)
        {
            using (var ctx = new AppDbContext())
            {
                var entity = ctx.GameStates.Find(id);

                ctx.GameStates.Remove(entity);
                ctx.SaveChanges();
            }
        }
        
        public static void DeleteFromDb(Game game)
        {
            using (var ctx = new AppDbContext())
            {
                var entity = ctx.GameStates.Find(game.DbId);

                ctx.GameStates.Remove(entity);
                ctx.SaveChanges();
            }
        }

        public static List<GameState> ShowGameSavesInDb()
        {
            var gameSaves = new List<GameState>{};
            
            using (var ctx = new AppDbContext())
            {
                gameSaves.AddRange(ctx.GameStates.Select(gameState => gameState));
            }
            return gameSaves;
        }
    }
}