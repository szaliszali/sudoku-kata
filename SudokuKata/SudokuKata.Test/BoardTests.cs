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
}
