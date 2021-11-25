namespace SudokuKata.Test;

internal class CellLocationTests
{
    [Test]
    public void BlockIndex()
    {
        Approvals.VerifyAll(new Board().AllLocations().Select(l => l.BlockIndex()), "");
    }

    [Test]
    public void BlockRow()
    {
        Approvals.VerifyAll(new Board().AllLocations().Select(l => l.BlockRow()), "");
    }

    [Test]
    public void BlockCol()
    {
        Approvals.VerifyAll(new Board().AllLocations().Select(l => l.BlockCol()), "");
    }
}
