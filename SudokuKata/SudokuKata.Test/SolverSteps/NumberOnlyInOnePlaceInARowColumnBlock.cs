using NUnit.Framework;
using System;

namespace SudokuKata.Test.SolverSteps;

internal class NumberOnlyInOnePlaceInARowColumnBlock
{
    [Test]
    public void NotEnoughConstraints_NoStepPossible()
    {
        var board = new Board(@"
            +---+---+---+
            |...|...|...|
            |...|2..|...|
            |...|...|...|
            +---+---+---+
            |.1.|...|...|
            |...|...|...|
            |...|...|.1.|
            +---+---+---+
            |...|..1|...|
            |...|...|...|
            |...|...|...|
            +---+---+---+");
        var solverState = new SolverState(board, new Random(1));
        solverState.RefreshCandidates();

        var result = SudokuKata.SolverSteps.TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock.Solve(solverState);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void MultipleStepsPossible_RandomPick()
    {
        var board = new Board(@"
            +---+---+---+
            |...|...|...|
            |...|1..|...|
            |...|...|...|
            +---+---+---+
            |.1.|...|...|
            |...|...|...|
            |...|...|.1.|
            +---+---+---+
            |...|..1|...|
            |...|...|...|
            |...|...|...|
            +---+---+---+");

        var solverStateWithSeed1 = new SolverState(board, new Random(1));
        solverStateWithSeed1.RefreshCandidates();
        var resultWithSeed1 = SudokuKata.SolverSteps.TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock.Solve(solverStateWithSeed1);

        var solverStateWithSeed2 = new SolverState(board, new Random(2));
        solverStateWithSeed2.RefreshCandidates();
        var resultWithSeed2 = SudokuKata.SolverSteps.TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock.Solve(solverStateWithSeed2);

        Assert.Multiple(() =>
        {
            Assert.That(resultWithSeed1, Is.EqualTo(new ISolverCommand[] {
                new SetCellCommand(new CellLocation(4,4), 1),
                new PrintMessageCommand("Row #5 can contain 1 only at (5, 5)."),
            }), "Seed 1");
            Assert.That(resultWithSeed2, Is.EqualTo(new ISolverCommand[] {
                new SetCellCommand(new CellLocation(4,4), 1),
                new PrintMessageCommand("Block (2, 2) can contain 1 only at (5, 5)."),
            }), "Seed 2");
        });
    }
}
