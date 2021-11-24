namespace SudokuKata.SolverSteps;

public record BoardHasMultipleSolutionsDetection(CellLocation cell1, CellLocation cell2, int digit1, int digit2);
