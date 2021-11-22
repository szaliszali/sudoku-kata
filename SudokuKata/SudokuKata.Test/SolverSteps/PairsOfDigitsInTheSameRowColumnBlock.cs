namespace SudokuKata.Test.SolverSteps;

internal class PairsOfDigitsInTheSameRowColumnBlock
{
    [Test]
    public void NoPairsToEliminateFromOtherCells_EmptyResult()
    {
        var board = new Board(@"
            +---+---+---+
            |...|...|...|
            |...|2..|...|
            |...|...|...|
            +---+---+---+
            |.1.|...|...|
            |...|...|...|
            |...|...|.1.|
            +---+---+---+
            |...|..1|...|
            |...|...|...|
            |...|...|...|
            +---+---+---+");
        var solverState = new SolverState(board, new Random());
        solverState.RefreshCandidates();

        var result = SudokuKata.SolverSteps.TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells.Solve(solverState);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void ATestWithOneCellEliminated()
    {
        var board = new Board(@"
            +---+---+---+
            |971|.2.|.8.|
            |832|7.5|19.|
            |546|819|..2|
            +---+---+---+
            |4..|..1|2..|
            |327|5..|9.1|
            |1..|..2|..8|
            +---+---+---+
            |2..|..6|457|
            |654|.97|8.3|
            |7..|.5.|6.9|
            +---+---+---+");
        var solverState = new SolverState(board, new Random());
        solverState.RefreshCandidates();

        var result = SudokuKata.SolverSteps.TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells.Solve(solverState);
        ISolverCommand[] expected = new ISolverCommand[] {
            new PrintMessageCommand("Values 3 and 7 in block (1, 3) are in cells (3, 7) and (3, 8)."),
            new EliminateCandidatesCommand(new CellLocation(0,6), new[]{ 3 }),
            new PrintMessageCommand("3 cannot appear in (1, 7)."),
        };
        // reason of duplication: normally, the algorithm finds each pair twice, and depends on mutating the candidates during evaluation, and the first iteration would eliminate the elements, the second would not enumerate anything
        Assert.That(result, Is.EqualTo(expected.Concat(expected)));
    }

    [Test]
    public void ATestWithSeveralCellsEliminated()
    {
        var board = new Board(@"
            +---+---+---+
            |586|.79|2..|
            |4.1|5.2|.7.|
            |2..|...|5..|
            +---+---+---+
            |6..|...|..2|
            |748|2.5|39.|
            |123|89.|.5.|
            +---+---+---+
            |364|...|925|
            |8..|92.|...|
            |912|65.|..8|
            +---+---+---+");
        var solverState = new SolverState(board, new Random());
        solverState.RefreshCandidates();

        var result = SudokuKata.SolverSteps.TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells.Solve(solverState);
        ISolverCommand[] expected = new ISolverCommand[] {
            new PrintMessageCommand("Values 5 and 7 in row #8 are in cells (8, 2) and (8, 3)."),
            new EliminateCandidatesCommand(new CellLocation(7, 5), new[]{ 7 }),
            new PrintMessageCommand("7 cannot appear in (8, 6)."),
            new EliminateCandidatesCommand(new CellLocation(7, 6), new[]{ 7 }),
            new PrintMessageCommand("7 cannot appear in (8, 7)."),
            new EliminateCandidatesCommand(new CellLocation(7, 8), new[]{ 7 }),
            new PrintMessageCommand("7 cannot appear in (8, 9)."),
        };
        // reason of duplication: normally, the algorithm finds each pair twice, and depends on mutating the candidates during evaluation, and the first iteration would eliminate the elements, the second would not enumerate anything
        Assert.That(result, Is.EqualTo(expected.Concat(expected)));
    }
}
