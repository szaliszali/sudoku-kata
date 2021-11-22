namespace SudokuKata.Test.SolverSteps;

internal class MultipleSolutions
{
    private const string FullySolvedBoard = @"
            +---+---+---+
            |971|623|584|
            |832|745|196|
            |546|819|732|
            +---+---+---+
            |498|361|275|
            |327|584|961|
            |165|972|348|
            +---+---+---+
            |289|136|457|
            |654|297|813|
            |713|458|629|
            +---+---+---+";
    private const string AmbiguousBoard = @"
            +---+---+---+
            |971|.2.|58.|
            |832|7.5|19.|
            |546|819|..2|
            +---+---+---+
            |4..|..1|2.5|
            |327|5..|9.1|
            |1.5|..2|..8|
            +---+---+---+
            |2..|..6|457|
            |654|.97|8.3|
            |7..|.5.|6.9|
            +---+---+---+";
    private const string DisambiguatedBoard = @"
            +---+---+---+
            |971|.2.|58.|
            |832|745|19.|
            |546|819|7.2|
            +---+---+---+
            |4..|..1|2.5|
            |327|5..|9.1|
            |1.5|..2|..8|
            +---+---+---+
            |2..|..6|457|
            |654|297|8.3|
            |7..|.5.|6.9|
            +---+---+---+";

    [Test]
    public void SingleSolution()
    {
        var board = new Board(DisambiguatedBoard);
        var solvedBoard = new Board(FullySolvedBoard);
        var solverState = new SolverState(board, new Random(1));
        solverState.RefreshCandidates();

        ISolverStep sut = new BoardHasMultipleSolutions(solverState, solvedBoard.State);
        var result = sut.Execute();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void SeveralMultipleSolutionsExist_PickedByRandom()
    {
        var board = new Board(AmbiguousBoard);
        var solvedBoard = new Board(FullySolvedBoard);
        var solverStateWithSeed1 = new SolverState(board, new Random(1));
        solverStateWithSeed1.RefreshCandidates();
        ISolverStep sutWithSeed1 = new BoardHasMultipleSolutions(solverStateWithSeed1, solvedBoard.State);
        var resultWithSeed1 = sutWithSeed1.Execute();

        var solverStateWithSeed2 = new SolverState(board, new Random(2));
        solverStateWithSeed2.RefreshCandidates();
        ISolverStep sutWithSeed2 = new BoardHasMultipleSolutions(solverStateWithSeed2, solvedBoard.State);
        var resultWithSeed2 = sutWithSeed2.Execute();

        Assert.Multiple(() =>
        {
            Assert.That(resultWithSeed1, Is.EqualTo(new ISolverCommand[] {
                new SetCellCommand(new CellLocation(2, 6), 7),
                new SetCellCommand(new CellLocation(2, 7), 3),
                new PrintMessageCommand("Guessing that 3 and 7 are arbitrary in row #3 (multiple solutions): Pick 7->(3, 7), 3->(3, 8)."),
            }), "Seed 1");
            Assert.That(resultWithSeed2, Is.EqualTo(new ISolverCommand[] {
                new SetCellCommand(new CellLocation(0, 8), 4),
                new SetCellCommand(new CellLocation(1, 8), 6),
                new PrintMessageCommand("Guessing that 4 and 6 are arbitrary in column #9 (multiple solutions): Pick 4->(1, 9), 6->(2, 9)."),
            }), "Seed 2");
        });
    }
}
