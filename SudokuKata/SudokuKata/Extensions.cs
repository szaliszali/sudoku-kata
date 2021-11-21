namespace SudokuKata;

public static class Extensions
{
    public static T[] ShallowCopy<T>(this T[] source)
    {
        var result = new T[source.Length];
        Array.Copy(source, result, source.Length);
        return result;
    }

    public static int Get(this int[] state, int row, int column) => state[row * 9 + column];
    public static void Set(this int[] state, int row, int column, int value) => state[row * 9 + column] = value;

    public static string Capitalize(this string input) => char.ToUpper(input[0]) + input.Substring(1);
}
