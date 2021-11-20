namespace SudokuKata;

public class Lol2
{
    public CandidateSet Mask { get; }
    public int Discriminator { get; }
    public string Description { get; }
    public IGrouping<int, NamedCellGroup> Cells { get; }

    public Lol2(CandidateSet mask, int discriminator, string description, IGrouping<int, NamedCellGroup> cells)
    {
        Mask = mask;
        Discriminator = discriminator;
        Description = description;
        Cells = cells;
    }
}
