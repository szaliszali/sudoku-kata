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
        var groupWithNMasks = detection;

        CandidateSet mask = groupWithNMasks.Mask;

        if (groupWithNMasks.Cells
            .Any(cell =>
                solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(mask) &&
                solverState.Candidates.Get(cell.Location).HasAtLeastOneDifferent(mask)))
        {
            StringBuilder message = new StringBuilder();
            message.Append($"In {groupWithNMasks.Description} values ");
            message.AppendJoin($", ", mask.AllCandidates);
            message.Append(" appear only in cells ");
            message.AppendJoin(" ", groupWithNMasks.CellsWithMask.Select(cell => cell.Location.ShortString()));

            message.Append(" and other values cannot appear in those cells.");

            yield return new PrintMessageCommand(message.ToString());
        }

        foreach (var cell in groupWithNMasks.CellsWithMask)
        {
            if (!solverState.Candidates.Get(cell.Location).HasAtLeastOneDifferent(groupWithNMasks.Mask))
                continue;

            var valuesToClear = solverState.Candidates.Get(cell.Location).AllCandidates.Except(groupWithNMasks.Mask.AllCandidates).ToArray();
            yield return new EliminateCandidatesCommand(cell.Location, valuesToClear);

            yield return new PrintMessageCommand(new StringBuilder().AppendJoin(", ", valuesToClear).Append($" cannot appear in cell {cell.Location.ShortString()}.").ToString());
        }
    }

    IReadOnlyList<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection> ISolverStep<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection>.Detect() =>
        CandidateSet.AllPossibleCandidateSets
            .Where(cs => cs.NumCandidates > 1)
            .SelectMany(mask =>
                solverState.CellGroups
                    .Where(group => group.All(cell =>
                        solverState.Board.Get(cell.Location) == 0 || (!mask.Contains(solverState.Board.Get(cell.Location)))))
                    .Select(group => new GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection(
                        Mask: mask,
                        Description: group.First().Description,
                        Cells: group,
                        CellsWithMask:
                            group.Where(cell => solverState.Board.Get(cell.Location) == 0 && solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(mask))
                                .ToList(),
                        CleanableCellsCount:
                            group.Count(
                                cell => solverState.Board.Get(cell.Location) == 0 &&
                                        solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(mask) &&
                                        solverState.Candidates.Get(cell.Location).HasAtLeastOneDifferent(mask))
                    )))
                .Where(group => group.CellsWithMask.Count() == group.Mask.NumCandidates)
                .ToList();

    IEnumerable<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection> ISolverStep<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection>.Pick(IReadOnlyList<GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection> detections) =>
        detections;
}
