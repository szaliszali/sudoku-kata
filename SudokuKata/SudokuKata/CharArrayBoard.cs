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

    public CharArrayBoard(int[] state) : this()
    {
        this.state = state;

        for (var row = 0; row < 9; ++row)
            for (var column = 0; column < 9; ++column)
                Set(row, column, state[row * 9 + column]);
    }

    private int[] state = new int[9 * 9]; // TODO hardcoded
    public virtual int[] State => state.ShallowCopy(); // TODO not used, only the override in RandomBoard

    public void Set(int row, int col, int value)
    {
        int rowToWrite = row + row / 3 + 1;
        int colToWrite = col + col / 3 + 1;

        this[rowToWrite][colToWrite] = value == 0 ? '.' : (char)('0' + value);
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
