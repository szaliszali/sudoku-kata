namespace SudokuKata.SolverSteps;

public class RemovePairsOfDigitsFromCollidingCells : ISolverStep<RemovePairsOfDigitsFromCollidingCellsDetection>
{
    private readonly SolverState solverState;

    public RemovePairsOfDigitsFromCollidingCells(SolverState solverState)
    {
        this.solverState = solverState;
    }

    IEnumerable<ISolverCommand> ISolverStep<RemovePairsOfDigitsFromCollidingCellsDetection>.Act(RemovePairsOfDigitsFromCollidingCellsDetection detection)
    {
        var cells =
            detection.Cells
                .Where(
                    cell =>
                        solverState.Candidates.Get(cell.Location) != detection.Mask &&
                        solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(detection.Mask))
                .ToList();

        var maskCells =
            detection.Cells
                .Where(cell => solverState.Candidates.Get(cell.Location) == detection.Mask)
                .ToArray();


        if (cells.Any())
        {
            CandidateSet temp = detection.Mask;
            (int lower, int upper) = temp.CandidatePair;

            yield return new PrintMessageCommand(
                $"Values {lower} and {upper} in {detection.Description} are in cells {maskCells[0].Location.ShortString()} and {maskCells[1].Location.ShortString()}.");

            foreach (var cell in cells)
            {
                List<int> valuesToRemove = solverState.Candidates.Get(cell.Location).AllCandidates
                    .Intersect(detection.Mask.AllCandidates)
                    .ToList();

                yield return new EliminateCandidatesCommand(cell.Location, valuesToRemove);
                yield return new PrintMessageCommand($"{string.Join(", ", valuesToRemove)} cannot appear in {cell.Location.ShortString()}.");
            }
        }
    }

    IReadOnlyList<RemovePairsOfDigitsFromCollidingCellsDetection> ISolverStep<RemovePairsOfDigitsFromCollidingCellsDetection>.Detect() =>
        solverState.Candidates
            .Where(mask => mask.NumCandidates == 2)
            .Distinct()
            .SelectMany(mask =>
                solverState.CellGroups
                    .Where(group => group.Count(tuple => solverState.Candidates.Get(tuple.Location) == mask) == 2)
                    .Where(group => group.Any(tuple => solverState.Candidates.Get(tuple.Location) != mask && solverState.Candidates.Get(tuple.Location).HasAtLeastOneCommon(mask)))
                    .Select(group => new RemovePairsOfDigitsFromCollidingCellsDetection(mask, group.First().Description, group)))
            .ToList();

    IEnumerable<RemovePairsOfDigitsFromCollidingCellsDetection> ISolverStep<RemovePairsOfDigitsFromCollidingCellsDetection>.Pick(IReadOnlyList<RemovePairsOfDigitsFromCollidingCellsDetection> detections) =>
        detections;
}
