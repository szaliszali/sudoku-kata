namespace SudokuKata;

public class Board
{
    public const int DefaultBlockSize = 3;

    public int BlockSize { get; }
    public int Size => BlockSize * BlockSize;

    public Board(int customBlockSize = DefaultBlockSize) : this(new int[customBlockSize * customBlockSize * customBlockSize * customBlockSize], customBlockSize)
    { }

    public Board(string board) : this(board
        .Where(c => c == '.' || char.IsDigit(c))
        .Select(c => c switch { '.' => 0, _ => (int)(c - '0') }).ToArray(), DefaultBlockSize)
    { }

    private Board(int[] initialState, int customBlockSize)
    {
        BlockSize = customBlockSize;

        state = new int[Size * Size];

        for (var row = 0; row < Size; ++row)
            for (var column = 0; column < Size; ++column)
                Set(row, column, initialState[row * Size + column]);
    }

    private readonly int[] state;
    public virtual int[] State => state.ShallowCopy();

    public void Set(CellLocation location, int value) => Set(location.Row, location.Column, value);
    public void Set(int row, int col, int value)
    {
        state[row * Size + col] = value;
    }

    public int Get(CellLocation location) => Get(location.Row, location.Column);
    public int Get(int row, int column) => state[row * Size + column];

    public string Code =>
        string.Concat(State.Select(CellDisplay.ToDisplay)).Replace('.', '0');

    public override string ToString() => new BoardDisplay(this).ToString();

    public IEnumerable<CellLocation> AllLocations()
    {
        for (var row = 0; row < Size; ++row)
            for (var column = 0; column < Size; ++column)
                yield return new CellLocation(this, row, column);
    }

    public Board Clone() => new Board(state, BlockSize);

    public bool IsSolved() => state.All(c => c > 0);

    public IEnumerable<CandidateSet> AllPossibleCandidateSets() => Enumerable.Range(0, 1 << Size).Select(m => new CandidateSet(Size, m));
}
