namespace SudokuKata;

public class RandomBoard
{
    private readonly int[] state;

    public int[] State => state;

    public RandomBoard(Random rng)
    {
        state = new StackBasedFilledBoardGenerator(rng, new int[9 * 9]).SolvedBoardState;
    }
}
