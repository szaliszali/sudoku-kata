namespace SudokuKata;

internal class Solver
{
    private Random rng;
    private Board board;
    private int[] finalState;

    public Solver(Random rng, Board board, int[] finalState)
    {
        this.rng = rng;
        this.board = board;
        this.finalState = finalState;
    }

    public void SolveBoard()
    {
        SolverState solverState = new(board, rng);

        do
        {
            solverState.RefreshCandidates();

            do
            {
                solverState.StartInnerLoop();

                Func<SolverState, IEnumerable<ISolverCommand>>[] steps = new Func<SolverState, IEnumerable<ISolverCommand>>[] {
                    storeState => SolverSteps.PickCellsWithOnlyOneCandidateLeft.Solve(storeState),
                    storeState => SolverSteps.TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock.Solve(storeState),
                    storeState => SolverSteps.TryToFindPairsOfDigitsInTheSameRowColumnBlockAndRemoveThemFromOtherCollidingCells.Solve(storeState),
                    storeState => SolverSteps.TryToFindGroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsWithinRowColumnBlock.Solve(storeState),
                };

                foreach (var step in steps)
                {
                    if (solverState.ChangeMade || solverState.StepChangeMade) break;
                    Apply(step(solverState), solverState);
                }
            }
            while (solverState.StepChangeMade);

            if (!solverState.ChangeMade) Apply(SolverSteps.BoardHasMultipleSolutions.Solve(solverState, finalState), solverState);

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
