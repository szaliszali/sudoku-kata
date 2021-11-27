namespace SudokuKata.Test;

internal class CellDisplayTest
{
    [Test]
    public void DigitToCharacter()
    {
        Approvals.VerifyAll(Enumerable.Range(0, 10).Select(CellDisplay.ToDisplay), "");
    }
}
