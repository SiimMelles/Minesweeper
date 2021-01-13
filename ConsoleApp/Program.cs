using System;
using System.Collections.Generic;
using Domain;
using GameEngine;
using Handler;
using MenuSystem;
using GameStateHandler = Handler.GameStateHandler;

namespace ConsoleApp
{
    class Program
    {
        private static GameSettings _settings  = GameConfigHandler.LoadConfig();
        private static Game _gameState = new Game();

        private static void Main(string[] args)
        {
            _settings = GameConfigHandler.LoadConfig();

            Console.WriteLine($"Hello {_settings.GameName}!");
            
            var menuLevel2 = new Menu(2)
            {
                Title = "Level2 menu",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
            };
            
            var gameMenu = new Menu(1)
            {
                Title = $"Start a new game of {_settings.GameName}",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
                {
                    {
                        "1", new MenuItem() {Title = "Beginner", CommandToExecute = RunGameEasy}
                    },
                    {
                        "2", new MenuItem() {Title = "Intermediate", CommandToExecute = RunGameMedium}
                    },
                    {
                        "3", new MenuItem() {Title = "Expert", CommandToExecute = RunGameHard}
                    },
                    {
                        "4", new MenuItem() {Title = "Custom", CommandToExecute = RunGameCustom}
                    },
                    {
                        "5", new MenuItem() {Title = "Custom Game from settings", CommandToExecute = RunGameCustomFromSettings}
                    },
                }
            };
            
            var menu0 = new Menu()
            {
                Title = $"{_settings.GameName} Main Menu",
                MenuItemsDictionary = new Dictionary<string, MenuItem>()
                {
                    {
                        "S", new MenuItem()
                        {
                            Title = "Start game",
                            CommandToExecute = gameMenu.Run
                        }
                    },
                    /*{
                        "J", new MenuItem()
                        {
                            Title = "Set defaults for game (save to JSON)",
                            CommandToExecute = SaveSettings
                        }
                    },
                    {
                        "L", new MenuItem()
                        {
                            Title = "Continue Game from saved file",
                            CommandToExecute = SavedGamesLoad
                        }
                    },*/
                    {
                        "D", new MenuItem()
                        {
                            Title = "Continue Game from database save",
                            CommandToExecute = SavedGamesLoadFromDb
                        }
                    }
                }
            };
            
            menu0.Run();
        }

        private static string SavedGamesLoadFromDb()
        {
            var gameSaves = GameStateHandler.ShowGameSavesInDb();
            if (gameSaves.Count == 0)
            {
                Console.WriteLine("No saved games in database!");
                return "";
            }

            var index = 1;
            foreach (var gameSave in gameSaves)
            {
                Console.WriteLine($"{index} {gameSave.Name}");
                index++;
            }

            bool userCanceled;
            int userInput;
            (userInput, userCanceled) = GetUserIntInput("Which game would you like to load?",
                1, gameSaves.Count);
            if (userCanceled)
            {
                return "";
            }

            GameState gameStateToLoad = gameSaves[userInput - 1];

            var gameToLoad = GameStateHandler.LoadGameFromDb(gameStateToLoad.GameStateId);
            if (gameToLoad != null)
            {
                GameRunner(gameToLoad);
            }
            else
            {
                Console.WriteLine("Game was not found!");
            }
            return "";
        }
        /*
        private static string SavedGamesLoad()
        {
            string[]? games = GameStateHandler.AllSavedGames();
            if (games.Length == 0)
            {
                Console.WriteLine("No game files found!");
                return "";
            }
            for (var i = 0; i < games.Length; i++)
            {
                Console.WriteLine($"{i + 1} {games.GetValue(i)}");
            }

            bool userCanceled;
            int userInput;
            (userInput, userCanceled) = GetUserIntInput("Which game would you like to load?",
                1, games.Length);
            if (userCanceled)
            {
                return "";
            }

            var gameToLoad = GameStateHandler.LoadGame(games.GetValue(userInput - 1).ToString());
            if (gameToLoad != null)
            {
                GameRunner(gameToLoad);
            }
            else
            {
                Console.WriteLine("Game was not found!");
            }
            return "";
        }

        private static string SaveSettings() 
        {
            Console.Clear();
            int boardWidth;
            int boardHeight;
            int minesAmount;

            bool userCanceled;
                
            (boardWidth, userCanceled) = GetUserIntInput("Enter board width (3-20)", 3, 30, 0);
            if (userCanceled)
            {
                return "";
            }
            (boardHeight, userCanceled) = GetUserIntInput("Enter board height (3-20)", 3, 16, 0);
            if (userCanceled)
            {
                return "";
            }
            (minesAmount, userCanceled) = GetUserIntInput($"Enter the amount of mines (1- {boardHeight*boardWidth - 1})",
                1, boardHeight*boardWidth - 1, 0);
            if (userCanceled)
            {
                return "";
            }
                
            _settings.BoardHeight = boardHeight;
            _settings.BoardWidth = boardWidth;
            _settings.MinesAmount = minesAmount;
            GameConfigHandler.SaveConfig(_settings);
                
            return "";
        }
        */
        
