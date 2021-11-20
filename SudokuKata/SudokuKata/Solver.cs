using System.Text;

namespace SudokuKata;

internal class Solver
{
    private Random rng;
    private CharArrayBoard board;
    private int[] finalState;
    private int[] state;

    private int[] candidateMasks;
    private CandidatesForEachCell candidateMasksNew;
    private readonly List<IGrouping<int, Lol1>> cellGroups;

    public Solver(Random rng, CharArrayBoard board, int[] finalState)
    {
        this.rng = rng;
        this.board = board;
        this.finalState = finalState;

        state = board.State;

        cellGroups = BuildACollectionNamedCellGroupsWhichMapsCellIndicesIntoDistinctGroupsRowsColumnsBlocks();
    }

    public void SolveBoard()
    {
        bool changeMade;
        do
        {
            CalculateCandidatesForCurrentStateOfTheBoard();

            bool stepChangeMade;
            do
            {
                changeMade = PickCellsWithOnlyOneCandidateLeft() || TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock();

                stepChangeMade = false;
                if (!changeMade) stepChangeMade = TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells() || TryToFindGroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsWithinRowColumnBlock();
            }
            while (stepChangeMade);

            if (!changeMade) changeMade = LookIfTheBoardHasMultipleSolutions();

            PrintBoardIfChanged(changeMade);
        }
        while (changeMade);
    }

    private void CalculateCandidatesForCurrentStateOfTheBoard()
    {
        candidateMasksNew = new CandidatesForEachCell(state);
        candidateMasks = Enumerable.Range(0, state.Length).Select(i => new CandidatesForEachCell(state).Get(i / 9, i % 9).RawValue).ToArray();
    }

    private List<IGrouping<int, Lol1>> BuildACollectionNamedCellGroupsWhichMapsCellIndicesIntoDistinctGroupsRowsColumnsBlocks()
    {
        var rowsIndices = state
            .Select((_, index) => new Lol1(index / 9, $"row #{index / 9 + 1}", index, index / 9, index % 9))
            .GroupBy(tuple => tuple.Discriminator);

        var columnIndices = state
            .Select((_, index) => new Lol1(9 + index % 9, $"column #{index % 9 + 1}", index, index / 9, index % 9))
            .GroupBy(tuple => tuple.Discriminator);

        var blockIndices = state
            .Select((_, index) => new
            {
                Row = index / 9,
                Column = index % 9,
                Index = index
            })
            .Select(tuple => new Lol1(18 + 3 * (tuple.Row / 3) + tuple.Column / 3, $"block ({tuple.Row / 3 + 1}, {tuple.Column / 3 + 1})", tuple.Index, tuple.Row, tuple.Column))
            .GroupBy(tuple => tuple.Discriminator);

        return rowsIndices.Concat(columnIndices).Concat(blockIndices).ToList();
    }

    private bool PickCellsWithOnlyOneCandidateLeft()
    {
        bool changeMade = false;

        int[] singleCandidateIndices =
            candidateMasks
                .Select((mask, index) => new
                {
                    CandidatesCount = BitMasks.maskToOnesCount[mask],
                    Index = index
                })
                .Where(tuple => tuple.CandidatesCount == 1)
                .Select(tuple => tuple.Index)
                .ToArray();

        if (singleCandidateIndices.Length > 0)
        {
            int pickSingleCandidateIndex = rng.Next(singleCandidateIndices.Length);
            int singleCandidateIndex = singleCandidateIndices[pickSingleCandidateIndex];
            int candidateMask = candidateMasks[singleCandidateIndex];
            int candidate = BitMasks.singleBitToIndex[candidateMask];

            int row = singleCandidateIndex / 9;
            int col = singleCandidateIndex % 9;

            SetCell(row, col, 1 + candidate);
            changeMade = true;

            Console.WriteLine("({0}, {1}) can only contain {2}.", row + 1, col + 1, candidate + 1);
        }

        return changeMade;
    }

