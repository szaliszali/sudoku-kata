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

        string description =
            cell1.Row == cell2.Row ? $"row #{cell1.Row + 1}"
            : cell1.Column == cell2.Column ? $"column #{cell1.Column + 1}"
            : $"block ({cell1.Row / 3 + 1}, {cell1.Column / 3 + 1})";

        yield return new SetCellCommand(cell1, finalState.Get(cell1));
        yield return new SetCellCommand(cell2, finalState.Get(cell2));

        yield return new PrintMessageCommand(
            $"Guessing that {digit1} and {digit2} are arbitrary in {description} (multiple solutions): Pick {finalState.Get(cell1)}->{cell1.ShortString()}, {finalState.Get(cell2)}->{cell2.ShortString()}.");
    }

    IReadOnlyList<BoardHasMultipleSolutionsDetection> ISolverStep<BoardHasMultipleSolutionsDetection>.Detect()
    {
        Queue<BoardHasMultipleSolutionsDetection> candidates = new();

        foreach (CellLocation cell in solverState.Board.AllLocations())
        {
            CandidateSet candidateSet = solverState.Candidates.Get(cell);
            if (candidateSet.NumCandidates == 2)
            {
                foreach (CellLocation cell1 in solverState.Board.AllLocations())
                {
                    if (cell.Row * 9 + cell.Column >= cell1.Row * 9 + cell1.Column) continue;

                    if (candidateSet == solverState.Candidates.Get(cell1.Row, cell1.Column))
                    {
                        if (cell.Row == cell1.Row || cell.Column == cell1.Column || cell.BlockIndex() == cell1.BlockIndex())
                        {
                            (int lower, int upper) = candidateSet.CandidatePair;
                            candidates.Enqueue(new(cell, cell1, lower, upper));
                        }
                    }
                }
            }
        }

        // At this point we have the lists with pairs of cells that might pick one of two digits each
        // Now we have to check whether that is really true - does the board have two solutions?

        return candidates.Select(c =>
        {
            Board alternateBoard = solverState.Board.Clone();
            alternateBoard.Set(c.cell1, finalState.Get(c.cell2));
            alternateBoard.Set(c.cell2, finalState.Get(c.cell1));

            return (Candidate: c, new StackBasedFilledBoardGenerator(solverState.Rng, alternateBoard).HasSolution);
        })
            .Where(c => c.HasSolution)
            .Select(c => c.Candidate)
            .ToList();
    }

    IEnumerable<BoardHasMultipleSolutionsDetection> ISolverStep<BoardHasMultipleSolutionsDetection>.Pick(IReadOnlyList<BoardHasMultipleSolutionsDetection> detections) =>
        detections.PickOneRandomly(solverState.Rng);
}