using System;

namespace GameEngine
{
    public class Game
    {
        public int DbId { get; set; }
        public string Name { get; set; }
        public Cell[,] Board { get; set; }
        public char[,] Mines { get; set; }
        public int BoardHeight { get; set; }
        public int BoardWidth { get; set; }
        public int MinesAmount { get; set; }
        
        public bool GameLost { get; set; }
        public bool GameWon { get; set; }

        public bool PlayerZeroMove { get; set; }

        public Game(int dbId, string name, Cell[,] board, char[,] mines, int boardHeight, int boardWidth, int minesAmount, bool playerZeroMove)
        {
            DbId = dbId;
            Name = name;
            Board = board;
            Mines = mines;
            BoardHeight = boardHeight;
            BoardWidth = boardWidth;
            MinesAmount = minesAmount;
            PlayerZeroMove = playerZeroMove;
            GameLost = false;
            GameWon = false;
        }

        public Game() : this(9, 9, 10) {}
        
        public Game(int boardHeight, int boardWidth, int minesAmount, string gameName)
        {
            if (boardHeight < 3 || boardWidth < 3)
            {
                throw new Exception("Board size has to be at least 3x3!");
            }
            
            BoardHeight = boardHeight;
            BoardWidth = boardWidth;
            MinesAmount = minesAmount;
            Board = new Cell[boardHeight, boardWidth];
            Mines = new char[boardHeight, boardWidth];
            Name = gameName;
            GameLost = false;
            GameWon = false;
            // Fill board with Cellstate O.
            for (int yIndex = 0; yIndex < BoardHeight; yIndex++)
            {
                for (int xIndex = 0; xIndex < BoardWidth; xIndex++)
                {
                    Board[yIndex, xIndex] = new Cell
                    {
                        IsMine = false,
                        IsRevealed = false,
                        MinesAround = 0
                    };
                }
            }
            PlayerZeroMove = true;
        }
        
        public Game(int boardHeight, int boardWidth, int minesAmount)
        {
            if (boardHeight < 3 || boardWidth < 3)
            {
                throw new Exception("Board size has to be at least 3x3!");
            }
            
            BoardHeight = boardHeight;
            BoardWidth = boardWidth;
            MinesAmount = minesAmount;
            Board = new Cell[boardHeight, boardWidth];
            Mines = new char[boardHeight, boardWidth];
            Name = "";
            GameLost = false;
            GameWon = false;
            // Fill board with Cellstate O.
            for (int yIndex = 0; yIndex < BoardHeight; yIndex++)
            {
                for (int xIndex = 0; xIndex < BoardWidth; xIndex++)
                {
                    Board[yIndex, xIndex] = new Cell
                    {
                        IsMine = false,
                        IsRevealed = false,
                        MinesAround = 0
                    };
                }
            }
            PlayerZeroMove = true;
        }
        
        
        public Cell[,] GetBoard()
        {
            var result = new Cell[BoardHeight, BoardWidth];
            Array.Copy(Board, result, Board.Length);
            return result;
        }


        private void PlaceMinesOnBoard(int userY, int userX) // take in userY and userX so the first move is secure
        {
            var mineCount = MinesAmount;
            do
            {
                var x = RandomIntGenerator(0, BoardWidth);
                var y = RandomIntGenerator(0, BoardHeight);
                if (!(x == userX && y == userY))
                {
                    if (Mines[y, x] != 'X')
                    {
                        Mines[y, x] = 'X';
                        mineCount -= 1;
                    }    
                }
            } while (mineCount > 0);
        }

        private int RandomIntGenerator(int min, int max)
        {
            Random ran = new Random();
            return ran.Next(min, max);
        }

        public bool Move(int userYint, int userXint, bool flag)
        {
            if (PlayerZeroMove)
            {
                PlaceMinesOnBoard(userYint, userXint);
                PlayerZeroMove = false;
            }
            
            if (flag) // Flag the selected spot
            {
                if (Board[userYint, userXint].IsFlagged)
                {
                    Board[userYint, userXint].IsFlagged = false;
                }
                else
                {
                    Board[userYint, userXint].IsFlagged = true;
                }
                return false;
            }

            if (Board[userYint, userXint].IsFlagged)
            {
                return false;
            }
            
            if (Mines[userYint, userXint] == 'X')
            {
                GameLost = true;
                RevealBoard();
                return true;
            }

            if (!Board[userYint, userXint].IsFlagged)
            {
                RevealCell(userYint, userXint);
            }
            else
            {
                Console.WriteLine("Can't reveal flagged square!");
            }
            return false;
        }

        private void CountMinesAroundCell(int userYint, int userXint)
        {
            var minesAroundMove = 0;
            
            for (int y = userYint - 1; y < userYint + 2; y++)
            {
                if (!(y < 0 || y >= BoardHeight))
                {
                    for (int x = userXint - 1; x < userXint + 2; x++)
                    {
                        if (!(x < 0 || x >= BoardWidth))
                        {
                            if (Mines[y, x] == 'X')
                            {
                                minesAroundMove++;
                            }
                        }
                    } 
                }
            }
            Board[userYint, userXint].MinesAround = minesAroundMove;
        }

        private void RevealCell(int userY, int userX)
        {
            CountMinesAroundCell(userY, userX);
            Board[userY, userX].IsRevealed = true;
            Board[userY, userX].IsFlagged = false;
            if (Board[userY, userX].MinesAround == 0)
            {
                FloodFill(userY, userX);
            }
        }

        private void FloodFill(in int userY, in int userX)
        {
            for (int y = userY - 1; y < userY + 2; y++)
            {
                if (!(y < 0 || y >= BoardHeight))
                {
                    for (int x = userX - 1; x < userX + 2; x++)
                    {
                        if (!(x < 0 || x >= BoardWidth))
                        {
                            if (!Board[y, x].IsMine && !Board[y, x].IsRevealed && !Board[y, x].IsFlagged)
                            {
                                RevealCell(y, x);
                            }
                        }
                    } 
                }
            }
            
        }

        public bool CheckIfGameWon()
        {
            var unopenedCells = 0;
            for (int yIndex = 0; yIndex < BoardHeight; yIndex++)
            {
                for (int xIndex = 0; xIndex < BoardWidth; xIndex++)
                {
                    if (!Board[yIndex, xIndex].IsRevealed)
                    {
                        unopenedCells += 1;
                    }
                }
            }
            if (unopenedCells == MinesAmount)
            {
                RevealBoard();
                GameWon = true;
                return true;
            }
            return false;
        }

        private void RevealBoard()
        {
            for (int yIndex = 0; yIndex < BoardHeight; yIndex++)
            {
                for (int xIndex = 0; xIndex < BoardWidth; xIndex++)
                {
                    if (Mines[yIndex, xIndex] == 'X')
                    {
                        Board[yIndex, xIndex].IsMine = true;
                        Board[yIndex, xIndex].IsRevealed = true;
                    }
                    else
                    {
                        RevealCell(yIndex, xIndex);
                    }
                }
            }
        }
    }
}