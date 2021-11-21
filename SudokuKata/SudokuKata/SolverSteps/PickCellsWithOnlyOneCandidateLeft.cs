namespace SudokuKata.SolverSteps;

public class PickCellsWithOnlyOneCandidateLeft
{
    public static IEnumerable<ISolverCommand> Solve(SolverState solverState)
    {
        CellLocation[] singleCandidateIndices =
    solverState.Candidates.Zip(solverState.Board.AllLocations(), (c, l) => (Location: l, CandidatesCount: c.NumCandidates))
        .Where(tuple => tuple.CandidatesCount == 1)
        .Select(tuple => tuple.Location)
        .ToArray();

        if (singleCandidateIndices.Length > 0)
        {
            int pickSingleCandidateIndex = solverState.Rng.Next(singleCandidateIndices.Length);
            CellLocation location = singleCandidateIndices[pickSingleCandidateIndex];

            int candidate = solverState.Candidates.Get(location).SingleCandidate;

            yield return new SetCellCommand(location, candidate);

            Console.WriteLine("{0} can only contain {1}.", location.ShortString(), candidate);
        }
    }
}
