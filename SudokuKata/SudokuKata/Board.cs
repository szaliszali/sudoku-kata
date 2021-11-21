namespace SudokuKata;

public class Board
{
    public int Size => 9;

    private readonly List<char[]> fancyBoard = new();

    public Board()
    {
        // Prepare empty board
        string line = "+---+---+---+";
        string middle = "|...|...|...|";
        fancyBoard.AddRange(new[]
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

    public Board(string board) : this(board
        .Where(c => c == '.' || char.IsDigit(c))
        .Select(c => c switch { '.' => 0, _ => (int)(c - '0') }).ToArray())
    { }

    public Board(int[] state) : this()
    {
        for (var row = 0; row < Size; ++row)
            for (var column = 0; column < Size; ++column)
                Set(row, column, state[row * Size + column]);
    }

    private int[] state = new int[9 * 9]; // TODO hardcoded
    public virtual int[] State => state.ShallowCopy();

    public void Set(int row, int col, int value)
    {
        int rowToWrite = row + row / 3 + 1;
        int colToWrite = col + col / 3 + 1;

        fancyBoard[rowToWrite][colToWrite] = value == 0 ? '.' : (char)('0' + value);
        state[row * Size + col] = value;
    }

    public int Get(CellLocation location) => Get(location.Row, location.Column);
    public int Get(int row, int column) => state[row * Size + column];

    public string Code =>
        string.Join(string.Empty, fancyBoard.Select(s => new string(s)).ToArray())
            .Replace("-", string.Empty)
            .Replace("+", string.Empty)
            .Replace("|", string.Empty)
            .Replace(".", "0");

    public override string ToString()
    {
        return string.Join(Environment.NewLine, fancyBoard.Select(s => new string(s)).ToArray());
    }

    public IEnumerable<CellLocation> AllLocations()
    {
        for (var row = 0; row < Size; ++row)
            for (var column = 0; column < Size; ++column)
                yield return new CellLocation(row, column);
    }
}
