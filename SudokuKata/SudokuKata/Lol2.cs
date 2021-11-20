namespace SudokuKata;

public class Lol2
{
    public CandidateSet Mask { get; }
    public int Discriminator { get; }
    public string Description { get; }
    public IGrouping<int, Lol1> Cells { get; }

    public Lol2(CandidateSet mask, int discriminator, string description, IGrouping<int, Lol1> cells)
    {
        Mask = mask;
        Discriminator = discriminator;
        Description = description;
        Cells = cells;
    }
}
