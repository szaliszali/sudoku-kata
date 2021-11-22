namespace SudokuKata;

public record PrintMessageCommand(string Message) : ISolverCommand
{
    void ISolverCommand.Execute(SolverState state)
    {
        Console.WriteLine(Message);
    }
}
