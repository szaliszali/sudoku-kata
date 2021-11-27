namespace SudokuKata;

public static class CellDisplay
{
    public static char ToDisplay(int digit) => digit switch
    {
        0 => '.',
        var d when 1 <= d && d <= 9 => (char)('0' + d),
        _ => throw new ArgumentException()
    };
}
