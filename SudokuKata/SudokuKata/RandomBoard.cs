namespace SudokuKata;

public class RandomBoard
{
    private int[] state;

    public int[] State => state;

    public RandomBoard(Random rng)
    {
        state = new RandomizerMainLoop(rng).SolvedBoardState;
    }
}
