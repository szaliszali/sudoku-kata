namespace SudokuKata;

public record struct CellLocation(Board board, int Row, int Column)
{
    public string ShortString() => $"({Row + 1}, {Column + 1})";

    public int BlockIndex() => 3 * (Row / 3) + Column / 3;

    public override string ToString() => $"CellLocation {{ Row = {Row}, Column = {Column} }}";
}
