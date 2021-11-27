namespace SudokuKata;

public class BoardDisplay
{
    public int BlockSize { get; }

    private readonly List<char[]> fancyBoard;

    public BoardDisplay(Board board)
    {
        BlockSize = board.BlockSize;

        // Prepare empty board
        fancyBoard = PrepareEmptyBoard();

        foreach (var cell in board.AllLocations()) Set(cell, board.Get(cell));
    }

    private List<char[]> PrepareEmptyBoard()
    {
        List<char[]> fancyBoard = new();
        string line = Line('+', '-');
        string middle = Line('|', '.');
        fancyBoard.Add(line.ToCharArray());
        for (var i = 0; i < BlockSize; ++i)
        {
            for (var j = 0; j < BlockSize; ++j)
                fancyBoard.Add(middle.ToCharArray());
            fancyBoard.Add(line.ToCharArray());
        }
        return fancyBoard;
    }

    private string Line(char separator, char cellChar) =>
        $"{separator}{string.Join(separator, Enumerable.Repeat(new string(cellChar, BlockSize), BlockSize))}{separator}";

    public void Set(CellLocation location, int value)
    {
        int rowToWrite = location.Row + location.Row / BlockSize + 1;
        int colToWrite = location.Column + location.Column / BlockSize + 1;

        fancyBoard[rowToWrite][colToWrite] = CellDisplay.ToDisplay(value);
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, fancyBoard.Select(s => new string(s)).ToArray());
    }
}
