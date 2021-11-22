namespace SudokuKata.SolverSteps;

public record NumberCanOnlyAppearInOnePlaceDetection(string groupDescription, CellLocation location, int candidate);
