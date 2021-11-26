﻿using System.Collections;

namespace SudokuKata;

public class CandidatesForEachCell : IEnumerable<CandidateSet>
{
    private readonly CandidateSet[] candidates;

    public CandidatesForEachCell(Board board)
    {
        candidates = board.AllLocations().Select(cell => Calculate(cell.Row, cell.Column, board)).ToArray();
    }

    public CandidateSet Get(CellLocation location) => Get(location.Row, location.Column);
    public CandidateSet Get(int row, int column) => candidates[row * 9 + column];

    private static CandidateSet Calculate(int row, int column, Board board)
    {
        var candidates = new CandidateSet(board.Size);
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
