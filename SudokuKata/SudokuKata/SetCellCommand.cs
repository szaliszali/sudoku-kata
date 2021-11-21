namespace SudokuKata;

internal record SetCellCommand(CellLocation Location, int Digit) : ISolverCommand
{
    void ISolverCommand.Execute(SolverState state)
    {
        state.SetCell(Location, Digit);
    }
}
