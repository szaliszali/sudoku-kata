namespace SudokuKata;

public class RandomBoard
{
    private readonly Board solvedBoard;

    public Board Board => solvedBoard;

    public RandomBoard(Random rng)
    {
        solvedBoard = new StackBasedFilledBoardGenerator(rng, new Board()).SolvedBoard;
    }
}
