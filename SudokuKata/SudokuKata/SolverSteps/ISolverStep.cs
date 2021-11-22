namespace SudokuKata.SolverSteps;

public interface ISolverStep
{
    IEnumerable<ISolverCommand> Execute();
}

public interface ISolverStep<TDetection> : ISolverStep
{
    IReadOnlyList<TDetection> Detect();
    IEnumerable<ISolverCommand> Act(TDetection detection);
    IEnumerable<TDetection> Pick(IReadOnlyList<TDetection> detections);

    IEnumerable<ISolverCommand> ISolverStep.Execute() => Pick(Detect()).SelectMany(detection => Act( detection ));
}
