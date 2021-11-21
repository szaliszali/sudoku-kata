namespace SudokuKata;

internal record SetCellCommand(CellLocation Location, int Digit);
