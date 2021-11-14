using System;
using System.Collections.Generic;
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
        var stateStack = new Stack<int[]>();
        var sut = new RandomBoard(rng, stateStack);
        Approvals.Verify((sut, sut.Code));
    }
}
