using System;
using NUnit.Framework;

namespace SudokuKata.Test.SolverCommands;

internal class EliminateCandidates
{
    [Test]
    public void EliminateCandidatesCommandEliminatesCandidates()
    {
        var board = new Board();
        var solverState = new SolverState(board, new Random());
        solverState.RefreshCandidates();

        ISolverCommand sut = new EliminateCandidatesCommand(new CellLocation(1, 2), new[] { 3, 4 });
        sut.Execute(solverState);

        Assert.That(solverState.Candidates.Get(1, 2).AllCandidates, Is.EqualTo(new[] { 1, 2, 5, 6, 7, 8, 9 }));
    }
}
