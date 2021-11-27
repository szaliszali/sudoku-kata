namespace SudokuKata;

public class RandomBoard
{
    public Board Board { get; }

    public RandomBoard(Random rng)
    {
        Board = new StackBasedFilledBoardGenerator(rng, new Board()).SolvedBoard;
    }
}
