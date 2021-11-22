namespace SudokuKata.SolverSteps;

public class NumberCanOnlyAppearInOnePlace : ISolverStep<NumberCanOnlyAppearInOnePlaceDetection>
{
    private SolverState solverState;

    public NumberCanOnlyAppearInOnePlace(SolverState solverState)
    {
        this.solverState = solverState;
    }

    public static IEnumerable<ISolverCommand> Solve(SolverState solverState)
    {
        ISolverStep<NumberCanOnlyAppearInOnePlaceDetection> step = new NumberCanOnlyAppearInOnePlace(solverState);
        return step.Execute();
    }

    IEnumerable<ISolverCommand> ISolverStep<NumberCanOnlyAppearInOnePlaceDetection>.Act(IReadOnlyList<NumberCanOnlyAppearInOnePlaceDetection> detections)
    {
        if (detections.Count > 0)
        {
            (string description, CellLocation location, int digit) = detections.PickOneRandomly(solverState.Rng);

            yield return new SetCellCommand(location, digit);

            yield return new PrintMessageCommand($"{description} can contain {digit} only at {location.ShortString()}.");
        }
    }

    IReadOnlyList<NumberCanOnlyAppearInOnePlaceDetection> ISolverStep<NumberCanOnlyAppearInOnePlaceDetection>.Detect()
    {
        return
            Enumerable.Range(1, solverState.Board.Size)
                .SelectMany(digit => solverState.CellGroups
                    .Select(g => (g, count: g.Count(c => solverState.Candidates.Get(c.Location).Contains(digit)), digit))
                    .Where(g => g.count == 1))
                .OrderBy(g => g.digit)
                .ThenBy(g => g.g.First().Discriminator % solverState.Board.Size) // HACK: original code enumerated cell groups in different order
                .Select(g => new NumberCanOnlyAppearInOnePlaceDetection(g.g.First().Description.Capitalize(), g.g.Single(c => solverState.Candidates.Get(c.Location).Contains(g.digit)).Location, g.digit))
                .ToList();
    }
}
