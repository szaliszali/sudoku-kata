namespace SudokuKata.Test.SolverSteps;

internal class GroupsOfDigitsOfSizeNWhichOnlyAppearInNCells
{
    [Test]
    public void OnAnEmptyBoard_NoStepsArePossible()
    {
        var board = new Board();
        var solverState = new SolverState(board, new Random());
        solverState.RefreshCandidates();

        var result = SudokuKata.SolverSteps.TryToFindGroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsWithinRowColumnBlock.Solve(solverState);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ATest()
    {
        var board = new Board(@"
            +---+---+---+
            |...|5.8|.17|
            |6.7|.1.|.25|
            |.5.|...|.39|
            +---+---+---+
            |.75|68.|1..|
            |.6.|.57|.83|
            |..8|1..|576|
            +---+---+---+
            |.16|..5|3..|
            |593|.41|76.|
            |...|3..|.51|
            +---+---+---+");
        var solverState = new SolverState(board, new Random());
        solverState.RefreshCandidates();

        var result = SudokuKata.SolverSteps.TryToFindGroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsWithinRowColumnBlock.Solve(solverState);

        Assert.That(result, Is.EqualTo(new ISolverCommand[]
        {
            new PrintMessageCommand("In column #7 values 2, 9 appear only in cells (5, 7) (9, 7) and other values cannot appear in those cells."),
            new EliminateCandidatesCommand(new CellLocation(4, 6), new[]{ 4 }),
            new PrintMessageCommand("4 cannot appear in cell (5, 7)."),
            new EliminateCandidatesCommand(new CellLocation(8, 6), new[]{ 4, 8 }),
            new PrintMessageCommand("4, 8 cannot appear in cell (9, 7)."),
        }));
    }
}
