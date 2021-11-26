namespace SudokuKata;

public class Board
{
    private const int DefaultBlockSize = 3;
    private const int DefaultSize = DefaultBlockSize * DefaultBlockSize;

    public int BlockSize => DefaultBlockSize;
    public int Size => DefaultSize;

    private readonly List<char[]> fancyBoard = new();

    public Board() : this(new int[DefaultSize * DefaultSize])
    { }

    public Board(string board) : this(board
        .Where(c => c == '.' || char.IsDigit(c))
        .Select(c => c switch { '.' => 0, _ => (int)(c - '0') }).ToArray())
    { }

    public Board(int[] initialState)
    {
        // Prepare empty board
        char separator = '+';
        char cellChar = '-';
        string line = Line(separator, cellChar);
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

        state = new int[Size * Size];

        for (var row = 0; row < Size; ++row)
            for (var column = 0; column < Size; ++column)
                Set(row, column, initialState[row * Size + column]);
    }

    private static string Line(char separator, char cellChar) =>
        $"{separator}{string.Join(separator, Enumerable.Repeat(new string(cellChar, 3), 3))}{separator}";

    private readonly int[] state;
    public virtual int[] State => state.ShallowCopy();

    public void Set(CellLocation location, int value) => Set(location.Row, location.Column, value);
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
                yield return new CellLocation(this, row, column);
    }

    public Board Clone() => new Board(state);

    public bool IsSolved() => state.All(c => c > 0);
}
