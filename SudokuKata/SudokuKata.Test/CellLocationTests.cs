namespace SudokuKata.Test;

internal class CellLocationTests
{
    [Test]
    public void BlockIndex()
    {
        var board = new Board();
        Approvals.VerifyAll(Enumerable.Range(0, 81).Select(i => new CellLocation(board, i / 9, i % 9).BlockIndex()), "");
    }

    [Test]
    public void BlockRow()
    {
        var board = new Board();
        Approvals.VerifyAll(Enumerable.Range(0, 81).Select(i => new CellLocation(board, i / 9, i % 9).BlockRow()), "");
    }

    [Test]
    public void BlockCol()
    {
        var board = new Board();
        Approvals.VerifyAll(Enumerable.Range(0, 81).Select(i => new CellLocation(board, i / 9, i % 9).BlockCol()), "");
    }
}