    private bool TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock()
    {
        bool changeMade = false;

        List<(string groupDescription, int candidateRow, int candidateCol, int candidate)> candidates = new();

        for (int digit = 1; digit <= 9; digit++)
        {
            int mask = 1 << (digit - 1);
            for (int cellGroup = 0; cellGroup < 9; cellGroup++)
            {
                int rowNumberCount = 0;
                int indexInRow = 0;

                int colNumberCount = 0;
                int indexInCol = 0;

                int blockNumberCount = 0;
                int indexInBlock = 0;

                for (int indexInGroup = 0; indexInGroup < 9; indexInGroup++)
                {
                    int rowStateIndex = 9 * cellGroup + indexInGroup;
                    int colStateIndex = 9 * indexInGroup + cellGroup;
                    int blockRowIndex = (cellGroup / 3) * 3 + indexInGroup / 3;
                    int blockColIndex = (cellGroup % 3) * 3 + indexInGroup % 3;
                    int blockStateIndex = blockRowIndex * 9 + blockColIndex;

                    if ((candidateMasks[rowStateIndex] & mask) != 0)
                    {
                        rowNumberCount += 1;
                        indexInRow = indexInGroup;
                    }

                    if ((candidateMasks[colStateIndex] & mask) != 0)
                    {
                        colNumberCount += 1;
                        indexInCol = indexInGroup;
                    }

                    if ((candidateMasks[blockStateIndex] & mask) != 0)
                    {
                        blockNumberCount += 1;
                        indexInBlock = indexInGroup;
                    }
                }

                if (rowNumberCount == 1)
                {
                    candidates.Add(($"Row #{cellGroup + 1}", cellGroup, indexInRow, digit));
                }

                if (colNumberCount == 1)
                {
                    candidates.Add((($"Column #{cellGroup + 1}"), indexInCol, cellGroup, digit));
                }

                if (blockNumberCount == 1)
                {
                    int blockRow = cellGroup / 3;
                    int blockCol = cellGroup % 3;

                    candidates.Add(($"Block ({blockRow + 1}, {blockCol + 1})", blockRow * 3 + indexInBlock / 3, blockCol * 3 + indexInBlock % 3, digit));
                }
            } // for (cellGroup = 0..8)
        } // for (digit = 1..9)

        if (candidates.Count > 0)
        {
            int index = rng.Next(candidates.Count);
            (string description, int row, int col, int digit) = candidates.ElementAt(index);

            string message = $"{description} can contain {digit} only at ({row + 1}, {col + 1}).";

            SetCell(row, col, digit);

            changeMade = true;

            Console.WriteLine(message);
        }

        return changeMade;
    }

    private bool TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells()
    {
        bool stepChangeMade = false;

        IEnumerable<int> twoDigitMasks =
            candidateMasks.Where(mask => BitMasks.maskToOnesCount[mask] == 2).Distinct().ToList();

        var groups =
            twoDigitMasks
                .SelectMany(mask =>
                    cellGroups
                        .Where(group => group.Count(tuple => candidateMasks[tuple.Index] == mask) == 2)
                        .Where(group => group.Any(tuple => candidateMasks[tuple.Index] != mask && (candidateMasks[tuple.Index] & mask) > 0))
                        .Select(group => new Lol2(mask, @group.Key, @group.First().Description, @group)))
                .ToList();

        if (groups.Any())
        {
            foreach (var group in groups)
            {
                var cells =
                    group.Cells
                        .Where(
                            cell =>
                                candidateMasks[cell.Index] != group.Mask &&
                                (candidateMasks[cell.Index] & group.Mask) > 0)
                        .ToList();

                var maskCells =
                    group.Cells
                        .Where(cell => candidateMasks[cell.Index] == group.Mask)
                        .ToArray();


                if (cells.Any())
                {
                    int upper = 0;
                    int lower = 0;
                    int temp = group.Mask;

                    int value = 1;
                    while (temp > 0)
                    {
                        if ((temp & 1) > 0)
                        {
                            lower = upper;
                            upper = value;
                        }
                        temp = temp >> 1;
                        value += 1;
                    }

                    Console.WriteLine(
                        $"Values {lower} and {upper} in {group.Description} are in cells ({maskCells[0].Row + 1}, {maskCells[0].Column + 1}) and ({maskCells[1].Row + 1}, {maskCells[1].Column + 1}).");

                    foreach (var cell in cells)
                    {
                        int maskToRemove = candidateMasks[cell.Index] & group.Mask;
                        List<int> valuesToRemove = new List<int>();
                        int curValue = 1;
                        while (maskToRemove > 0)
                        {
                            if ((maskToRemove & 1) > 0)
                            {
                                valuesToRemove.Add(curValue);
                                ExcludeCandidate(cell.Row, cell.Column, curValue);
                            }
                            maskToRemove = maskToRemove >> 1;
                            curValue += 1;
                        }

                        string valuesReport = string.Join(", ", valuesToRemove.ToArray());
                        Console.WriteLine($"{valuesReport} cannot appear in ({cell.Row + 1}, {cell.Column + 1}).");

                        stepChangeMade = true;
                    }
                }
            }
        }

        return stepChangeMade;
    }

