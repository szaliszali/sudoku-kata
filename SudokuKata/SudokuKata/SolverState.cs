namespace SudokuKata;

internal class SolverState
{
    private Board board;
    public CandidatesForEachCell Candidates { get; private set; }

    public SolverState(Board board)
    {
        this.board = board;
    }

    public void SetCell(CellLocation location, int digit)
    {
        board.Set(location, digit);
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
        Candidates = new CandidatesForEachCell(this.board);
        ChangeMade = false;
    }

    public bool ChangeMade { get; private set; }
    public bool StepChangeMade { get; private set; }

    internal void StartInnerLoop()
    {
        StepChangeMade = false;
    }
}
