namespace SudokuKata.Test.SolverCommands;

internal class SetCell
{
    [Test]
    public void SetCellSetsCellToGivenDigit()
    {
        var board = new Board();
        var solverState = new SolverState(board, new Random());
        solverState.RefreshCandidates();

        ISolverCommand sut = new SetCellCommand(new CellLocation(1, 2), 3);
        sut.Execute(solverState);

        Assert.That(board.Get(1, 2), Is.EqualTo(3));
    }
}
