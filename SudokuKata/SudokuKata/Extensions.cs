namespace SudokuKata;

internal static class Extensions
{
    public static T[] ShallowCopy<T>(this T[] source)
    {
        var result = new T[source.Length];
        Array.Copy(source, result, source.Length);
        return result;
    }
}
