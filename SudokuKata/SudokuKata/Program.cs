namespace SudokuKata;

public class Program
{
    static void Play()
    {
        Play(new Random());
    }

    public static void Play(Random rng)
    {
        Board board = ConstructFullyPopulatedBoard(rng);

        var finalState = board.State.ShallowCopy();

        GenerateInitalBoardFromTheCompletelySolvedOne(rng, board);

        Console.WriteLine();
        Console.WriteLine(new string('=', 80));
        Console.WriteLine();

        new Solver(rng, board, finalState).SolveBoard();
    }

    private static Board ConstructFullyPopulatedBoard(Random rng)
    {
        var board = new Board(new RandomBoard(rng).State);

        Console.WriteLine();
        Console.WriteLine("Final look of the solved board:");
        Console.WriteLine(board);

        return board;
    }

    private static void GenerateInitalBoardFromTheCompletelySolvedOne(Random rng, Board board)
    {
        int[] state = board.State;

        // Board is solved at this point.
        // Now pick subset of digits as the starting position.
        int remainingDigits = 30;
        int maxRemovedPerBlock = 6;
        int[,] removedPerBlock = new int[3, 3];
        int[] positions = Enumerable.Range(0, 9 * 9).ToArray();

        int removedPos = 0;
        while (removedPos < 9 * 9 - remainingDigits)
        {
            int curRemainingDigits = positions.Length - removedPos;
            int indexToPick = removedPos + rng.Next(curRemainingDigits);

            int row = positions[indexToPick] / 9;
            int col = positions[indexToPick] % 9;

            int blockRowToRemove = row / 3;
            int blockColToRemove = col / 3;

            if (removedPerBlock[blockRowToRemove, blockColToRemove] >= maxRemovedPerBlock)
                continue;

            removedPerBlock[blockRowToRemove, blockColToRemove] += 1;

            int temp = positions[removedPos];
            positions[removedPos] = positions[indexToPick];
            positions[indexToPick] = temp;

            board.Set(row, col, 0);

            int stateIndex = 9 * row + col;
            state[stateIndex] = 0;

            removedPos += 1;
        }

        Console.WriteLine();
        Console.WriteLine("Starting look of the board to solve:");
        Console.WriteLine(board);
    }

    static void Main(string[] args)
    {
        Play();

        Console.WriteLine();
        Console.Write("Press ENTER to exit... ");
        Console.ReadLine();
    }
}