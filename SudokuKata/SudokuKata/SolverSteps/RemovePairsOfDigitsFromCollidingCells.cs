﻿namespace SudokuKata.SolverSteps;

public class RemovePairsOfDigitsFromCollidingCells : ISolverStep<RemovePairsOfDigitsFromCollidingCellsDetection>
{
    private SolverState solverState;

    public RemovePairsOfDigitsFromCollidingCells(SolverState solverState)
    {
        this.solverState = solverState;
    }

    public static IEnumerable<ISolverCommand> Solve(SolverState solverState)
    {
        ISolverStep<RemovePairsOfDigitsFromCollidingCellsDetection> step = new RemovePairsOfDigitsFromCollidingCells(solverState);
        return step.Act(step.Detect());
    }

    IEnumerable<ISolverCommand> ISolverStep<RemovePairsOfDigitsFromCollidingCellsDetection>.Act(IReadOnlyList<RemovePairsOfDigitsFromCollidingCellsDetection> detections)
    {
        foreach (var group in detections)
        {
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
                        .Select(group => new RemovePairsOfDigitsFromCollidingCellsDetection(mask, @group.First().Description, @group)))
                .ToList();
    }
}
