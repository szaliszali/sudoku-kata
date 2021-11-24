namespace SudokuKata.SolverSteps;

public class CellsWithOnlyOneCandidateLeft : ISolverStep<CellLocation>
{
    private readonly SolverState solverState;

    public CellsWithOnlyOneCandidateLeft(SolverState solverState)
    {
        this.solverState = solverState;
    }

    IEnumerable<ISolverCommand> ISolverStep<CellLocation>.Act(CellLocation detection)
    {
        int candidate = solverState.Candidates.Get(detection).SingleCandidate;

        yield return new SetCellCommand(detection, candidate);
        yield return new PrintMessageCommand($"{detection.ShortString()} can only contain {candidate}.");
    }

    IReadOnlyList<CellLocation> ISolverStep<CellLocation>.Detect() =>
        solverState.Candidates
            .Zip(solverState.Board.AllLocations(), (c, l) => (Location: l, CandidatesCount: c.NumCandidates))
            .Where(tuple => tuple.CandidatesCount == 1)
            .Select(tuple => tuple.Location)
            .ToArray();

    IEnumerable<CellLocation> ISolverStep<CellLocation>.Pick(IReadOnlyList<CellLocation> detections) =>
        detections.PickOneRandomly(solverState.Rng);
}
