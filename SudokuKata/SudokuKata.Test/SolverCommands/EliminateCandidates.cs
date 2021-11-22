using NUnit.Framework;

namespace SudokuKata.Test.SolverCommands;

internal class EliminateCandidates
{
    [Test]
    public void Wip()
    {
        var sut = new EliminateCandidatesCommand(new CellLocation(1, 2), new[] { 3, 4 });
    }
}
