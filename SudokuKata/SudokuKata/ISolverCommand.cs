namespace SudokuKata;

public interface ISolverCommand
{
    void Execute(SolverState state);
}
