﻿namespace SudokuKata;

public static class Extensions
{
    public static T[] ShallowCopy<T>(this T[] source)
    {
        var result = new T[source.Length];
        Array.Copy(source, result, source.Length);
        return result;
    }

    public static void Set(this int[] state, int row, int column, int value) => state[row * 9 + column] = value;
}
