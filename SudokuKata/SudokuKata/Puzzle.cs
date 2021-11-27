namespace SudokuKata;

internal class Puzzle
{
    public static void CreatePuzzle(Random rng, Board board)
    {
        // Board is solved at this point.
        // Now pick subset of digits as the starting position.
        (int remainingDigits, int maxRemovedPerBlock) = board.BlockSize switch
        {
            3 => (30, 6),
            _ => throw new ArgumentException()
        };
        int[,] removedPerBlock = new int[board.BlockSize, board.BlockSize];
        CellLocation[] positions = board.AllLocations().ToArray();

        int removedPos = 0;
        while (removedPos < board.Size * board.Size - remainingDigits)
        {
            int curRemainingDigits = positions.Length - removedPos;
            int indexToPick = removedPos + rng.Next(curRemainingDigits);

            CellLocation pickedCell = positions[indexToPick];

            int row = pickedCell.Row;
            int col = pickedCell.Column;

            int blockRowToRemove = row / board.BlockSize;
            int blockColToRemove = col / board.BlockSize;

            if (removedPerBlock[blockRowToRemove, blockColToRemove] >= maxRemovedPerBlock)
                continue;

            removedPerBlock[blockRowToRemove, blockColToRemove] += 1;

            CellLocation temp = positions[removedPos];
            positions[removedPos] = pickedCell;
            positions[indexToPick] = temp;

            board.Set(row, col, 0);

            removedPos += 1;
        }
    }
}
