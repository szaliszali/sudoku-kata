namespace SudokuKata.SolverSteps;

public class RemovePairsOfDigitsFromCollidingCellsDetection
{
    public CandidateSet Candidates { get; }
    public string Description { get; }
    public IGrouping<int, NamedCell> Cells { get; }

    public RemovePairsOfDigitsFromCollidingCellsDetection(CandidateSet mask, string description, IGrouping<int, NamedCell> cells)
    {
        Candidates = mask;
        Description = description;
        Cells = cells;
    }
}
