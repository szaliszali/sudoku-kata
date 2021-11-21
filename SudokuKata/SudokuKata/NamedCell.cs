namespace SudokuKata;

public class NamedCell
{
    public int Discriminator { get; }
    public string Description { get; }
    public CellLocation Location { get; }

    public NamedCell(int discriminator, string description, CellLocation location)
    {
        Discriminator = discriminator;
        Description = description;
        Location = location;
    }
}
