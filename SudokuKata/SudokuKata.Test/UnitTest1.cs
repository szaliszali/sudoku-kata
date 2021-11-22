namespace SudokuKata.Test;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var output = new StringWriter();
        Console.SetOut(output);

        for (var i = 0; i < 10; i++)
        {
            Program.Play(new Random(i));
        }

        Approvals.Verify(output);
    }
}