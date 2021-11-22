namespace SudokuKata.SolverSteps;

public interface ISolverStep
{
    IEnumerable<ISolverCommand> Execute();
}

public interface ISolverStep<TDetection> : ISolverStep
{
    IReadOnlyList<TDetection> Detect();
    IEnumerable<ISolverCommand> Act(IReadOnlyList<TDetection> detections);

    IEnumerable<ISolverCommand> ISolverStep.Execute() => Act(Detect());
}
