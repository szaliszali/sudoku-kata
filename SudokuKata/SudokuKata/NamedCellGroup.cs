namespace SudokuKata;

public class NamedCellGroup
{
    public int Discriminator { get; }
    public string Description { get; }
    public int Row { get; }
    public int Column { get; }

    public NamedCellGroup(int discriminator, string description, int index, int row, int column)
    {
        Discriminator = discriminator;
        Description = description;
        Row = row;
        Column = column;
    }
}
