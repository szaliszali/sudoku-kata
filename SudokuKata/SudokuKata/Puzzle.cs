namespace SudokuKata;

internal class Puzzle
{
    public static void CreatePuzzle(Random rng, Board board)
    {
        // Board is solved at this point.
        // Now pick subset of digits as the starting position.
        int remainingDigits = 30;
        int maxRemovedPerBlock = 6;
        int[,] removedPerBlock = new int[board.BlockSize, board.BlockSize];
        int[] positions = Enumerable.Range(0, board.Size * board.Size).ToArray();

        int removedPos = 0;
        while (removedPos < board.Size * board.Size - remainingDigits)
        {
            int curRemainingDigits = positions.Length - removedPos;
            int indexToPick = removedPos + rng.Next(curRemainingDigits);

            int row = positions[indexToPick] / board.Size;
            int col = positions[indexToPick] % board.Size;

            int blockRowToRemove = row / board.BlockSize;
            int blockColToRemove = col / board.BlockSize;

            if (removedPerBlock[blockRowToRemove, blockColToRemove] >= maxRemovedPerBlock)
                continue;

            removedPerBlock[blockRowToRemove, blockColToRemove] += 1;

            int temp = positions[removedPos];
            positions[removedPos] = positions[indexToPick];
            positions[indexToPick] = temp;

            board.Set(row, col, 0);

            removedPos += 1;
        }
    }
}
