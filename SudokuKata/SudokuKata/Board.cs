namespace SudokuKata;

public class Board
{
    public const int DefaultBlockSize = 3;

    public int BlockSize { get; }
    public int Size => BlockSize * BlockSize;

    private readonly List<char[]> fancyBoard = new();

    public Board(int customBlockSize = DefaultBlockSize) : this(new int[customBlockSize * customBlockSize * customBlockSize * customBlockSize], customBlockSize)
    { }

    public Board(string board) : this(board
        .Where(c => c == '.' || char.IsDigit(c))
        .Select(c => c switch { '.' => 0, _ => (int)(c - '0') }).ToArray(), DefaultBlockSize)
    { }

    private Board(int[] initialState, int customBlockSize)
    {
        BlockSize = customBlockSize;

        // Prepare empty board
        string line = Line('+', '-');
        string middle = Line('|', '.');
        fancyBoard.Add(line.ToCharArray());
        for (var i = 0; i < BlockSize; ++i)
        {
            for (var j = 0; j < BlockSize; ++j)
                fancyBoard.Add(middle.ToCharArray());
            fancyBoard.Add(line.ToCharArray());
        };

        state = new int[Size * Size];

        for (var row = 0; row < Size; ++row)
            for (var column = 0; column < Size; ++column)
                Set(row, column, initialState[row * Size + column]);
    }

    private string Line(char separator, char cellChar) =>
        $"{separator}{string.Join(separator, Enumerable.Repeat(new string(cellChar, BlockSize), BlockSize))}{separator}";

    private readonly int[] state;
    public virtual int[] State => state.ShallowCopy();

    public void Set(CellLocation location, int value) => Set(location.Row, location.Column, value);
    public void Set(int row, int col, int value)
    {
        int rowToWrite = row + row / BlockSize + 1;
        int colToWrite = col + col / BlockSize + 1;

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

    public Board Clone() => new Board(state, DefaultBlockSize);

    public bool IsSolved() => state.All(c => c > 0);

    public IEnumerable<CandidateSet> AllPossibleCandidateSets() => Enumerable.Range(0, 1 << Size).Select(m => new CandidateSet(Size, m));
}
