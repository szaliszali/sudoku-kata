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

        var finalState = board.Clone();

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
        Puzzle.CreatePuzzle(rng, board);

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