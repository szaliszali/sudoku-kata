namespace SudokuKata.SolverSteps;

public class RemovePairsOfDigitsFromCollidingCellsDetection
{
    public CandidateSet Mask { get; }
    public string Description { get; }
    public IGrouping<int, NamedCell> Cells { get; }

    public RemovePairsOfDigitsFromCollidingCellsDetection(CandidateSet mask, string description, IGrouping<int, NamedCell> cells)
    {
        Mask = mask;
        Description = description;
        Cells = cells;
    }
}
