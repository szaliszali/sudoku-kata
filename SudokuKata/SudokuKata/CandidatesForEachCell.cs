using System.Collections;

namespace SudokuKata;

public class CandidatesForEachCell : IEnumerable<CandidateSet>
{
    private readonly int boardSize;
    private readonly CandidateSet[] candidates;

    public CandidatesForEachCell(Board board)
    {
        boardSize = board.Size;
        candidates = board.AllLocations().Select(cell => Calculate(cell.Row, cell.Column, board)).ToArray();
    }

    public CandidateSet Get(CellLocation location) => Get(location.Row, location.Column);
    public CandidateSet Get(int row, int column) => candidates[row * boardSize + column];

    private static CandidateSet Calculate(int row, int column, Board board)
    {
        var candidates = new CandidateSet(board.Size);
        if (board.Get(row, column) == 0)
        {
            candidates.IncludeAll();
            int blockRow = row / board.BlockSize;
            int blockCol = column / board.BlockSize;

            for (int j = 0; j < board.Size; j++)
            {
                candidates.Exclude(board.Get(row, j));
                candidates.Exclude(board.Get(j, column));
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
