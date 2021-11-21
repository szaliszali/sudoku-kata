using System.Text;

namespace SudokuKata;

internal class Solver
{
    private Random rng;
    private Board board;
    private int[] finalState;

    private CandidatesForEachCell cellCandidates;
    private readonly List<IGrouping<int, NamedCell>> cellGroups;

    private int TotalCellCount { get; }

    public Solver(Random rng, Board board, int[] finalState)
    {
        this.rng = rng;
        this.board = board;
        TotalCellCount = board.AllLocations().Count();
        this.finalState = finalState;

        cellGroups = BuildACollectionNamedCellGroupsWhichMapsCellIndicesIntoDistinctGroupsRowsColumnsBlocks();
    }

    public void SolveBoard()
    {
        bool changeMade;
        do
        {
            cellCandidates = new CandidatesForEachCell(board);

            bool stepChangeMade;
            do
            {
                changeMade = Apply(PickCellsWithOnlyOneCandidateLeft()) || Apply(TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock());
                stepChangeMade = !changeMade && (
                    Apply(TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells())
                    || Apply(TryToFindGroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsWithinRowColumnBlock()));
            }
            while (stepChangeMade);

            if (!changeMade) changeMade = Apply(LookIfTheBoardHasMultipleSolutions());

            PrintBoardIfChanged(changeMade);
        }
        while (changeMade);
    }

    private List<IGrouping<int, NamedCell>> BuildACollectionNamedCellGroupsWhichMapsCellIndicesIntoDistinctGroupsRowsColumnsBlocks() =>
        board.AllLocations()
            .Select(location => new NamedCell(location.Row, $"row #{location.Row + 1}", location))
            .Concat(board.AllLocations()
                .Select(location => new NamedCell(board.Size + location.Column, $"column #{location.Column + 1}", location)))
            .Concat(board.AllLocations()
                .Select(location => new NamedCell(2 * board.Size + 3 * (location.Row / 3) + location.Column / 3, $"block ({location.Row / 3 + 1}, {location.Column / 3 + 1})", location)))
            .GroupBy(tuple => tuple.Discriminator)
            .ToList();

    private bool Apply(IEnumerable<SetCellCommand> commands)
    {
        var ret = false;
        foreach (var command in commands)
        {
            SetCell(command.Location, command.Digit);
            ret = true;
        }
        return ret;

        void SetCell(CellLocation location, int digit)
        {
            board.Set(location, digit);
            cellCandidates.Get(location).Clear();
        }
    }

    private bool Apply(IEnumerable<EliminateCandidatesCommand> commands)
    {
        var ret = false;
        foreach (var command in commands)
        {
            ExcludeCandidates(command.Location, command.Digits);
            ret = true;
        }
        return ret;

        void ExcludeCandidates(CellLocation location, IEnumerable<int> digits)
        {
            foreach (int digit in digits)
            {
                cellCandidates.Get(location).Exclude(digit);
            }
        }
    }

    private IEnumerable<SetCellCommand> PickCellsWithOnlyOneCandidateLeft()
    {
        CellLocation[] singleCandidateIndices =
            cellCandidates.Zip(board.AllLocations(), (c, l) => (Location: l, CandidatesCount: c.NumCandidates))
                .Where(tuple => tuple.CandidatesCount == 1)
                .Select(tuple => tuple.Location)
                .ToArray();

        if (singleCandidateIndices.Length > 0)
        {
            int pickSingleCandidateIndex = rng.Next(singleCandidateIndices.Length);
            CellLocation location = singleCandidateIndices[pickSingleCandidateIndex];

            int candidate = cellCandidates.Get(location).SingleCandidate;

            yield return new SetCellCommand(location, candidate);

            Console.WriteLine("{0} can only contain {1}.", location.ShortString(), candidate);
        }
    }

    private IEnumerable<SetCellCommand> TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock()
    {
        List<(string groupDescription, CellLocation location, int candidate)> candidates =
            Enumerable.Range(1, board.Size)
                .SelectMany(digit => cellGroups
                    .Select(g => (g, count: g.Count(c => cellCandidates.Get(c.Location).Contains(digit)), digit))
                    .Where(g => g.count == 1))
                .OrderBy(g => g.digit)
                .ThenBy(g => g.g.First().Discriminator % board.Size) // HACK: original code enumerated cell groups in different order
                .Select(g => (description: g.g.First().Description.Capitalize(), location: g.g.Single(c => cellCandidates.Get(c.Location).Contains(g.digit)).Location, g.digit))
                .ToList();

        if (candidates.Count > 0)
        {
            int index = rng.Next(candidates.Count);
            (string description, CellLocation location, int digit) = candidates.ElementAt(index);

            string message = $"{description} can contain {digit} only at {location.ShortString()}.";

            yield return new SetCellCommand(location, digit);

            Console.WriteLine(message);
        }
    }

