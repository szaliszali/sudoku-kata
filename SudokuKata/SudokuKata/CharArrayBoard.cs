namespace SudokuKata;

public class CharArrayBoard : List<char[]>
{
    public CharArrayBoard()
    {
        // Prepare empty board
        string line = "+---+---+---+";
        string middle = "|...|...|...|";
        AddRange(new[]
        {
            line.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            line.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            line.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            line.ToCharArray()
        });
    }

    public string Code =>
        string.Join(string.Empty, this.Select(s => new string(s)).ToArray())
            .Replace("-", string.Empty)
            .Replace("+", string.Empty)
            .Replace("|", string.Empty)
            .Replace(".", "0");

    public override string ToString()
    {
        return string.Join(Environment.NewLine, this.Select(s => new string(s)).ToArray());
    }
}
