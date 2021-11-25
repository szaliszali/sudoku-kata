using System.Text;

namespace SudokuKata.SolverSteps;

public class GroupsOfDigitsOfSizeNWhichOnlyAppearInNCells : ISolverStep<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection>
{
    // When a set of N digits only appears in N cells within row/column/block, then no other digit can appear in the same set of cells
    // All other candidates can then be removed from those cells

    private readonly SolverState solverState;

    public GroupsOfDigitsOfSizeNWhichOnlyAppearInNCells(SolverState solverState)
    {
        this.solverState = solverState;
    }

    IEnumerable<ISolverCommand> ISolverStep<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection>.Act(GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection detection)
    {
        var groupWithNCandidates = detection;

        CandidateSet candidates = groupWithNCandidates.Candidates;

        if (groupWithNCandidates.Cells
            .Any(cell =>
                solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(candidates) &&
                solverState.Candidates.Get(cell.Location).HasAtLeastOneDifferent(candidates)))
        {
            yield return new PrintMessageCommand(new StringBuilder()
                .Append($"In {groupWithNCandidates.Description} values ")
                .AppendJoin(", ", candidates.AllCandidates)
                .Append(" appear only in cells ")
                .AppendJoin(" ", groupWithNCandidates.CellsWithMask.Select(cell => cell.Location.ShortString()))
                .Append(" and other values cannot appear in those cells.")
                .ToString());
        }

        foreach (var cell in groupWithNCandidates.CellsWithMask
            .Where(cell => solverState.Candidates.Get(cell.Location).HasAtLeastOneDifferent(candidates))
            .Select(cell=>cell.Location))
        {
            var valuesToClear = solverState.Candidates.Get(cell).AllCandidates.Except(candidates.AllCandidates).ToArray();

            yield return new EliminateCandidatesCommand(cell, valuesToClear);
            yield return new PrintMessageCommand(new StringBuilder().AppendJoin(", ", valuesToClear).Append($" cannot appear in cell {cell.ShortString()}.").ToString());
        }
    }

    IReadOnlyList<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection> ISolverStep<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection>.Detect() =>
        CandidateSet.AllPossibleCandidateSets
            .Where(cs => cs.NumCandidates > 1)
            .SelectMany(candidates =>
                solverState.CellGroups
                    .Where(group => group.All(cell =>
                        solverState.Board.Get(cell.Location) == 0 || (!candidates.Contains(solverState.Board.Get(cell.Location)))))
                    .Select(group => new GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection(
                        Candidates: candidates,
                        Description: group.First().Description,
                        Cells: group,
                        CellsWithMask:
                            group.Where(cell => solverState.Board.Get(cell.Location) == 0 && solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(candidates))
                                .ToList(),
                        CleanableCellsCount:
                            group.Count(
                                cell => solverState.Board.Get(cell.Location) == 0 &&
                                        solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(candidates) &&
                                        solverState.Candidates.Get(cell.Location).HasAtLeastOneDifferent(candidates)))))
            .Where(group => group.CellsWithMask.Count() == group.Candidates.NumCandidates)
            .ToList();

    IEnumerable<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection> ISolverStep<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection>.Pick(IReadOnlyList<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection> detections) =>
        detections;
}
