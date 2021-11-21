namespace SudokuKata;

internal class SolverState
{
    public Board Board { get; }
    public Random Rng { get; }
    public CandidatesForEachCell Candidates { get; private set; }

    public SolverState(Board board, Random rng)
    {
        Board = board;
        Rng = rng;
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
}
