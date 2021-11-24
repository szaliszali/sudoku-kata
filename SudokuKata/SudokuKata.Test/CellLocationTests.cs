namespace SudokuKata.Test;

internal class CellLocationTests
{
    [Test]
    public void BlockIndex()
    {
        Approvals.VerifyAll(Enumerable.Range(0, 81).Select(i => new CellLocation(i / 9, i % 9).BlockIndex()), "");
    }
}
