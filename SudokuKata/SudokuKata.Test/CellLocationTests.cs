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

    [Test]
    [TestCase(2, 5)]
    [TestCase(3, 10)]
    [TestCase(4, 17)]
    public void Index(int blockSize, int expectedIndex)
    {
        var board = new Board(blockSize);
        var sut = new CellLocation(board, 1, 1);

        Assert.That(sut.Index(), Is.EqualTo(expectedIndex));
    }
}
