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
}
