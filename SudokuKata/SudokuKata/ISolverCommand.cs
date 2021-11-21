namespace SudokuKata;

internal interface ISolverCommand
{
    void Execute(SolverState state);
}
