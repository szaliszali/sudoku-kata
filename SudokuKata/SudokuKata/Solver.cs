using SudokuKata.SolverSteps;

namespace SudokuKata;

internal class Solver
{
    private readonly Random rng;
    private readonly Board board;
    private readonly int[] finalState;

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

                ISolverStep[] steps = new ISolverStep[] {
                    new CellsWithOnlyOneCandidateLeft(solverState),
                    new NumberCanOnlyAppearInOnePlace(solverState),
                    new RemovePairsOfDigitsFromCollidingCells(solverState),
                    new GroupsOfDigitsOfSizeNWhichOnlyAppearInNCells(solverState),
                };

                foreach (var step in steps)
                {
                    if (solverState.ChangeMade || solverState.StepChangeMade) break;
                    Apply(step.Execute(), solverState);
                }
            }
            while (solverState.StepChangeMade);

            if (!solverState.ChangeMade)
            {
                ISolverStep step = new BoardHasMultipleSolutions(solverState, new Board(finalState));
                Apply(step.Execute(), solverState);
            }

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
