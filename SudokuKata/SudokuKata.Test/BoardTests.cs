using System;
using ApprovalTests;
using NUnit.Framework;

namespace SudokuKata.Test;
[TestFixture]
internal class BoardTests
{
    [Test]
    public void EmptyBoardToString() => Approvals.Verify(new CharArrayBoard());

    [Test]
    public void EmptyBoardCode() => Approvals.Verify(new CharArrayBoard().Code);

    [Test]
    public void RandomBoard()
    {
        var rng = new Random(1);
        var sut = new CharArrayBoard(new RandomBoard(rng).State);
        Approvals.Verify((sut, sut.Code));
    }

    [Test]
    public void SetCellToDigit()
    {
        var sut = new CharArrayBoard();
        sut.Set(1, 2, 3);
        Approvals.Verify(sut);
    }

    [Test]
    public void ClearCell()
    {
        var sut = new CharArrayBoard();
        sut.Set(1, 2, 3);
        sut.Set(1, 2, 0);
        Approvals.Verify(sut);
    }

    [Test]
    public void ConstructFromStateArray()
    {
        var state = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        var sut = new CharArrayBoard(state);
        Approvals.Verify((sut, sut.Code));
    }

    [Test]
    public void ConstructFromStateArray_StateIsPreserved()
    {
        var state = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
        var sut = new CharArrayBoard(state);
        Assert.That(sut.State, Is.EqualTo(state));
    }

    [Test]
    public void InitialSateIsAllEmpty()
    {
        var sut = new CharArrayBoard();
        Assert.That(sut.State, Is.EqualTo(new int[9 * 9]));
    }

    [Test]
    public void SetSetsTheSate()
    {
        var sut = new CharArrayBoard();
        sut.Set(1, 2, 3);
        Assert.That(sut.State, Is.Not.EqualTo(new int[9 * 9]));
        Assert.That(sut.State[1 * 9 + 2], Is.EqualTo(3));
    }

    [Test]
    public void StateSetExtensionMethod()
    {
        int[] sut = new int[9 * 9];
        sut.Set(1, 2, 3);
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
}
