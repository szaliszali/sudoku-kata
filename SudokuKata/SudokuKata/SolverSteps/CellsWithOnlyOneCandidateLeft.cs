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
        CellLocation location = detection;

        int candidate = solverState.Candidates.Get(location).SingleCandidate;

        yield return new SetCellCommand(location, candidate);

        yield return new PrintMessageCommand($"{location.ShortString()} can only contain {candidate}.");
    }

    IReadOnlyList<CellLocation> ISolverStep<CellLocation>.Detect()
    {
        return solverState.Candidates
            .Zip(solverState.Board.AllLocations(), (c, l) => (Location: l, CandidatesCount: c.NumCandidates))
            .Where(tuple => tuple.CandidatesCount == 1)
            .Select(tuple => tuple.Location)
            .ToArray();
    }

    IEnumerable<CellLocation> ISolverStep<CellLocation>.Pick(IReadOnlyList<CellLocation> detections)
    {
        if (detections.Count > 0)
            yield return detections.PickOneRandomly(solverState.Rng);
    }
}
