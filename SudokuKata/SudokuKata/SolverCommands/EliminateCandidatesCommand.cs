namespace SudokuKata;

public class EliminateCandidatesCommand : ISolverCommand
{
    CellLocation Location { get; }

    IReadOnlyList<int> Digits { get; }

    public EliminateCandidatesCommand(CellLocation location, IReadOnlyList<int> digits)
    {
        Location = location;
        Digits = digits;
    }

    void ISolverCommand.Execute(SolverState state)
    {
        state.EliminateCandidates(Location, Digits);
    }

    public override string ToString()
    {
        return $"EliminateCandidatesCommand {{ Location: {Location}, Digits: {string.Join(", ", Digits)} }}";
    }

    public override bool Equals(object obj)
    {
        return obj is EliminateCandidatesCommand &&
            ((EliminateCandidatesCommand)obj).Location == Location &&
            Enumerable.SequenceEqual(((EliminateCandidatesCommand)obj).Digits, Digits);
    }
}
