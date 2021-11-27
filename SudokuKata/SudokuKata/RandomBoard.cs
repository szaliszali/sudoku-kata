namespace SudokuKata;

public class RandomBoard
{
    public Board Board { get; }

    public RandomBoard(Random rng, int customBlockSize)
    {
        Board = new StackBasedFilledBoardGenerator(rng, new Board(customBlockSize)).SolvedBoard;
    }
}
