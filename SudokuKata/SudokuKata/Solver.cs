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

    private static IEnumerable<ISolverCommand> TryToFindGroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsWithinRowColumnBlock(SolverState solverState) =>
        SolverSteps.TryToFindGroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsWithinRowColumnBlock.Solve(solverState);

    private static IEnumerable<ISolverCommand> LookIfTheBoardHasMultipleSolutions(SolverState solverState, int[] finalState) =>
        SolverSteps.LookIfTheBoardHasMultipleSolutions.Solve(solverState, finalState);

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