    private IEnumerable<EliminateCandidatesCommand> TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells()
    {
        IEnumerable<CandidateSet> twoDigitMasks =
            cellCandidates.Where(mask => mask.NumCandidates == 2).Distinct().ToList();

        var groups =
            twoDigitMasks
                .SelectMany(mask =>
                    cellGroups
                        .Where(group => group.Count(tuple => cellCandidates.Get(tuple.Location) == mask) == 2)
                        .Where(group => group.Any(tuple => cellCandidates.Get(tuple.Location) != mask && cellCandidates.Get(tuple.Location).HasAtLeastOneCommon(mask)))
                        .Select(group => new Lol2(mask, @group.First().Description, @group)))
                .ToList();

        if (groups.Any())
        {
            foreach (var group in groups)
            {
                var cells =
                    group.Cells
                        .Where(
                            cell =>
                                cellCandidates.Get(cell.Location) != group.Mask &&
                                cellCandidates.Get(cell.Location).HasAtLeastOneCommon(group.Mask))
                        .ToList();

                var maskCells =
                    group.Cells
                        .Where(cell => cellCandidates.Get(cell.Location) == group.Mask)
                        .ToArray();


                if (cells.Any())
                {
                    CandidateSet temp = group.Mask;
                    (int lower, int upper) = temp.CandidatePair;

                    Console.WriteLine(
                        $"Values {lower} and {upper} in {group.Description} are in cells {maskCells[0].Location.ShortString()} and {maskCells[1].Location.ShortString()}.");

                    foreach (var cell in cells)
                    {
                        List<int> valuesToRemove = cellCandidates.Get(cell.Location).AllCandidates.Intersect(group.Mask.AllCandidates).ToList();
                        yield return new EliminateCandidatesCommand(cell.Location, valuesToRemove);

                        string valuesReport = string.Join(", ", valuesToRemove.ToArray());
                        Console.WriteLine($"{valuesReport} cannot appear in {cell.Location.ShortString()}.");
                    }
                }
            }
        }
    }

    private IEnumerable<EliminateCandidatesCommand> TryToFindGroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsWithinRowColumnBlock()
    {
        // When a set of N digits only appears in N cells within row/column/block, then no other digit can appear in the same set of cells
        // All other candidates can then be removed from those cells

        IEnumerable<CandidateSet> masks =
            CandidateSet.AllPossibleCandidateSets
                .Where(cs => cs.NumCandidates > 1)
                .ToList();

        var groupsWithNMasks =
            masks
                .SelectMany(mask =>
                    cellGroups
                        .Where(group => @group.All(cell =>
                            board.Get(cell.Location) == 0 || (!mask.Contains(board.Get(cell.Location)))))
                        .Select(group => new
                        {
                            Mask = mask,
                            Description = @group.First().Description,
                            Cells = @group,
                            CellsWithMask =
                                @group.Where(cell => board.Get(cell.Location) == 0 && cellCandidates.Get(cell.Location).HasAtLeastOneCommon(mask))
                                    .ToList(),
                            CleanableCellsCount =
                                @group.Count(
                                    cell => board.Get(cell.Location) == 0 &&
                                            cellCandidates.Get(cell.Location).HasAtLeastOneCommon(mask) &&
                                            cellCandidates.Get(cell.Location).HasAtLeastOneDifferent(mask))
                        }))
                .Where(group => @group.CellsWithMask.Count() == @group.Mask.NumCandidates)
                .ToList();

        foreach (var groupWithNMasks in groupsWithNMasks)
        {
            CandidateSet mask = groupWithNMasks.Mask;

            if (groupWithNMasks.Cells
                .Any(cell =>
                    cellCandidates.Get(cell.Location).HasAtLeastOneCommon(mask) &&
                    cellCandidates.Get(cell.Location).HasAtLeastOneDifferent(mask)))
            {
                StringBuilder message = new StringBuilder();
                message.Append($"In {groupWithNMasks.Description} values ");
                message.AppendJoin($", ", mask.AllCandidates);
                message.Append(" appear only in cells");
                foreach (var cell in groupWithNMasks.CellsWithMask)
                {
                    message.Append($" {cell.Location.ShortString()}");
                }

                message.Append(" and other values cannot appear in those cells.");

                Console.WriteLine(message.ToString());
            }

            foreach (var cell in groupWithNMasks.CellsWithMask)
            {
                if (!cellCandidates.Get(cell.Location).HasAtLeastOneDifferent(groupWithNMasks.Mask))
                    continue;

                var valuesToClear = cellCandidates.Get(cell.Location).AllCandidates.Except(groupWithNMasks.Mask.AllCandidates).ToArray();
                yield return new EliminateCandidatesCommand(cell.Location, valuesToClear);

                StringBuilder message = new StringBuilder();
                message.AppendJoin(", ", valuesToClear);
                message.Append($" cannot appear in cell {cell.Location.ShortString()}.");
                Console.WriteLine(message.ToString());
            }
        }
    }