    private bool TryToFindGroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsWithinRowColumnBlock()
    {
        bool stepChangeMade = false;

        // When a set of N digits only appears in N cells within row/column/block, then no other digit can appear in the same set of cells
        // All other candidates can then be removed from those cells

        IEnumerable<int> masks =
            BitMasks.maskToOnesCount
                .Where(tuple => tuple.Value > 1)
                .Select(tuple => tuple.Key).ToList();

        var groupsWithNMasks =
            masks
                .SelectMany(mask =>
                    cellGroups
                        .Where(group => @group.All(cell =>
                            state[cell.Index] == 0 || (mask & (1 << (state[cell.Index] - 1))) == 0))
                        .Select(group => new
                        {
                            Mask = mask,
                            Description = @group.First().Description,
                            Cells = @group,
                            CellsWithMask =
                                @group.Where(cell => state[cell.Index] == 0 && (candidateMasks[cell.Index] & mask) != 0)
                                    .ToList(),
                            CleanableCellsCount =
                                @group.Count(
                                    cell => state[cell.Index] == 0 &&
                                            (candidateMasks[cell.Index] & mask) != 0 &&
                                            (candidateMasks[cell.Index] & ~mask) != 0)
                        }))
                .Where(group => @group.CellsWithMask.Count() == BitMasks.maskToOnesCount[@group.Mask])
                .ToList();

        foreach (var groupWithNMasks in groupsWithNMasks)
        {
            int mask = groupWithNMasks.Mask;

            if (groupWithNMasks.Cells
                .Any(cell =>
                    (candidateMasks[cell.Index] & mask) != 0 &&
                    (candidateMasks[cell.Index] & ~mask) != 0))
            {
                StringBuilder message = new StringBuilder();
                message.Append($"In {groupWithNMasks.Description} values ");

                string separator = string.Empty;
                int temp = mask;
                int curValue = 1;
                while (temp > 0)
                {
                    if ((temp & 1) > 0)
                    {
                        message.Append($"{separator}{curValue}");
                        separator = ", ";
                    }

                    temp = temp >> 1;
                    curValue += 1;
                }

                message.Append(" appear only in cells");
                foreach (var cell in groupWithNMasks.CellsWithMask)
                {
                    message.Append($" ({cell.Row + 1}, {cell.Column + 1})");
                }

                message.Append(" and other values cannot appear in those cells.");

                Console.WriteLine(message.ToString());
            }

            foreach (var cell in groupWithNMasks.CellsWithMask)
            {
                int maskToClear = candidateMasks[cell.Index] & ~groupWithNMasks.Mask;
                if (maskToClear == 0)
                    continue;

                stepChangeMade = true;

                int valueToClear = 1;

                string separator = string.Empty;
                StringBuilder message = new StringBuilder();

                while (maskToClear > 0)
                {
                    if ((maskToClear & 1) > 0)
                    {
                        ExcludeCandidate(cell.Row, cell.Column, valueToClear);
                        message.Append($"{separator}{valueToClear}");
                        separator = ", ";
                    }

                    maskToClear = maskToClear >> 1;
                    valueToClear += 1;
                }

                message.Append($" cannot appear in cell ({cell.Row + 1}, {cell.Column + 1}).");
                Console.WriteLine(message.ToString());
            }
        }

        return stepChangeMade;
    }

    private bool LookIfTheBoardHasMultipleSolutions()
    {
        bool changeMade = false;

        // This is the last chance to do something in this iteration:
        // If this attempt fails, board will not be entirely solved.

        // Try to see if there are pairs of values that can be exchanged arbitrarily
        // This happens when board has more than one valid solution

        Queue<(int index1, int index2, int digit1, int digit2)> candidates = new();

        for (int i = 0; i < candidateMasks.Length - 1; i++)
        {
            if (BitMasks.maskToOnesCount[candidateMasks[i]] == 2)
            {
                int row = i / 9;
                int col = i % 9;
                int blockIndex = 3 * (row / 3) + col / 3;

                int temp = candidateMasks[i];
                int lower = 0;
                int upper = 0;
                for (int digit = 1; temp > 0; digit++)
                {
                    if ((temp & 1) != 0)
                    {
                        lower = upper;
                        upper = digit;
                    }

                    temp = temp >> 1;
                }

                for (int j = i + 1; j < candidateMasks.Length; j++)
                {
                    if (candidateMasks[j] == candidateMasks[i])
                    {
                        int row1 = j / 9;
                        int col1 = j % 9;
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

            int[] alternateState = state.ShallowCopy();

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
            int row1 = index1 / 9;
            int col1 = index1 % 9;
            int row2 = index2 / 9;
            int col2 = index2 % 9;

            string description = string.Empty;

            if (row1 == row2)
            {
                description = $"row #{row1 + 1}";
            }
            else if (col1 == col2)
            {
                description = $"column #{col1 + 1}";
            }
            else
            {
                description = $"block ({row1 / 3 + 1}, {col1 / 3 + 1})";
            }

            SetCell(row1, col1, finalState[index1]);
            SetCell(row2, col2, finalState[index2]);
            changeMade = true;

            for (int i = 0; i < state.Length; i++)
            {
                int tempRow = i / 9;
                int tempCol = i % 9;

                board.Set(tempRow, tempCol, state[i]);
            }

            Console.WriteLine(
                $"Guessing that {digit1} and {digit2} are arbitrary in {description} (multiple solutions): Pick {finalState[index1]}->({row1 + 1}, {col1 + 1}), {finalState[index2]}->({row2 + 1}, {col2 + 1}).");
        }

        return changeMade;
    }

    private void SetCell(int row, int column, int digit)
    {
        state.Set(row, column, digit);
        board.Set(row, column, digit);
        candidateMasks[9 * row + column] = 0;
        candidateMasksNew.Get(row, column).Clear();
    }

    private void ExcludeCandidate(int row, int column, int digit)
    {
        candidateMasks[9 * row + column] &= ~(1 << (digit - 1));
        candidateMasksNew.Get(row, column).Exclude(digit);
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
