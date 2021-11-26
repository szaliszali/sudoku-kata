namespace SudokuKata;

internal static class BitMasks
{
    public static readonly Dictionary<int, int> singleBitToIndex;
    public static readonly int allOnes = (1 << 9) - 1; // bit mask with all bits set

    static BitMasks()
    {
        singleBitToIndex = new Dictionary<int, int>();
        for (int i = 0; i < 8 * sizeof(int); i++)
            singleBitToIndex[1 << i] = i;
    }
}
