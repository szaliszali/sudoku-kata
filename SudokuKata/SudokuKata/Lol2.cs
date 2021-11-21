namespace SudokuKata;

public class Lol2
{
    public CandidateSet Mask { get; }
    public int Discriminator { get; }
    public string Description { get; }
    public IGrouping<int, NamedCell> Cells { get; }

    public Lol2(CandidateSet mask, int discriminator, string description, IGrouping<int, NamedCell> cells)
    {
        Mask = mask;
        Discriminator = discriminator;
        Description = description;
        Cells = cells;
    }
}
