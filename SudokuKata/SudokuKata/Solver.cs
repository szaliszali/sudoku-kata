﻿using System.Text;

namespace SudokuKata;

internal class Solver
{
    private Random rng;
    private CharArrayBoard board;
    private int[] finalState;

    private CandidatesForEachCell cellCandidates;
    private readonly List<IGrouping<int, NamedCell>> cellGroups;

    private int TotalCellCount => 9 * 9;

    public Solver(Random rng, CharArrayBoard board, int[] finalState)
    {
        this.rng = rng;
        this.board = board;
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

    private List<IGrouping<int, NamedCell>> BuildACollectionNamedCellGroupsWhichMapsCellIndicesIntoDistinctGroupsRowsColumnsBlocks()
    {
        var rowsIndices = Enumerable.Range(0, TotalCellCount)
            .Select(index => new NamedCell(index / 9, $"row #{index / 9 + 1}", index / 9, index % 9))
            .GroupBy(tuple => tuple.Discriminator);

        var columnIndices = Enumerable.Range(0, TotalCellCount)
            .Select(index => new NamedCell(9 + index % 9, $"column #{index % 9 + 1}", index / 9, index % 9))
            .GroupBy(tuple => tuple.Discriminator);

        var blockIndices = Enumerable.Range(0, TotalCellCount)
            .Select(index => new
            {
                Row = index / 9,
                Column = index % 9,
                Index = index
            })
            .Select(tuple => new NamedCell(18 + 3 * (tuple.Row / 3) + tuple.Column / 3, $"block ({tuple.Row / 3 + 1}, {tuple.Column / 3 + 1})", tuple.Row, tuple.Column))
            .GroupBy(tuple => tuple.Discriminator);

        return rowsIndices.Concat(columnIndices).Concat(blockIndices).ToList();
    }

    private bool PickCellsWithOnlyOneCandidateLeft()
    {
        bool changeMade = false;

        int[] singleCandidateIndices =
            cellCandidates
                .Select((mask, index) => new
                {
                    CandidatesCount = mask.NumCandidates,
                    Index = index
                })
                .Where(tuple => tuple.CandidatesCount == 1)
                .Select(tuple => tuple.Index)
                .ToArray();

        if (singleCandidateIndices.Length > 0)
        {
            int pickSingleCandidateIndex = rng.Next(singleCandidateIndices.Length);
            int singleCandidateIndex = singleCandidateIndices[pickSingleCandidateIndex];

            int row = singleCandidateIndex / 9;
            int col = singleCandidateIndex % 9;

            int candidate = cellCandidates.Get(row, col).SingleCandidate;

            SetCell(row, col, candidate);
            changeMade = true;

            Console.WriteLine("({0}, {1}) can only contain {2}.", row + 1, col + 1, candidate);
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
                    int blockRowIndex = (cellGroup / 3) * 3 + indexInGroup / 3;
                    int blockColIndex = (cellGroup % 3) * 3 + indexInGroup % 3;

                    if (cellCandidates.Get(cellGroup, indexInGroup).Contains(digit))
                    {
                        rowNumberCount += 1;
                        indexInRow = indexInGroup;
                    }

                    if (cellCandidates.Get(indexInGroup, cellGroup).Contains(digit))
                    {
                        colNumberCount += 1;
                        indexInCol = indexInGroup;
                    }

                    if (cellCandidates.Get(blockRowIndex, blockColIndex).Contains(digit))
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

        IEnumerable<CandidateSet> twoDigitMasks =
            cellCandidates.Where(mask => mask.NumCandidates == 2).Distinct().ToList();

        var groups =
            twoDigitMasks
                .SelectMany(mask =>
                    cellGroups
                        .Where(group => group.Count(tuple => cellCandidates.Get(tuple.Row, tuple.Column) == mask) == 2)
                        .Where(group => group.Any(tuple => cellCandidates.Get(tuple.Row, tuple.Column) != mask && cellCandidates.Get(tuple.Row, tuple.Column).HasAtLeastOneCommon(mask)))
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
                                cellCandidates.Get(cell.Row, cell.Column) != group.Mask &&
                                cellCandidates.Get(cell.Row, cell.Column).HasAtLeastOneCommon(group.Mask))
                        .ToList();

                var maskCells =
                    group.Cells
                        .Where(cell => cellCandidates.Get(cell.Row, cell.Column) == group.Mask)
                        .ToArray();


                if (cells.Any())
                {
                    CandidateSet temp = group.Mask;
                    (int lower, int upper) = temp.CandidatePair;

                    Console.WriteLine(
                        $"Values {lower} and {upper} in {group.Description} are in cells ({maskCells[0].Row + 1}, {maskCells[0].Column + 1}) and ({maskCells[1].Row + 1}, {maskCells[1].Column + 1}).");

                    foreach (var cell in cells)
                    {
                        List<int> valuesToRemove = cellCandidates.Get(cell.Row, cell.Column).AllCandidates.Intersect(group.Mask.AllCandidates).ToList();
                        ExcludeCandidates(cell.Row, cell.Column, valuesToRemove);

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

        IEnumerable<CandidateSet> masks =
            CandidateSet.AllPossibleCandidateSets
                .Where(cs => cs.NumCandidates > 1)
                .ToList();

        var groupsWithNMasks =
            masks
                .SelectMany(mask =>
                    cellGroups
                        .Where(group => @group.All(cell =>
                            board.Get(cell.Row, cell.Column) == 0 || (!mask.Contains(board.Get(cell.Row, cell.Column)))))
                        .Select(group => new
                        {
                            Mask = mask,
                            Description = @group.First().Description,
                            Cells = @group,
                            CellsWithMask =
                                @group.Where(cell => board.Get(cell.Row, cell.Column) == 0 && cellCandidates.Get(cell.Row, cell.Column).HasAtLeastOneCommon(mask))
                                    .ToList(),
                            CleanableCellsCount =
                                @group.Count(
                                    cell => board.Get(cell.Row, cell.Column) == 0 &&
                                            cellCandidates.Get(cell.Row, cell.Column).HasAtLeastOneCommon(mask) &&
                                            cellCandidates.Get(cell.Row, cell.Column).HasAtLeastOneDifferent(mask))
                        }))
                .Where(group => @group.CellsWithMask.Count() == @group.Mask.NumCandidates)
                .ToList();

        foreach (var groupWithNMasks in groupsWithNMasks)
        {
            CandidateSet mask = groupWithNMasks.Mask;

            if (groupWithNMasks.Cells
                .Any(cell =>
                    cellCandidates.Get(cell.Row, cell.Column).HasAtLeastOneCommon(mask) &&
                    cellCandidates.Get(cell.Row, cell.Column).HasAtLeastOneDifferent(mask)))
            {
                StringBuilder message = new StringBuilder();
                message.Append($"In {groupWithNMasks.Description} values ");
                message.AppendJoin($", ", mask.AllCandidates);
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
                if (!cellCandidates.Get(cell.Row, cell.Column).HasAtLeastOneDifferent(groupWithNMasks.Mask))
                    continue;

                stepChangeMade = true;

                var valuesToClear = cellCandidates.Get(cell.Row, cell.Column).AllCandidates.Except(groupWithNMasks.Mask.AllCandidates).ToArray();
                ExcludeCandidates(cell.Row, cell.Column, valuesToClear);

                StringBuilder message = new StringBuilder();
                message.AppendJoin(", ", valuesToClear);
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

        for (int i = 0; i < TotalCellCount - 1; i++)
        {
            int row = i / 9;
            int col = i % 9;

            CandidateSet candidateSet = cellCandidates.Get(row, col);
            if (candidateSet.NumCandidates == 2)
            {
                int blockIndex = 3 * (row / 3) + col / 3;
                (int lower, int upper) = candidateSet.CandidatePair;

                for (int j = i + 1; j < TotalCellCount; j++)
                {
                    int row1 = j / 9;
                    int col1 = j % 9;

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

            Console.WriteLine(
                $"Guessing that {digit1} and {digit2} are arbitrary in {description} (multiple solutions): Pick {finalState[index1]}->({row1 + 1}, {col1 + 1}), {finalState[index2]}->({row2 + 1}, {col2 + 1}).");
        }

        return changeMade;
    }

    private void SetCell(int row, int column, int digit)
    {
        board.Set(row, column, digit);
        cellCandidates.Get(row, column).Clear();
    }

    private void ExcludeCandidates(int row, int column, IEnumerable<int> digits)
    {
        foreach (int digit in digits)
        {
            cellCandidates.Get(row, column).Exclude(digit);

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
