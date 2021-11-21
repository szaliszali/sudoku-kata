namespace SudokuKata;

internal class SolverState
{
    public Board Board { get; }
    public Random Rng { get; }
    public List<IGrouping<int, NamedCell>> CellGroups { get; }
    public CandidatesForEachCell Candidates { get; private set; }

    public SolverState(Board board, Random rng)
    {
        Board = board;
        Rng = rng;
        CellGroups = BuildACollectionNamedCellGroupsWhichMapsCellIndicesIntoDistinctGroupsRowsColumnsBlocks();
    }

    public void SetCell(CellLocation location, int digit)
    {
        Board.Set(location, digit);
        Candidates.Get(location).Clear();
        ChangeMade = true;
    }

    public void EliminateCandidates(CellLocation location, IReadOnlyCollection<int> digits)
    {
        foreach (int digit in digits)
        {
            Candidates.Get(location).Exclude(digit);
            StepChangeMade = true;
        }
    }

    internal void RefreshCandidates()
    {
        Candidates = new CandidatesForEachCell(Board);
        ChangeMade = false;
    }

    public bool ChangeMade { get; private set; }
    public bool StepChangeMade { get; private set; }

    internal void StartInnerLoop()
    {
        StepChangeMade = false;
    }

    private List<IGrouping<int, NamedCell>> BuildACollectionNamedCellGroupsWhichMapsCellIndicesIntoDistinctGroupsRowsColumnsBlocks() =>
        Board.AllLocations()
            .Select(location => new NamedCell(location.Row, $"row #{location.Row + 1}", location))
            .Concat(Board.AllLocations()
                .Select(location => new NamedCell(Board.Size + location.Column, $"column #{location.Column + 1}", location)))
            .Concat(Board.AllLocations()
                .Select(location => new NamedCell(2 * Board.Size + 3 * (location.Row / 3) + location.Column / 3, $"block ({location.Row / 3 + 1}, {location.Column / 3 + 1})", location)))
            .GroupBy(tuple => tuple.Discriminator)
            .ToList();

}
