using NUnit.Framework;

namespace SudokuKata.Test.SolverCommands;

internal class SetCell
{
    [Test]
    public void Wip()
    {
        var sut = new SetCellCommand(new CellLocation(1, 2), 3);
    }
}
