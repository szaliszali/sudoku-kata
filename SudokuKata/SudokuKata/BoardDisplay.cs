namespace SudokuKata;

public class BoardDisplay
{
    public const int DefaultBlockSize = 3;

    public int BlockSize { get; }
    public int Size => BlockSize * BlockSize;

    private readonly List<char[]> fancyBoard = new();

    public BoardDisplay(Board board)
    {
        BlockSize = board.BlockSize;

        // Prepare empty board
        string line = Line('+', '-');
        string middle = Line('|', '.');
        fancyBoard.Add(line.ToCharArray());
        for (var i = 0; i < BlockSize; ++i)
        {
            for (var j = 0; j < BlockSize; ++j)
                fancyBoard.Add(middle.ToCharArray());
            fancyBoard.Add(line.ToCharArray());
        }

        state = new int[Size * Size];

        foreach (var cell in board.AllLocations()) Set(cell, board.Get(cell));
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

        fancyBoard[rowToWrite][colToWrite] = CellDisplay.ToDisplay(value);
        state[row * Size + col] = value;
    }

    public int Get(CellLocation location) => Get(location.Row, location.Column);
    public int Get(int row, int column) => state[row * Size + column];

    public string Code =>
        string.Concat(fancyBoard.Select(s => new string(s)))
            .Replace("-", string.Empty)
            .Replace("+", string.Empty)
            .Replace("|", string.Empty)
            .Replace(".", "0");

    public override string ToString()
    {
        return string.Join(Environment.NewLine, fancyBoard.Select(s => new string(s)).ToArray());
    }

    public IEnumerable<CandidateSet> AllPossibleCandidateSets() => Enumerable.Range(0, 1 << Size).Select(m => new CandidateSet(Size, m));
}
