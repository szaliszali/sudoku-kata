namespace SudokuKata.Test;

internal class CalculateCandidatesTests
{
    [Test]
    public void CalculateCandidatesEmptyBoard()
    {
        var emptyBoard = new Board();
        var sut = new CandidatesForEachCell(emptyBoard);

        CandidateSet expected = new CandidateSet();
        expected.IncludeAll();

        Assert.That(sut.Get(1, 1), Is.EqualTo(expected));
    }

    [Test]
    public void CalculateCandidatesForFilledCellAreEmpty()
    {
        var board = new Board();
        board.Set(3, 3, 3);
        var sut = new CandidatesForEachCell(board);

        Assert.That(sut.Get(3, 3).NumCandidates, Is.Zero);
    }

    [Test]
    public void CalculateCandidatesForEmptyCell()
    {
        var board = new Board(@"
            ...|...|...
            ...|.3.|...
            ...|...|...
            ---+---+---
            ...|...|...
            ...|...|..4
            ...|..6|...
            ---+---+---
            ...|...|...
            ...|...|...
            ...|...|...");
        var sut = new CandidatesForEachCell(board);

        var expected = new CandidateSet();
        expected.IncludeAll();
        expected.Exclude(3);
        expected.Exclude(4);
        expected.Exclude(6);

        Assert.That(sut.Get(4, 4), Is.EqualTo(expected));
    }

    [Test]
    public void PreserveState()
    {
        var board = new Board();
        var sut = new CandidatesForEachCell(board);

        sut.Get(0, 0).Exclude(1);

        Assert.That(sut.Get(0, 0).NumCandidates, Is.EqualTo(8));
    }
}
