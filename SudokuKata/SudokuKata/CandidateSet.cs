namespace SudokuKata;

public class CandidateSet
{
    private int candidateMask;

    public int NumCandidates => BitMasks.maskToOnesCount[candidateMask];

    public bool Contains(int digit) => (candidateMask & (1 << digit - 1)) != 0;

    public void Include(int value) => candidateMask |= 1 << value - 1;
    public void Exclude(int value) => candidateMask &= ~(1 << value - 1);

    public void IncludeAll() => candidateMask = BitMasks.allOnes;
}
