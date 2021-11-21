namespace SudokuKata;

public record struct CellLocation(int Row, int Column)
{
    public string ShortString() => $"({Row + 1}, {Column + 1})";
}
