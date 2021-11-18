namespace SudokuKata;

public class CandidateSet
{
    private int candidateMask;

    public int NumCandidates => BitMasks.maskToOnesCount[candidateMask];

    public bool Contains(int digit) => (candidateMask & (1 << digit - 1)) != 0;

    public void Include(int value) => candidateMask |= 1 << value - 1;
    public void Exclude(int value) => candidateMask &= ~(1 << value - 1);

    public void IncludeAll() => candidateMask = BitMasks.allOnes;
    public void Clear() => candidateMask = 0;

    public int SingleCandidate => BitMasks.singleBitToIndex[candidateMask] + 1;

    public (int lower, int upper) CandidatePair
    {
        get
        {
            if (NumCandidates != 2) throw new InvalidOperationException();

            int temp = candidateMask;
            int lower = 0;
            int upper = 0;
            for (int digit = 1; temp > 0; digit++)
            {
                if ((temp & 1) != 0)
                {
                    lower = upper;
                    upper = digit;
                }

                temp = temp >> 1;
            }
            return (lower, upper);
        }
    }

    public bool HasAtLeastOneCommon(CandidateSet other) => (candidateMask & other.candidateMask) != 0;
    public bool HasAtLeastOneDifferent(CandidateSet other) => (~candidateMask & other.candidateMask) != 0;
}
