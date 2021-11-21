namespace SudokuKata;

public class Lol2
{
    public CandidateSet Mask { get; }
    public string Description { get; }
    public IGrouping<int, NamedCell> Cells { get; }

    public Lol2(CandidateSet mask, string description, IGrouping<int, NamedCell> cells)
    {
        Mask = mask;
        Description = description;
        Cells = cells;
    }
}
