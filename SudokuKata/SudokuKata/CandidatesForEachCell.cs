using System.Collections;

namespace SudokuKata;

public class CandidatesForEachCell : IEnumerable<CandidateSet>
{
    private readonly int boardSize;
    private readonly CandidateSet[] candidates;

    public CandidatesForEachCell(Board board)
    {
        boardSize = board.Size;
        candidates = board.AllLocations().Select(cell => Calculate(cell, board)).ToArray();
    }

    public CandidateSet Get(CellLocation location) => Get(location.Row, location.Column);
    public CandidateSet Get(int row, int column) => candidates[row * boardSize + column];

    private static CandidateSet Calculate(CellLocation cell, Board board)
    {
        var candidates = new CandidateSet(board.Size);
        if (board.Get(cell) == 0)
        {
            candidates.IncludeAll();
            int blockRow = cell.BlockRow();
            int blockCol = cell.BlockCol();

            for (int j = 0; j < board.Size; j++)
            {
                candidates.Exclude(board.Get(cell.Row, j));
                candidates.Exclude(board.Get(j, cell.Column));
                candidates.Exclude(board.Get(blockRow * board.BlockSize + j / board.BlockSize, blockCol * board.BlockSize + j % board.BlockSize));
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
