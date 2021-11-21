namespace SudokuKata;

internal record EliminateCandidatesCommand (CellLocation Location, IReadOnlyList<int> Digits);
