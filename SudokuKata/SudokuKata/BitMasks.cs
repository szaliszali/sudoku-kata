namespace SudokuKata;

internal static class BitMasks
{
    public static readonly Dictionary<int, int> singleBitToIndex;

    static BitMasks()
    {
        singleBitToIndex = new Dictionary<int, int>();
        for (int i = 0; i < 8 * sizeof(int); i++)
            singleBitToIndex[1 << i] = i;
    }
}
