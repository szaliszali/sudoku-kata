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

    public static string Capitalize(this string input) => char.ToUpper(input[0]) + input.Substring(1);

    public static IEnumerable<T> PickOneRandomly<T>(this IReadOnlyList<T> list, Random rng)
    {
        if (list.Count > 0)
            yield return list[rng.Next(list.Count)];
    }
}
