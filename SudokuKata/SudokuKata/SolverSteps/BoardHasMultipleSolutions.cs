namespace SudokuKata.SolverSteps;

public class BoardHasMultipleSolutions : ISolverStep<BoardHasMultipleSolutionsDetection>
{
    private readonly SolverState solverState;
    private readonly int[] finalState;

    public BoardHasMultipleSolutions(SolverState solverState, int[] finalState)
    {
        this.solverState = solverState;
        this.finalState = finalState;
    }

    IEnumerable<ISolverCommand> ISolverStep<BoardHasMultipleSolutionsDetection>.Act(IReadOnlyList<BoardHasMultipleSolutionsDetection> detections)
    {
        if (detections.Any())
        {
            (int index1, int index2, int digit1, int digit2) = detections.PickOneRandomly(solverState.Rng);
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

            yield return new PrintMessageCommand(
                $"Guessing that {digit1} and {digit2} are arbitrary in {description} (multiple solutions): Pick {finalState[index1]}->({row1 + 1}, {col1 + 1}), {finalState[index2]}->({row2 + 1}, {col2 + 1}).");
        }
    }

    IReadOnlyList<BoardHasMultipleSolutionsDetection> ISolverStep<BoardHasMultipleSolutionsDetection>.Detect()
    {
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

        List<BoardHasMultipleSolutionsDetection> solutions = new();

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
                solutions.Add(new(index1, index2, digit1, digit2));
            }
        }

        return solutions;
    }
}