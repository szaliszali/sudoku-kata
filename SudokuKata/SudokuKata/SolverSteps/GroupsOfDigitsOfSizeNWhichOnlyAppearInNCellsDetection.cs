namespace SudokuKata.SolverSteps;

public record GroupsOfDigitsOfSizeNWhichOnlyAppearInNCellsDetection
(
    CandidateSet Candidates,
    string Description,
    IGrouping<int, NamedCell> Cells,
    List<NamedCell> CellsWithMask,
    int CleanableCellsCount
);
