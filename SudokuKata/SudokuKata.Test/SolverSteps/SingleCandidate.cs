using System;

using NUnit.Framework;
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

        var result = PickCellsWithOnlyOneCandidateLeft.Solve(solverState);

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

        var result = PickCellsWithOnlyOneCandidateLeft.Solve(solverState);

        Assert.That(result, Is.EqualTo(new ISolverCommand[] {
            new SetCellCommand(new CellLocation(0,6),7),
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
        var resultWithSeed1 = PickCellsWithOnlyOneCandidateLeft.Solve(solverStateWithSeed1);

        var solverStateWithSeed2 = new SolverState(board, new Random(2));
        solverStateWithSeed2.RefreshCandidates();
        var resultWithSeed2 = PickCellsWithOnlyOneCandidateLeft.Solve(solverStateWithSeed2);

        Assert.Multiple(() =>
        {
            Assert.That(resultWithSeed1, Is.EqualTo(new ISolverCommand[] {
                new SetCellCommand(new CellLocation(0,6),7),
            }), "Seed 1");
            Assert.That(resultWithSeed2, Is.EqualTo(new ISolverCommand[] {
                new SetCellCommand(new CellLocation(6,2),5),
            }), "Seed 2");
        });
    }
}
