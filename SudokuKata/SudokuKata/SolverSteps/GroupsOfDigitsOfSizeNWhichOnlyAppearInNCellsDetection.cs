namespace SudokuKata.SolverSteps;

public record GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection
(
    CandidateSet Mask,
    string Description,
    IGrouping<int, NamedCell> Cells,
    List<NamedCell> CellsWithMask,
    int CleanableCellsCount
);
