namespace SudokuKata;

internal record EliminateCandidatesCommand(CellLocation Location, IReadOnlyList<int> Digits) : ISolverCommand
{
    void ISolverCommand.Execute(SolverState state)
    {
        state.EliminateCandidates(Location, Digits);
    }
}
