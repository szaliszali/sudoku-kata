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
        var sut = new RandomBoard(rng);
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
}
