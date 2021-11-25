namespace SudokuKata.Test;

internal class CellLocationTests
{
    [Test]
    public void BlockIndex()
    {
        var board = new Board();
        Approvals.VerifyAll(board.AllLocations().Select(l => l.BlockIndex()), "");
    }

    [Test]
    public void BlockRow()
    {
        var board = new Board();
        Approvals.VerifyAll(board.AllLocations().Select(l => l.BlockRow()), "");
    }

    [Test]
    public void BlockCol()
    {
        var board = new Board();
        Approvals.VerifyAll(board.AllLocations().Select(l => l.BlockCol()), "");
    }
}
