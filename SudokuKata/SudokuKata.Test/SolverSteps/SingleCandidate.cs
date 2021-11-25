using SudokuKata.SolverSteps;

namespace SudokuKata.Test.SolverSteps;

internal class SingleCandidate
{
    [Test]
    public void MultipleCandidates()
    {
        var board = new Board(@"
            12.456.89
            .........
            .........
            .........
            .........
            .........
            .........
            .........
            .........");

        var solverState = new SolverState(board, new Random());
        solverState.RefreshCandidates();

        ISolverStep sut = new CellsWithOnlyOneCandidateLeft(solverState);
        var result = sut.Execute();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void SingleCaseOfSingleCandidate()
    {
        var board = new Board(@"
            123456.89
            .........
            .........
            .........
            .........
            .........
            .........
            .........
            .........");

        var solverState = new SolverState(board, new Random());
        solverState.RefreshCandidates();

        ISolverStep sut = new CellsWithOnlyOneCandidateLeft(solverState);
        var result = sut.Execute();

        Assert.That(result, Is.EqualTo(new ISolverCommand[] {
            new SetCellCommand(new CellLocation(board, 0, 6),7),
            new PrintMessageCommand("(1, 7) can only contain 7."),
        }));
    }

    [Test]
    public void MultipleCasesOfSingleCandidate_RandomPick()
    {
        var board = new Board(@"
            123456.89
            .........
            .........
            .........
            .........
            .........
            34.789126
            .........
            .........");

        var solverStateWithSeed1 = new SolverState(board, new Random(1));
        solverStateWithSeed1.RefreshCandidates();
        ISolverStep sutWithSeed1 = new CellsWithOnlyOneCandidateLeft(solverStateWithSeed1);
        var resultWithSeed1 = sutWithSeed1.Execute();

        var solverStateWithSeed2 = new SolverState(board, new Random(2));
        solverStateWithSeed2.RefreshCandidates();
        ISolverStep sutWithSeed2 = new CellsWithOnlyOneCandidateLeft(solverStateWithSeed2);
        var resultWithSeed2 = sutWithSeed2.Execute();

        Assert.Multiple(() =>
        {
            Assert.That(resultWithSeed1, Is.EqualTo(new ISolverCommand[] {
                new SetCellCommand(new CellLocation(board, 0, 6),7),
                new PrintMessageCommand("(1, 7) can only contain 7."),
            }), "Seed 1");
            Assert.That(resultWithSeed2, Is.EqualTo(new ISolverCommand[] {
                new SetCellCommand(new CellLocation(board, 6, 2),5),
                new PrintMessageCommand("(7, 3) can only contain 5."),
            }), "Seed 2");
        });
    }
}
