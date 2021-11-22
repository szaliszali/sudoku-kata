namespace SudokuKata.SolverSteps;

public interface ISolverStep<TDetection>
{
    IReadOnlyList<TDetection> Detect();
    IEnumerable<ISolverCommand> Act(IReadOnlyList<TDetection> detections);

    IEnumerable<ISolverCommand> Execute() => Act(Detect());
}
