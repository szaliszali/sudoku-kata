﻿namespace SudokuKata;

public class CharArrayBoard
{
    private readonly List<char[]> fancyBoard = new();

    public CharArrayBoard()
    {
        // Prepare empty board
        string line = "+---+---+---+";
        string middle = "|...|...|...|";
        fancyBoard.AddRange(new[]
        {
            line.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            line.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            line.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            middle.ToCharArray(),
            line.ToCharArray()
        });
    }

    public CharArrayBoard(string board) : this(board
        .Where(c => c == '.' || char.IsDigit(c))
        .Select(c => c switch { '.' => 0, _ => (int)(c - '0') }).ToArray())
    { }

    public CharArrayBoard(int[] state) : this()
    {
        for (var row = 0; row < 9; ++row)
            for (var column = 0; column < 9; ++column)
                Set(row, column, state[row * 9 + column]);
    }

    private int[] state = new int[9 * 9]; // TODO hardcoded
    public virtual int[] State => state.ShallowCopy();

    public void Set(int row, int col, int value)
    {
        int rowToWrite = row + row / 3 + 1;
        int colToWrite = col + col / 3 + 1;

        fancyBoard[rowToWrite][colToWrite] = value == 0 ? '.' : (char)('0' + value);
        state[row * 9 + col] = value;
    }

    public int Get(int row, int column) => state[row * 9 + column];

    public string Code =>
        string.Join(string.Empty, fancyBoard.Select(s => new string(s)).ToArray())
            .Replace("-", string.Empty)
            .Replace("+", string.Empty)
            .Replace("|", string.Empty)
            .Replace(".", "0");

    public override string ToString()
    {
        return string.Join(Environment.NewLine, fancyBoard.Select(s => new string(s)).ToArray());
    }
}
