namespace SudokuKata.SolverSteps;

internal interface ISolverStep<TDetection>
{
    IReadOnlyList<TDetection> Detect();
    IEnumerable<ISolverCommand> Act(IReadOnlyList<TDetection> detections);
}
