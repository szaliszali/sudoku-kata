namespace SudokuKata.Test;

[TestFixture]
internal class BoardTests
{
    [Test]
    public void EmptyBoardToString() => Approvals.Verify(new Board());

    [Test]
    public void EmptyBoardCode() => Approvals.Verify(new Board().Code);

    [Test]
    public void RandomBoard()
    {
        var rng = new Random(1);
        var sut = new Board(new RandomBoard(rng).State);
        Approvals.Verify((sut, sut.Code));
    }

    [Test]
    public void SetCellToDigit()
    {
        var sut = new Board();
        sut.Set(1, 2, 3);
        Approvals.Verify(sut);
    }

    [Test]
    public void GetCell()
    {
        var sut = new Board();
        sut.Set(1, 2, 3);
        Assert.Multiple(() =>
        {
            Assert.That(sut.Get(0, 0), Is.Zero, "empty cell");
            Assert.That(sut.Get(1, 2), Is.EqualTo(3), "digit");
        });
    }

    [Test]
    public void ClearCell()
    {
        var sut = new Board();
        sut.Set(1, 2, 3);
        sut.Set(1, 2, 0);
        Approvals.Verify(sut);
    }

    [Test]
    public void ConstructFromStateArray()
    {
        var state = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        var sut = new Board(state);
        Approvals.Verify((sut, sut.Code));
    }

    [Test]
    public void ConstructFromStateArray_StateIsPreserved()
    {
        var state = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        var sut = new Board(state);
        Assert.That(sut.State, Is.EqualTo(state));
    }

    [Test]
    public void InitialSateIsAllEmpty()
    {
        var sut = new Board();
        Assert.That(sut.State, Is.EqualTo(new int[9 * 9]));
    }

    [Test]
    public void SetSetsTheSate()
    {
        var sut = new Board();
        sut.Set(1, 2, 3);
        Assert.That(sut.State, Is.Not.EqualTo(new int[9 * 9]));
        Assert.That(sut.State[1 * 9 + 2], Is.EqualTo(3));
    }

    [Test]
    public void StateSetExtensionMethod()
    {
        int[] sut = new int[9 * 9];
        sut.Set(0, 0, 1);
        sut.Set(1, 2, 3);
        Assert.That(sut[0], Is.EqualTo(1));
        Assert.That(sut[11], Is.EqualTo(3));
    }

    [Test]
    public void StateGetExtensionMethod()
    {
        int[] sut = new int[9 * 9];
        sut.Set(1, 2, 3);
        Assert.Multiple(() =>
        {
            Assert.That(sut.Get(1, 1), Is.EqualTo(0));
            Assert.That(sut.Get(1, 2), Is.EqualTo(3));
        });
    }

    [Test]
    public void AllLocations()
    {
        var sut = new Board();

        Approvals.VerifyAll(sut.AllLocations(), "");
    }

    [Test]
    public void Size()
    {
        var sut = new Board();

        Assert.That(sut.Size, Is.EqualTo(9));
    }

    [Test]
    public void Clone_PreservesState()
    {
        var sut = new Board();
        var cloned = sut.Clone();

        Assert.That(sut.State, Is.EqualTo(cloned.State));
    }

    [Test]
    public void Clone_CreatesACopy()
    {
        var sut = new Board();
        var cloned = sut.Clone();
        sut.Set(1, 2, 3);

        Assert.That(sut.State, Is.Not.EqualTo(cloned.State));
    }

    [Test]
    public void IsSolved()
    {
        var emptyBoard = new Board();
        var partiallySolvedBoard = new Board();
        partiallySolvedBoard.Set(1, 2, 3);
        var fullySolvedBoard = new Board(@"
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
            +---+---+---+");

        Assert.Multiple(() =>
        {
            Assert.That(emptyBoard.IsSolved(), Is.False, "empty board");
            Assert.That(partiallySolvedBoard.IsSolved(), Is.False, "partially solved");
            Assert.That(fullySolvedBoard.IsSolved(), Is.True, "fully solved");
        });
    }
}
