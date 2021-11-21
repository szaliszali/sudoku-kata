namespace SudokuKata;

public class NamedCell
{
    public int Discriminator { get; }
    public string Description { get; }
    public int Row { get; }
    public int Column { get; }

    public NamedCell(int discriminator, string description, int row, int column)
    {
        Discriminator = discriminator;
        Description = description;
        Row = row;
        Column = column;
    }
}
