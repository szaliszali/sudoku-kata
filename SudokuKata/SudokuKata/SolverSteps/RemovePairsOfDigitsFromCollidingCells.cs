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
        var group = detection;

        var cells =
            group.Cells
                .Where(
                    cell =>
                        solverState.Candidates.Get(cell.Location) != group.Mask &&
                        solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(group.Mask))
                .ToList();

        var maskCells =
            group.Cells
                .Where(cell => solverState.Candidates.Get(cell.Location) == group.Mask)
                .ToArray();


        if (cells.Any())
        {
            CandidateSet temp = group.Mask;
            (int lower, int upper) = temp.CandidatePair;

            yield return new PrintMessageCommand(
                $"Values {lower} and {upper} in {group.Description} are in cells {maskCells[0].Location.ShortString()} and {maskCells[1].Location.ShortString()}.");

            foreach (var cell in cells)
            {
                List<int> valuesToRemove = solverState.Candidates.Get(cell.Location).AllCandidates.Intersect(group.Mask.AllCandidates).ToList();
                yield return new EliminateCandidatesCommand(cell.Location, valuesToRemove);

                string valuesReport = string.Join(", ", valuesToRemove.ToArray());
                yield return new PrintMessageCommand($"{valuesReport} cannot appear in {cell.Location.ShortString()}.");
            }
        }
    }

    IReadOnlyList<RemovePairsOfDigitsFromCollidingCellsDetection> ISolverStep<RemovePairsOfDigitsFromCollidingCellsDetection>.Detect()
    {
        IEnumerable<CandidateSet> twoDigitMasks =
            solverState.Candidates.Where(mask => mask.NumCandidates == 2).Distinct().ToList();

        return
            twoDigitMasks
                .SelectMany(mask =>
                    solverState.CellGroups
                        .Where(group => group.Count(tuple => solverState.Candidates.Get(tuple.Location) == mask) == 2)
                        .Where(group => group.Any(tuple => solverState.Candidates.Get(tuple.Location) != mask && solverState.Candidates.Get(tuple.Location).HasAtLeastOneCommon(mask)))
                        .Select(group => new RemovePairsOfDigitsFromCollidingCellsDetection(mask, group.First().Description, group)))
                .ToList();
    }

    IEnumerable<RemovePairsOfDigitsFromCollidingCellsDetection> ISolverStep<RemovePairsOfDigitsFromCollidingCellsDetection>.Pick(IReadOnlyList<RemovePairsOfDigitsFromCollidingCellsDetection> detections)
    {
        return detections;
    }
}
