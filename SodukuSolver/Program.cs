using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudokuSolver;

namespace SudokuSolverProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            int?[,] board = new int?[,]
            {
                {null, null, 3   , null, 2   , null, 6   , null, null},
                {9   , null, null, 3   , null, 5   , null, null, 1   },
                {null, null, 1   , 8   , null, 6   , 4   , null, null},
                {null, null, 8   , 1   , null, 2   , 9   , null, null},
                {7   , null, null, null, null, null, null, null, 8   },
                {null, null, 6   , 7   , null, 8   , 2   , null, null},
                {null, null, 2   , 6   , null, 9   , 5   , null, null},
                {8   , null, null, 2   , null, 3   , null, null, 9   },
                {null, null, 5   , null, 1   , null, 3   , null, null},
            };

            var newboard = Solver.SolveBoard(board);

            Console.WriteLine("\n{0}\n", Solver.IsBoardSolved(newboard));

            PrintNullableBoard(board);
            PrintNullableBoard(newboard);

            board = new int?[,]
            {
                {4   , null, null, null, null, null, 8   , null, 5   },
                {null, 3   , null, null, null, null, null, null, null},
                {null, null, null, 7   , null, null, null, null, null},
                {null, 2   , null, null, null, null, null, 6   , null},
                {null, null, null, null, 8   , null, 4   , null, null},
                {null, null, null, null, 1   , null, null, null, null},
                {null, null, null, 6   , null, 3   , null, 7   , null},
                {5   , null, null, 2   , null, null, null, null, null},
                {1   , null, 4   , null, null, null, null, null, null},
            };

            newboard = Solver.SolveBoard(board);

            Console.WriteLine("\n{0}\n", Solver.IsBoardSolved(newboard));

            PrintNullableBoard(board);
            PrintNullableBoard(newboard);

            board = new int?[,]
            {
                {15  , null, 14  , null, 3   , 7   , null, 4   , null, null, null, null, null, 8   , null, 5   },
                {5   , 2   , null, 16  , null, null, null, null, 3   , null, null, 10  , null, 14  , 1   , null},
                {null, 1   , null, 3   , null, 11  , null, null, null, 13  , 7   , null, null, null, null, null},
                {13  , null, null, 12  , null, null, 5   , null, 2   , 14  , null, null, null, null, 6   , null},
                {null, null, 3   , null, null, null, null, null, null, null, null, 8   , null, 4   , null, null},
                {null, null, null, null, 9   , null, null, null, null, null, null, 1   , null, null, null, 8   },
                {1   , null, null, null, null, null, null, null, null, null, 6   , null, 3   , null, 7   , null},
                {null, 12  , 11  , null, null, null, 3   , 5   , null, null, 2   , null, null, null, null, null},
                {null, null, 13  , null, null, 10  , null, 1   , null, 4   , null, null, null, null, null, null},
                {null, null, null, null, null, null, null, null, null, null, null, null, 2   , null, null, 11  },
                {null, 8   , null, null, null, null, null, null, null, null, 11  , null, null, null, null, 12  },
                {null, null, null, 7   , 4   , null, null, null, 16  , 12  , null, null, 1   , null, null, null},
                {7   , null, 5   , null, null, null, null, null, 6   , null, null, null, null, 16  , null, 2   },
                {null, null, null, 11  , null, null, 4   , 10  , null, null, null, null, null, null, null, 13  },
                {2   , null, null, null, null, null, null, null, 12  , null, 13  , null, 4   , null, null, null},
                {null, 6   , null, 4   , null, null, 9   , null, null, 8   , null, null, null, null, 3   , null},
           };

            newboard = Solver.SolveBoard(board);

            Console.WriteLine("\n{0}\n", Solver.IsBoardSolved(newboard));

            PrintNullableBoard(board);
            PrintNullableBoard(newboard);

            Console.ReadLine();
        }

        static void PrintNullableBoard(int?[,] board)
        {
            Console.WriteLine();
            var numRows = board.GetLength(0);
            var numCols = board.GetLength(1);
            int row;
            int col;
            int? boardVal;
            for (var i = 0; i < numRows*numCols; i++)
            {
                row = i/numCols;
                col = i%numCols;
                boardVal = board[row, col];
                if (boardVal == null || boardVal < 1 || boardVal > numCols)
                {
                    Console.Write("x  ");
                }
                else

                {
                    Console.Write("{0} ", boardVal);
                    if (boardVal < 10) Console.Write(" ");
                }
            if (col == numCols - 1) Console.WriteLine();
            }

            Console.WriteLine();
        }

        static void PrintBoard(int[,] board)
        {
            Console.WriteLine();
            var numRows = board.GetLength(0);
            var numCols = board.GetLength(1);
            int row;
            int col;
            int boardVal;
            for (var i = 0; i < numRows*numCols; i++)
            {
                row = i/numCols;
                col = i%numCols;
                boardVal = board[row, col];
                if (boardVal < 1 || boardVal > numCols)
                {
                    Console.Write("x ");
                }
                else

                {
                    Console.Write("{0} ", boardVal);
                }
            if (col == numCols - 1) Console.WriteLine();
            }

            Console.WriteLine();
        }
    }
}
