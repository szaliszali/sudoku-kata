namespace SudokuKata;

internal static class BitMasks
{
    public static readonly Dictionary<int, int> singleBitToIndex;
    public static readonly Dictionary<int, int> maskToOnesCount;
    public static readonly int allOnes = (1 << 9) - 1; // bit mask with all bits set

    static BitMasks()
    {
        maskToOnesCount = new Dictionary<int, int>();
        maskToOnesCount[0] = 0;
        for (int i = 1; i < (1 << 9); i++)
        {
            int smaller = i >> 1;
            int increment = i & 1;
            maskToOnesCount[i] = maskToOnesCount[smaller] + increment;
        }

        singleBitToIndex = new Dictionary<int, int>();
        for (int i = 0; i < 9; i++)
            singleBitToIndex[1 << i] = i;
    }
}
