using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public static class Solver
    {
        private static int _sudokuDimension;
        private static int _boardDimension;
        private static int _allPossible;
        private static int[] _valueMasks;

        public static int?[,] SolveBoard(int?[,] board)
        {
            // Initializes board dimensions and creates value masks
            Init(board);
            
            // Create board for eliminations.  All values start at _allPossible.
            var elimBoard = CreateEliminationBoard();

            // One sweep to assign and eliminate all given values
            int row;
            int col;
            int? boardVal;
            for (var i = 0; i < _boardDimension*_boardDimension; i++)
            {
                row = i/_boardDimension;
                col = i%_boardDimension;

                boardVal = board[row, col];
                if (boardVal == null) continue;

                var mask = _valueMasks[(int) boardVal - 1];
                elimBoard[row, col] = mask;
                EliminatePeers(row, col, mask, elimBoard);
            }

            // Reduce and assign values until no change is made.
            ReduceAndAssign(elimBoard);

            // Brute force search the remaining values
            var boardStack = new Stack<LocationBoardPair>();
            bool next = true;
            for (var i = 0; i < _boardDimension*_boardDimension; i++)
            {
                row = i/_boardDimension;
                col = i%_boardDimension;
                if (next && ElementSet(elimBoard[row, col])) continue;

                if (elimBoard[row, col] == 0 && boardStack.Count == 0) throw new Exception("Solution not found.");
                if (elimBoard[row, col] == 0)
                {
                    var stackElem = boardStack.Pop();
                    elimBoard = stackElem.Board;
                    i = stackElem.Row*_boardDimension + stackElem.Col - 1;
                    next = false;
                    continue;
                }

                var newVal = FindFirstCandidate(elimBoard[row, col]);

                var stackBoard = new int[_boardDimension, _boardDimension];
                Array.Copy(elimBoard, stackBoard, _boardDimension * _boardDimension);
                stackBoard[row, col] &= ~ newVal;
                boardStack.Push(new LocationBoardPair(row, col, stackBoard));

                elimBoard[row, col] = newVal;
                try
                {
                    EliminatePeers(row, col, newVal, elimBoard);
                    ReduceAndAssign(elimBoard);
                }
                catch (Exception)
                {
                    if (boardStack.Count == 0) throw new Exception("Solution not found.");
                    var stackElem = boardStack.Pop();
                    elimBoard = stackElem.Board;
                    i = stackElem.Row * _boardDimension + stackElem.Col - 1;
                    next = false;
                    continue;
                }
                next = true;
            }

            // Convert to nullable board
            return ConvertBoards(elimBoard);
        }

        // Eliminates possibilities in peers of set values.  Throws exception if we run out of possibilities.
        private static void EliminatePeers(int row, int col, int elimVal, int[,] elimBoard)
        {
            var elimQueue = new Queue<RowValuePair>();
            elimQueue.Enqueue(new RowValuePair(row, col, elimVal));

            while(elimQueue.Count > 0)
            {
                // Dequeue coordinate and eliminate
                var coord = elimQueue.Dequeue();
                row = coord.Row;
                col = coord.Col;
                elimVal = coord.Val;

                var elimMask = ~elimVal & _allPossible;

                bool elementAlreadySet;

                // Eliminate row
                for (var elimCol = 0; elimCol < _boardDimension; elimCol++)
                {
                    // Skip if it's the number location we're eliminating.  Check if already set.
                    if (elimCol == col) continue;
                    elementAlreadySet = ElementSet(elimBoard[row, elimCol]);

                    // Eliminate options
                    elimBoard[row, elimCol] &= elimMask;

                    // See if we've set a value.  If so add it to the queue.
                    if (!elementAlreadySet && ElementSet(elimBoard[row, elimCol])) elimQueue.Enqueue(new RowValuePair(row, elimCol, elimBoard[row, elimCol]));

                    // If it's zero then we have a problem.
                    if (elimBoard[row, elimCol] == 0) throw new Exception("Ran out of row options.");
                }

                // Eliminate col
                for (var elimRow = 0; elimRow < _boardDimension; elimRow++)
                {
                    // Skip if it's the number location we're eliminating.  Check if already set.
                    if (elimRow == row) continue;
                    elementAlreadySet = ElementSet(elimBoard[elimRow, col]);

                    // Eliminate options
                    elimBoard[elimRow, col] &= elimMask;

                    // See if we've set a value.  If so add it to the queue.
                    if (!elementAlreadySet && ElementSet(elimBoard[elimRow, col])) elimQueue.Enqueue(new RowValuePair(elimRow, col, elimBoard[elimRow, col]));

                    // If it's zero then we have a problem.
                    if (elimBoard[elimRow, col] == 0) throw new Exception("Ran out of col options.");
                }

                // Eliminate block
                var blockNum = col/_sudokuDimension + _sudokuDimension*(row/_sudokuDimension);
                var firstRow = (blockNum / _sudokuDimension) * _sudokuDimension;
                var firstCol = (blockNum % _sudokuDimension) * _sudokuDimension;
                for (var i = 0; i < _boardDimension; i++)
                {
                    var curRow = firstRow + i/_sudokuDimension;
                    var curCol = firstCol + i%_sudokuDimension;

                    // Skip if it's the number location we're eliminating.  Check if already set.
                    if (curRow == row && curCol == col) continue;
                    elementAlreadySet = ElementSet(elimBoard[curRow, curCol]);

                    // Eliminate options
                    elimBoard[curRow, curCol] &= elimMask;

                    // See if we've set a value.  If so add it to the queue.
                    if (!elementAlreadySet && ElementSet(elimBoard[curRow, curCol])) elimQueue.Enqueue(new RowValuePair(curRow, curCol, elimBoard[curRow, curCol]));

                    // If it's zero then we have a problem.
                    if (elimBoard[curRow, curCol] == 0) throw new Exception("Ran out of blk options.");
                }
            }
        }

        private static int FindFirstCandidate(int testVal)
        {
            foreach (var mask in _valueMasks)
            {
                if ((testVal & mask) != 0) return mask;
            }

            return 0;
        }

        private class LocationBoardPair
        {
            public readonly int Row;
            public readonly int Col;
            public readonly int[,] Board;

            public LocationBoardPair(int row, int col, int[,] board)
            {
                Row = row;
                Col = col;
                Board = board;
            }
        }

        private static void ReduceAndAssign(int[,] board)
        {
            int row;
            int col;
            var changeMade = true;
            while (changeMade)
            {
                changeMade = false;
                for (var i = 0; i < _boardDimension * _boardDimension; i++)
                {
                    row = i / _boardDimension;
                    col = i % _boardDimension;
                    if (ElementSet(board[row, col])) continue;

                    changeMade |= ExtrapolateFromPeers(row, col, board);
                }
            }
        }

        private static int?[,] ConvertBoards(int[,] board)
        {
            var retBoard = new int?[_boardDimension, _boardDimension];
            int row;
            int col;
            for (var i = 0; i < _boardDimension * _boardDimension; i++)
            {
                row = i / _boardDimension;
                col = i % _boardDimension;
                retBoard[row, col] = ConvertVal(board[row, col]);
            }

            return retBoard;
        }

        private static int? ConvertVal(int value)
        {
            for (var i = 0; i < _boardDimension; i++)
            {
                if (_valueMasks[i] != value) continue;
                return i+1;
            }

            return null;
        }

        private static bool ExtrapolateFromPeers(int row, int col, int[,] elimBoard)
        {
            var curVal = elimBoard[row, col];

            // Extrapolate from row peers
            var possibilities = 0;
            for (var elimCol = 0; elimCol < _boardDimension; elimCol++)
            {
                if (elimCol == col) continue;
                possibilities |= elimBoard[row, elimCol];
            }
            possibilities = (~possibilities & _allPossible) & curVal;
            int newValue;
            if (FindFixedPoint(possibilities, out newValue))
            {
                elimBoard[row, col] = newValue;
                EliminatePeers(row, col, newValue, elimBoard);
                return true;
            }

            // Extrapolate from col peers
            possibilities = 0;
            for (var elimRow = 0; elimRow < _boardDimension; elimRow++)
            {
                if (elimRow == row) continue;
                possibilities |= elimBoard[elimRow, col];
            }
            possibilities = (~possibilities & _allPossible) & curVal;
            if (FindFixedPoint(possibilities, out newValue))
            {
                elimBoard[row, col] = newValue;
                EliminatePeers(row, col, newValue, elimBoard);
                return true;
            }

            // Extrapolate from blk peers
            possibilities = 0;
            var blockNum = col / _sudokuDimension + _sudokuDimension * (row / _sudokuDimension);
            var firstRow = (blockNum / _sudokuDimension) * _sudokuDimension;
            var firstCol = (blockNum % _sudokuDimension)*_sudokuDimension;
            for (var i = 0; i < _boardDimension; i++)
            {
                var curRow = firstRow + i / _sudokuDimension;
                var curCol = firstCol + i % _sudokuDimension;
                if (curRow == row && curCol == col) continue;
                possibilities |= elimBoard[curRow, curCol];
            }
            possibilities = (~possibilities & _allPossible) & curVal;
            if (FindFixedPoint(possibilities, out newValue))
            {
                elimBoard[row, col] = newValue;
                EliminatePeers(row, col, newValue, elimBoard);
                return true;
            }

            return false;
        }

        private static bool FindFixedPoint(int possibilities, out int value)
        {
            value = 0;
            foreach (var mask in _valueMasks)
            {
                if (mask != possibilities) continue;
                value = mask;
                return true;
            }

            return false;
        }

        private static bool ElementSet(int val)
        {
            foreach (var mask in _valueMasks)
            {
                if (mask != val) continue;
                return true;
            }

            return false;
        }

        // Class to store row column pairs
        private class RowValuePair
        {
            public readonly int Row;
            public readonly int Col;
            public readonly int Val;

            public RowValuePair(int row, int col, int val)
            {
                Row = row;
                Col = col;
                Val = val;
            }
        }

        // Creates a board initialized to _allPossible values.
        private static int[,] CreateEliminationBoard()
        {
            var elimBoard = new int[_boardDimension, _boardDimension];
            for (var row = 0; row < _boardDimension; row++)
            {
                for (var col = 0; col < _boardDimension; col++)
                {
                    elimBoard[row, col] = _allPossible;
                }
            }

            return elimBoard;
        }

        // Determines if a board is complete and correct.  Initializes board dimensions first.
        public static bool IsBoardSolved(int?[,] board)
        {
            SetBoardDimensions(board);
            return IsBoardSolved(board, _sudokuDimension, _boardDimension);
        }

        // Determines if a board is complete and correct.
        private static bool IsBoardSolved(int?[,] board, int sudokuDimension, int boardDimension)
        {
            GenerateValueMasks(boardDimension);

            var rows = new int[boardDimension];
            var cols = new int[boardDimension];
            var blks = new int[boardDimension];

            int? boardVal;
            for (var row = 0; row < boardDimension; row++)
            {
                for (var col = 0; col < boardDimension; col++)
                {
                    boardVal = board[row, col];
                    if (boardVal == null) return false;

                    rows[row] |= _valueMasks[(int)boardVal - 1];
                    cols[col] |= _valueMasks[(int)boardVal - 1];
                    blks[col / sudokuDimension + sudokuDimension * (row / sudokuDimension)] |= _valueMasks[(int)boardVal - 1];
                }
            }

            var correctVal = 0;
            foreach (var mask in _valueMasks)
            {
                correctVal |= mask;
            }

            for (var i = 0; i < boardDimension; i++)
            {
                var valid = rows[i] == correctVal;
                valid &= cols[i] == correctVal;
                valid &= blks[i] == correctVal;
                if (!valid) return false;
            }

            return true;
        }

        // Initializes board dimensions and creates value masks.
        private static void Init(int?[,] board)
        {
            SetBoardDimensions(board);
            GenerateValueMasks(_boardDimension);
        }

        // Takes board, determines if it is a valid Sodoku board, and sets the dimensions.
        private static void SetBoardDimensions(int?[,] board)
        {
            var boardDim = board.GetLength(0);
            var sudokuDim = (int) (Math.Floor(Math.Sqrt(boardDim)));

            var valid = boardDim == board.GetLength(1);
            valid &= (int) (Math.Pow(sudokuDim, 2.0)) == boardDim;

            if (!valid) throw new Exception("Board dimensions invalid.");
            if (boardDim > 64) throw new Exception("Board is too large!");

            _boardDimension = boardDim;
            _sudokuDimension = sudokuDim;
        }

        // Creates the masks that represent the sodoku values
        private static void GenerateValueMasks(int dimension)
        {
            _valueMasks = new int[dimension];
            _valueMasks[0] = 1;
            _allPossible = _valueMasks[0];
            for (var i = 1; i < _valueMasks.Length; i++)
            {
                _valueMasks[i] = _valueMasks[i - 1] << 1;
                _allPossible |= _valueMasks[i];
            }
        }

    }
}
