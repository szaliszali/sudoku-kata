using System.Numerics;

namespace SudokuKata;

public class CandidateSet
{
    private readonly int size;
    private int candidateMask;

    public int NumCandidates => BitOperations.PopCount((uint)candidateMask);

    public bool Contains(int digit) => (candidateMask & (1 << digit - 1)) != 0;

    public void Include(int value) => candidateMask |= 1 << value - 1;
    public void Exclude(int value) => candidateMask &= ~(1 << value - 1);

    public void IncludeAll() => candidateMask = (1 << size) - 1;
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

    public IReadOnlyList<int> AllCandidates
    {
        get
        {
            var ret = new List<int>(NumCandidates);
            int temp = candidateMask;
            int curValue = 1;
            while (temp > 0)
            {
                if ((temp & 1) > 0)
                {
                    ret.Add(curValue);
                }

                temp = temp >> 1;
                curValue += 1;
            }
            return ret.AsReadOnly();
        }
    }

    public bool HasAtLeastOneCommon(CandidateSet other) => (candidateMask & other.candidateMask) != 0;
    public bool HasAtLeastOneDifferent(CandidateSet other) => (candidateMask & ~other.candidateMask) != 0;

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return candidateMask.Equals(((CandidateSet)obj).candidateMask);
    }

    public static bool operator ==(CandidateSet lhs, CandidateSet rhs) => lhs.Equals(rhs);
    public static bool operator !=(CandidateSet lhs, CandidateSet rhs) => !lhs.Equals(rhs);

    public static readonly IReadOnlyList<CandidateSet> AllPossibleCandidateSets;

    private CandidateSet(int size, int mask)
    {
        this.size = size;
        candidateMask = mask;
    }

    public CandidateSet(int size) : this(size, 0) { }
    static CandidateSet()
    {
        AllPossibleCandidateSets = Enumerable.Range(0, 1 << 9).Select(m => new CandidateSet(9, m)).ToList();
    }
}
