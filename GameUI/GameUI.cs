using System;
using System.ComponentModel;
using GameEngine;

namespace GameUI
{
    public static class GameUI
    {
        private static readonly string _verticalSeparator = "|";
        private static readonly string _horizontalSeparator = "-";
        private static readonly string _centerSeparator = "+";
        
        public static void PrintBoard(Game game)
        {
            Cell[,] board = game.GetBoard();
            var coordinatesLine = "   |";
            for (int i = 1; i < game.BoardWidth + 1; i++)
            {
                if (i < 10)
                {
                    coordinatesLine += " " + i + " ";
                    if (i < game.BoardWidth)
                    {
                        coordinatesLine += _verticalSeparator;
                    }
                }
                else
                {
                    coordinatesLine += i + " ";
                    if (i < game.BoardWidth)
                    {
                        coordinatesLine += _verticalSeparator;
                    }
                }
            }
            Console.WriteLine(coordinatesLine);
            var sepLine = "____";
            for (int xIndex = 0; xIndex < game.BoardWidth; xIndex++)
            {
                sepLine +=  "___";
                if (xIndex < game.BoardWidth - 1)
                {
                    sepLine += "_";
                }
            }
            Console.WriteLine(sepLine);

            for (int yIndex = 0; yIndex < game.BoardHeight; yIndex++)
            {
                var numeration = yIndex + 1;
                var line = "";
                if (numeration < 10)
                {
                    line = " "+ numeration +" |";
                    
                }
                else
                {
                    line = numeration + " |";
                }
                for (int xIndex = 0; xIndex < game.BoardWidth; xIndex++)
                {
                    
                    line = line + " " + GetSingleState(board[yIndex, xIndex]) + " ";
                    if (xIndex < game.BoardWidth - 1)
                    {
                        line += _verticalSeparator;
                    }
                }
                
                Console.WriteLine(line);

                if (yIndex < game.BoardHeight - 1)
                {
                    line = "  -|";
                    for (int xIndex = 0; xIndex < game.BoardWidth; xIndex++)
                    {
                        line = line + _horizontalSeparator+ _horizontalSeparator + _horizontalSeparator;
                        if (xIndex < game.BoardWidth - 1)
                        {
                            line += _centerSeparator;
                        }
                    }
                    Console.WriteLine(line);
                }

                
            }
        }

        private static char GetSingleState(Cell cell)
        {
            if (cell.IsFlagged)
            {
                return 'F';
            }
            if (!cell.IsRevealed)
            {
                return '-';
            }
            if (cell.IsMine)
            {
                return 'X';
            }
            switch (cell.MinesAround)
            {
                case 0:
                    return ' ';
                case 1:
                    return '1';
                case 2:
                    return '2';
                case 3:
                    return '3';
                case 4:
                    return '4';
                case 5:
                    return '5';
                case 6:
                    return '6';
                case 7:
                    return '7';
                case 8:
                    return '8';
                default:
                    throw new InvalidEnumArgumentException("Unknown cell option!");
            }
        }
    }
}
