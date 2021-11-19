namespace SudokuKata;

public class CalculateCandidates
{
    private int[] board;

    public CalculateCandidates(int[] board)
    {
        this.board = board;
    }

    public CandidateSet Get(int row, int column)
    {
        var candidates = new CandidateSet();
        if (board.Get(row, column) == 0)
        {
            candidates.IncludeAll();
            int blockRow = row / 3;
            int blockCol = column / 3;

            for (int j = 0; j < 9; j++)
            {
                candidates.Exclude(board.Get(row, j));
                candidates.Exclude(board.Get(j, column));
                candidates.Exclude(board.Get(blockRow * 3 + j / 3, blockCol * 3 + j % 3));
            }
        }
        return candidates;
    }
}
