﻿namespace SudokuKata.SolverSteps;

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
                        solverState.Candidates.Get(cell.Location) != detection.Candidates &&
                        solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(detection.Candidates))
                .ToList();

        var cellsWithSameCandidates =
            detection.Cells
                .Where(cell => solverState.Candidates.Get(cell.Location) == detection.Candidates)
                .ToArray();

        if (cells.Any())
        {
            CandidateSet temp = detection.Candidates;
            (int lower, int upper) = temp.CandidatePair;

            yield return new PrintMessageCommand(
                $"Values {lower} and {upper} in {detection.Description} are in cells {cellsWithSameCandidates[0].Location.ShortString()} and {cellsWithSameCandidates[1].Location.ShortString()}.");

            foreach (var cell in cells)
            {
                List<int> valuesToRemove = solverState.Candidates.Get(cell.Location).AllCandidates
                    .Intersect(detection.Candidates.AllCandidates)
                    .ToList();

                yield return new EliminateCandidatesCommand(cell.Location, valuesToRemove);
                yield return new PrintMessageCommand($"{string.Join(", ", valuesToRemove)} cannot appear in {cell.Location.ShortString()}.");
            }
        }
    }

    IReadOnlyList<RemovePairsOfDigitsFromCollidingCellsDetection> ISolverStep<RemovePairsOfDigitsFromCollidingCellsDetection>.Detect() =>
        solverState.Candidates
            .Where(candidateSet => candidateSet.NumCandidates == 2)
            .Distinct()
            .SelectMany(candidates =>
                solverState.CellGroups
                    .Where(group => group.Count(tuple => solverState.Candidates.Get(tuple.Location) == candidates) == 2)
                    .Where(group => group.Any(tuple => solverState.Candidates.Get(tuple.Location) != candidates && solverState.Candidates.Get(tuple.Location).HasAtLeastOneCommon(candidates)))
                    .Select(group => new RemovePairsOfDigitsFromCollidingCellsDetection(candidates, group.First().Description, group)))
            .ToList();

    IEnumerable<RemovePairsOfDigitsFromCollidingCellsDetection> ISolverStep<RemovePairsOfDigitsFromCollidingCellsDetection>.Pick(IReadOnlyList<RemovePairsOfDigitsFromCollidingCellsDetection> detections) =>
        detections;
}
