using System;
using System.IO;
using NUnit.Framework;

namespace SudokuKata.Test.SolverCommands;

internal class PrintMessage
{
    [Test]
    public void ExecutingPrintMessagePrintsALineToTheConsole()
    {
        var board = new Board();
        var solverState = new SolverState(board, new Random());

        var output = new StringWriter();
        Console.SetOut(output);

        ISolverCommand sut = new PrintMessageCommand("Test...");
        sut.Execute(solverState);

        Assert.That(output.ToString(), Is.EqualTo("Test..." + Environment.NewLine));
    }
}
