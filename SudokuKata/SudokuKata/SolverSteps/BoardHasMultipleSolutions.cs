namespace SudokuKata.SolverSteps;

public class BoardHasMultipleSolutions : ISolverStep<BoardHasMultipleSolutionsDetection>
{
    private readonly SolverState solverState;
    private readonly Board finalState;

    public BoardHasMultipleSolutions(SolverState solverState, Board finalState)
    {
        this.solverState = solverState;
        this.finalState = finalState;
    }

    IEnumerable<ISolverCommand> ISolverStep<BoardHasMultipleSolutionsDetection>.Act(BoardHasMultipleSolutionsDetection detection)
    {
        (CellLocation cell1, CellLocation cell2, int digit1, int digit2) = detection;
        int row1 = cell1.Row;
        int col1 = cell1.Column;
        int row2 = cell2.Row;
        int col2 = cell2.Column;

        string description =
            row1 == row2 ? $"row #{row1 + 1}"
            : col1 == col2 ? $"column #{col1 + 1}"
            : $"block ({row1 / 3 + 1}, {col1 / 3 + 1})";

        yield return new SetCellCommand(new CellLocation(row1, col1), finalState.Get(row1, col1));
        yield return new SetCellCommand(new CellLocation(row2, col2), finalState.Get(row2, col2));

        yield return new PrintMessageCommand(
            $"Guessing that {digit1} and {digit2} are arbitrary in {description} (multiple solutions): Pick {finalState.Get(row1, col1)}->({row1 + 1}, {col1 + 1}), {finalState.Get(row2, col2)}->({row2 + 1}, {col2 + 1}).");
    }

    IReadOnlyList<BoardHasMultipleSolutionsDetection> ISolverStep<BoardHasMultipleSolutionsDetection>.Detect()
    {
        Queue<(CellLocation index1, CellLocation index2, int digit1, int digit2)> candidates = new();

        foreach (CellLocation cell in solverState.Board.AllLocations())
        {
            CandidateSet candidateSet = solverState.Candidates.Get(cell);
            if (candidateSet.NumCandidates == 2)
            {
                (int lower, int upper) = candidateSet.CandidatePair;

                foreach (CellLocation cell1 in solverState.Board.AllLocations())
                {
                    if (cell.Row * 9 + cell.Column >= cell1.Row * 9 + cell1.Column) continue;

                    if (candidateSet == solverState.Candidates.Get(cell1.Row, cell1.Column))
                    {
                        if (cell.Row == cell1.Row || cell.Column == cell1.Column || cell.BlockIndex() == cell1.BlockIndex())
                        {
                            candidates.Enqueue((cell, cell1, lower, upper));
                        }
                    }
                }
            }
        }

        // At this point we have the lists with pairs of cells that might pick one of two digits each
        // Now we have to check whether that is really true - does the board have two solutions?

        List<BoardHasMultipleSolutionsDetection> solutions = new();

        while (candidates.Any())
        {
            (CellLocation cell1, CellLocation cell2, int digit1, int digit2) = candidates.Dequeue();

            Board alternateBoard = solverState.Board.Clone();
            alternateBoard.Set(cell1, digit1);
            alternateBoard.Set(cell2, digit2);
            if (finalState.Get(cell1) == digit1) alternateBoard.Swap(cell1, cell2);

            if (new StackBasedFilledBoardGenerator(solverState.Rng, alternateBoard).HasSolution)
            {
                // Board was solved successfully even with two digits swapped
                solutions.Add(new(cell1, cell2, digit1, digit2));
            }
        }

        return solutions;
    }

    IEnumerable<BoardHasMultipleSolutionsDetection> ISolverStep<BoardHasMultipleSolutionsDetection>.Pick(IReadOnlyList<BoardHasMultipleSolutionsDetection> detections) =>
        detections.PickOneRandomly(solverState.Rng);
}