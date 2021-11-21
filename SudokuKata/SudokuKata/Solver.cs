using System.Text;

namespace SudokuKata;

internal class Solver
{
    private Random rng;
    private Board board;
    private int[] finalState;

    private CandidatesForEachCell cellCandidates;

    private int TotalCellCount { get; }

    public Solver(Random rng, Board board, int[] finalState)
    {
        this.rng = rng;
        this.board = board;
        TotalCellCount = board.AllLocations().Count();
        this.finalState = finalState;
    }

    public void SolveBoard()
    {
        SolverState solverState = new(board, rng);

        do
        {
            solverState.RefreshCandidates();
            cellCandidates = solverState.Candidates;

            do
            {
                solverState.StartInnerLoop();

                Func<SolverState, IEnumerable<ISolverCommand>>[] steps = new[] {
                    PickCellsWithOnlyOneCandidateLeft,
                    TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock,
                    TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells,
                    TryToFindGroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsWithinRowColumnBlock,
                };

                foreach (var step in steps)
                {
                    if (solverState.ChangeMade || solverState.StepChangeMade) break;
                    Apply(step(solverState), solverState);
                }
            }
            while (solverState.StepChangeMade);

            if (!solverState.ChangeMade) Apply(LookIfTheBoardHasMultipleSolutions(solverState, finalState), solverState);

            PrintBoardIfChanged(solverState.ChangeMade);
        }
        while (solverState.ChangeMade);
    }

    private void Apply(IEnumerable<ISolverCommand> commands, SolverState solverState)
    {
        foreach (var command in commands)
        {
            command.Execute(solverState);
        }
    }

    private static IEnumerable<ISolverCommand> PickCellsWithOnlyOneCandidateLeft(SolverState solverState) =>
        SolverSteps.PickCellsWithOnlyOneCandidateLeft.Solve(solverState);

    private static IEnumerable<ISolverCommand> TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock(SolverState solverState) =>
        SolverSteps.TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock.Solve(solverState);

    private static IEnumerable<ISolverCommand> TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells(SolverState solverState) =>
        SolverSteps.TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells.Solve(solverState);

    private static IEnumerable<ISolverCommand> TryToFindGroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsWithinRowColumnBlock(SolverState solverState)
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
                    solverState.CellGroups
                        .Where(group => @group.All(cell =>
                            solverState.Board.Get(cell.Location) == 0 || (!mask.Contains(solverState.Board.Get(cell.Location)))))
                        .Select(group => new
                        {
                            Mask = mask,
                            Description = @group.First().Description,
                            Cells = @group,
                            CellsWithMask =
                                @group.Where(cell => solverState.Board.Get(cell.Location) == 0 && solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(mask))
                                    .ToList(),
                            CleanableCellsCount =
                                @group.Count(
                                    cell => solverState.Board.Get(cell.Location) == 0 &&
                                            solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(mask) &&
                                            solverState.Candidates.Get(cell.Location).HasAtLeastOneDifferent(mask))
                        }))
                .Where(group => @group.CellsWithMask.Count() == @group.Mask.NumCandidates)
                .ToList();

        foreach (var groupWithNMasks in groupsWithNMasks)
        {
            CandidateSet mask = groupWithNMasks.Mask;

            if (groupWithNMasks.Cells
                .Any(cell =>
                    solverState.Candidates.Get(cell.Location).HasAtLeastOneCommon(mask) &&
                    solverState.Candidates.Get(cell.Location).HasAtLeastOneDifferent(mask)))
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
                if (!solverState.Candidates.Get(cell.Location).HasAtLeastOneDifferent(groupWithNMasks.Mask))
                    continue;

                var valuesToClear = solverState.Candidates.Get(cell.Location).AllCandidates.Except(groupWithNMasks.Mask.AllCandidates).ToArray();
                yield return new EliminateCandidatesCommand(cell.Location, valuesToClear);

                StringBuilder message = new StringBuilder();
                message.AppendJoin(", ", valuesToClear);
                message.Append($" cannot appear in cell {cell.Location.ShortString()}.");
                Console.WriteLine(message.ToString());
            }
        }
    }

    private static IEnumerable<ISolverCommand> LookIfTheBoardHasMultipleSolutions(SolverState solverState, int[] finalState)
    {
        // This is the last chance to do something in this iteration:
        // If this attempt fails, board will not be entirely solved.

        // Try to see if there are pairs of values that can be exchanged arbitrarily
        // This happens when board has more than one valid solution

        Queue<(int index1, int index2, int digit1, int digit2)> candidates = new();

        for (int i = 0; i < finalState.Length - 1; i++)
        {
            int row = i / solverState.Board.Size;
            int col = i % solverState.Board.Size;

            CandidateSet candidateSet = solverState.Candidates.Get(row, col);
            if (candidateSet.NumCandidates == 2)
            {
                int blockIndex = 3 * (row / 3) + col / 3;
                (int lower, int upper) = candidateSet.CandidatePair;

                for (int j = i + 1; j < finalState.Length; j++)
                {
                    int row1 = j / solverState.Board.Size;
                    int col1 = j % solverState.Board.Size;

                    if (candidateSet == solverState.Candidates.Get(row1, col1))
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

            int[] alternateState = solverState.Board.State.ShallowCopy();

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

            if (new StackBasedFilledBoardGenerator(solverState.Rng, alternateState).HasSolution)
            {
                // Board was solved successfully even with two digits swapped
                solutions.Add((index1, index2, digit1, digit2));
            }
        }

        if (solutions.Any())
        {
            int pos = solverState.Rng.Next(solutions.Count());
            (int index1, int index2, int digit1, int digit2) = solutions.ElementAt(pos);
            int row1 = index1 / solverState.Board.Size;
            int col1 = index1 % solverState.Board.Size;
            int row2 = index2 / solverState.Board.Size;
            int col2 = index2 % solverState.Board.Size;

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
