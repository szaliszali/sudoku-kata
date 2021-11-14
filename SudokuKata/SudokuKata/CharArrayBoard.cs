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
}