    private IEnumerable<SetCellCommand> LookIfTheBoardHasMultipleSolutions()
    {
        // This is the last chance to do something in this iteration:
        // If this attempt fails, board will not be entirely solved.

        // Try to see if there are pairs of values that can be exchanged arbitrarily
        // This happens when board has more than one valid solution

        Queue<(int index1, int index2, int digit1, int digit2)> candidates = new();

        for (int i = 0; i < TotalCellCount - 1; i++)
        {
            int row = i / board.Size;
            int col = i % board.Size;

            CandidateSet candidateSet = cellCandidates.Get(row, col);
            if (candidateSet.NumCandidates == 2)
            {
                int blockIndex = 3 * (row / 3) + col / 3;
                (int lower, int upper) = candidateSet.CandidatePair;

                for (int j = i + 1; j < TotalCellCount; j++)
                {
                    int row1 = j / board.Size;
                    int col1 = j % board.Size;

                    if (candidateSet == cellCandidates.Get(row1, col1))
                    {
                        int blockIndex1 = 3 * (row1 / 3) + col1 / 3;

                        if (row == row1 || col == col1 || blockIndex == blockIndex1)
                        {
                            candidates.Enqueue((i, j, lower, upper));
                        }
                    }
                }
            }
        }

        // At this point we have the lists with pairs of cells that might pick one of two digits each
        // Now we have to check whether that is really true - does the board have two solutions?

        List<(int stateIndex1, int stateIndex2, int value1, int value2)> solutions = new();

        while (candidates.Any())
        {
            (int index1, int index2, int digit1, int digit2) = candidates.Dequeue();

            int[] alternateState = board.State.ShallowCopy();

            if (finalState[index1] == digit1)
            {
                alternateState[index1] = digit2;
                alternateState[index2] = digit1;
            }
            else
            {
                alternateState[index1] = digit1;
                alternateState[index2] = digit2;
            }

            if (new StackBasedFilledBoardGenerator(rng, alternateState).HasSolution)
            {
                // Board was solved successfully even with two digits swapped
                solutions.Add((index1, index2, digit1, digit2));
            }
        }

        if (solutions.Any())
        {
            int pos = rng.Next(solutions.Count());
            (int index1, int index2, int digit1, int digit2) = solutions.ElementAt(pos);
            int row1 = index1 / board.Size;
            int col1 = index1 % board.Size;
            int row2 = index2 / board.Size;
            int col2 = index2 % board.Size;

            string description =
                row1 == row2 ? $"row #{row1 + 1}"
                : col1 == col2 ? $"column #{col1 + 1}"
                : $"block ({row1 / 3 + 1}, {col1 / 3 + 1})";

            yield return new SetCellCommand(new CellLocation(row1, col1), finalState[index1]);
            yield return new SetCellCommand(new CellLocation(row2, col2), finalState[index2]);

            Console.WriteLine(
                $"Guessing that {digit1} and {digit2} are arbitrary in {description} (multiple solutions): Pick {finalState[index1]}->({row1 + 1}, {col1 + 1}), {finalState[index2]}->({row2 + 1}, {col2 + 1}).");
        }
    }

    private void PrintBoardIfChanged(bool changeMade)
    {
        if (changeMade)
        {
            PrintBoard();
        }
    }

    private void PrintBoard()
    {
        Console.WriteLine(board);
        Console.WriteLine("Code: {0}", board.Code);
        Console.WriteLine();
    }
}