        private static string GameRunner(Game game)
        {
            var done = false;
            bool firstMove = game.PlayerZeroMove;
            var gameLost = false;
            var gameWon = false;

            if (game.Name == "")
                
            {
                Console.WriteLine("Set name for game instance");
                Console.Write(">");
                var command = Console.ReadLine()?.Trim() ?? "";
                game.Name = command;
            } 
            
            do
            {
                GameStateHandler.SaveGameToDb(game, game.Name);
                GameUI.GameUI.PrintBoard(game);
                var userXint = 0;
                var userYint = 0;
                bool flag;

                bool userCanceled;
                (flag, userCanceled) = GetUserBoolInput("Flag (F) / Open (A)");
                if (!userCanceled)
                {
                    (userXint, userCanceled) = GetUserIntInput("Enter X coordinate", 1, game.BoardWidth);
                    if (!userCanceled)
                    {
                        (userYint, userCanceled) = GetUserIntInput("Enter Y coordinate", 1, game.BoardHeight);
                    }
                }
                
                if (userCanceled)
                {
                    done = true;
                }
                else
                {
                    gameLost = game.Move(userYint-1, userXint-1, flag);
                }
                
                if (!firstMove && !userCanceled)
                {
                    gameWon = game.CheckIfGameWon(); 
                    done = gameWon || gameLost;
                }

                if (firstMove)
                {
                    firstMove = false;
                }
                GameStateHandler.SaveGame(game, game.Name);

            } while (!done);
            
            GameUI.GameUI.PrintBoard(game);

            if (gameLost)
            {
                Console.WriteLine("GAME OVER!");
                GameStateHandler.DeleteSave(game.Name);
                GameStateHandler.DeleteFromDb(game);

            }
            else if (gameWon)
            {
                Console.WriteLine("You won!");
                GameStateHandler.DeleteSave(game.Name);
                GameStateHandler.DeleteFromDb(game);
            }
            return "";
        }

        private static string RunGameEasy()
        {
            var game = new Game(9, 9, 10);
            GameRunner(game);
            return "";
        }

        private static string RunGameMedium()
        {
            var game = new Game(16, 16, 30);
            GameRunner(game);
            return "";
        }

        static string RunGameHard()
        {
            var game = new Game(16, 30, 99);
            GameRunner(game);
            return "";
        }

        private static string RunGameCustomFromSettings()
        {
            var game = new Game(_settings.BoardHeight, _settings.BoardWidth, _settings.MinesAmount);
            GameRunner(game);
            return "";
        }

        private static string RunGameCustom()
        {
            int boardHeight;
            int boardWidth;
            int mineCount;
            bool userCanceled;
            
            (boardWidth, userCanceled) = GetUserIntInput("Enter board width (3-30):", 3, 30, 0);
            if (userCanceled)
            {
                return "";
            }
            (boardHeight, userCanceled) = GetUserIntInput("Enter board height (3-16):", 3, 16, 0);
            if (userCanceled)
            {
                return "";
            }
            (mineCount, userCanceled) = GetUserIntInput("Enter amount of mines:", 1, boardWidth * boardHeight - 1, 0);
            if (userCanceled)
            {
                return "";
            }
            var game = new Game(boardHeight, boardWidth, mineCount);
            GameRunner(game);
            return "";
        }

        private static (int result, bool wasCanceled) GetUserIntInput(string prompt, int min, int max, int? cancelIntValue = null, string cancelStrValue = "" )
        {
            do
            {
                Console.WriteLine(prompt);
                if (cancelIntValue.HasValue || !string.IsNullOrWhiteSpace(cancelStrValue))
                {
                    Console.WriteLine($"To cancel input enter: {cancelIntValue}" +
                                      $"{ (cancelIntValue.HasValue && !string.IsNullOrWhiteSpace(cancelStrValue) ? " or " : "") }" +
                                      $"{cancelStrValue}");
                }
                else
                {
                    Console.WriteLine("To cancel input press Enter");
                }

                Console.Write(">");
                var consoleLine = Console.ReadLine();

                if (consoleLine == cancelStrValue) return (0, true);
                
                if (int.TryParse(consoleLine, out var userInt))
                {
                    if ((!(userInt > max || userInt < min)))
                    {
                        return userInt == cancelIntValue ? (userInt, true) : (userInt, false);
                    }
                    Console.WriteLine("Number is invalid!");
                }
                else
                {
                    Console.WriteLine($"'{consoleLine}' cant be converted to int value!");
                }
            } while (true);
        }

        private static (bool flag, bool wasCanceled) GetUserBoolInput(string prompt)
        {
            do
            {
                Console.WriteLine(prompt);
                Console.WriteLine("To cancel input press X");
                Console.Write(">");
                
                var consoleLine = Console.ReadLine();

                var command = consoleLine?.Trim().ToUpper() ?? "";
                
                if (command == "X") return (false, true);

                if (command == "F")
                {
                    return (true, false);
                }
                if (command == "A")
                {
                    return (false, false);
                }
                Console.WriteLine("Unrecognized command!");
            }
            while (true);
        }
    }
}
