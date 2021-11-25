namespace SudokuKata;

public record struct CellLocation(Board board, int Row, int Column)
{
    public string ShortString() => $"({Row + 1}, {Column + 1})";

    public int BlockIndex() => board.BlockSize * (Row / board.BlockSize) + Column / board.BlockSize;
    public int BlockRow() => Row / board.BlockSize;

    public override string ToString() => $"CellLocation {{ Row = {Row}, Column = {Column} }}";
}
