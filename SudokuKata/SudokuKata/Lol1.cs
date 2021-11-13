namespace SudokuKata;

public class Lol1
{
    public int Discriminator { get; }
    public string Description { get; }
    public int Index { get; }
    public int Row { get; }
    public int Column { get; }

    public Lol1(int discriminator, string description, int index, int row, int column)
    {
        Discriminator = discriminator;
        Description = description;
        Index = index;
        Row = row;
        Column = column;
    }
}
