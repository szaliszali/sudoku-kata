using System.Collections;

namespace SudokuKata;

public class CandidatesForEachCell : IEnumerable<CandidateSet>
{
    private readonly CandidateSet[] candidates;

    public CandidatesForEachCell(Board board)
    {
        candidates = board.State.Select((_, index) => Calculate(index / 9, index % 9, board.State)).ToArray();
    }

    public CandidateSet Get(int row, int column) => candidates[row * 9 + column];

    private static CandidateSet Calculate(int row, int column, int[] board)
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

    IEnumerator<CandidateSet> IEnumerable<CandidateSet>.GetEnumerator()
    {
        return ((IEnumerable<CandidateSet>)candidates).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return candidates.GetEnumerator();
    }
}
