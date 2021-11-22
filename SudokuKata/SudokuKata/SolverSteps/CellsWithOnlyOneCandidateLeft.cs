namespace SudokuKata.SolverSteps;

public class CellsWithOnlyOneCandidateLeft : ISolverStep<CellLocation>
{
    private SolverState solverState;

    public CellsWithOnlyOneCandidateLeft(SolverState solverState)
    {
        this.solverState = solverState;
    }

    public static IEnumerable<ISolverCommand> Solve(SolverState solverState)
    {
        ISolverStep<CellLocation> step = new CellsWithOnlyOneCandidateLeft(solverState);
        return step.Execute();
    }

    IEnumerable<ISolverCommand> ISolverStep<CellLocation>.Act(IReadOnlyList<CellLocation> detections)
    {
        if (detections.Count > 0)
        {
            CellLocation location = detections.PickOneRandomly(solverState.Rng);

            int candidate = solverState.Candidates.Get(location).SingleCandidate;

            yield return new SetCellCommand(location, candidate);

            yield return new PrintMessageCommand(string.Format("{0} can only contain {1}.", location.ShortString(), candidate));
        }
    }

    IReadOnlyList<CellLocation> ISolverStep<CellLocation>.Detect()
    {
        return solverState.Candidates
            .Zip(solverState.Board.AllLocations(), (c, l) => (Location: l, CandidatesCount: c.NumCandidates))
            .Where(tuple => tuple.CandidatesCount == 1)
            .Select(tuple => tuple.Location)
            .ToArray();
    }
}
