namespace SudokuKata;

internal class StackBasedFilledBoardGenerator
{
    private readonly Random rng;
    private readonly int[] initialState;
    private readonly Stack<(Board board, CellLocation cell, bool[] usedDigits)> combinedStack;
    private readonly Stack<int> lastDigitStack;

    private string command;
    public bool HasSolution => command == "complete";

    private int[] solvedBoardState;
    public int[] SolvedBoardState => solvedBoardState.ShallowCopy();

    public StackBasedFilledBoardGenerator(Random rng, int[] initialState)
    {
        this.rng = rng;
        this.initialState = initialState;
        this.combinedStack = new();
        this.lastDigitStack = new();

        // Indicates operation to perform next
        // - expand - finds next empty cell and puts new state on stacks
        // - move - finds next candidate number at current pos and applies it to current state
        // - collapse - pops current state from stack as it did not yield a solution
        command = "expand";
        while (command != "complete" && command != "fail")
        {
            if (command == "expand")
            {
                Expand();
            }
            else if (command == "collapse")
            {
                Collapse();
            }
            else if (command == "move")
            {
                Move();
            }
        }
    }

    private void Expand()
    {
        Board currentBoard = combinedStack.Any() ? combinedStack.Peek().board.Clone() : new Board(initialState);

        CellLocation bestCell = new(-1, -1);
        bool[] bestUsedDigits = null;
        int bestCandidatesCount = -1;
        int bestRandomValue = -1;
        bool containsUnsolvableCells = false;

        foreach (CellLocation cell in currentBoard.AllLocations())
            if (currentBoard.Get(cell.Row, cell.Column) == 0)
            {
                bool[] isDigitUsed = new bool[currentBoard.Size];

                for (int i = 0; i < currentBoard.Size; i++)
                {
                    {
                        Func<CellLocation, (int row, int column)> f = cell => (i, cell.Column);
                        (int row, int column) = f(cell);
                        int rowDigit = currentBoard.Get(row, column);
                        if (rowDigit > 0)
                            isDigitUsed[rowDigit - 1] = true;
                    }

                    {
                        Func<CellLocation, (int row, int column)> f = cell => (cell.Row, i);
                        (int row, int column) = f(cell);
                        int colDigit = currentBoard.Get(row, column);
                        if (colDigit > 0)
                            isDigitUsed[colDigit - 1] = true;
                    }

                    {
                        int blockRow = cell.Row / 3;
                        int blockCol = cell.Column / 3;

                        Func<CellLocation, (int row, int column)> f = cell => (blockRow * 3 + i / 3, blockCol * 3 + i % 3);
                        (int row, int column) = f(cell);
                        int blockDigit = currentBoard.Get(row, column);
                        if (blockDigit > 0)
                            isDigitUsed[blockDigit - 1] = true;
                    }
                }

                int candidatesCount = isDigitUsed.Where(used => !used).Count();

                if (candidatesCount == 0)
                {
                    containsUnsolvableCells = true;
                    break;
                }

                int randomValue = rng.Next();

                if (bestCandidatesCount < 0 ||
                    candidatesCount < bestCandidatesCount ||
                    (candidatesCount == bestCandidatesCount && randomValue < bestRandomValue))
                {
                    bestCell = cell;
                    bestUsedDigits = isDigitUsed;
                    bestCandidatesCount = candidatesCount;
                    bestRandomValue = randomValue;
                }
            }

        if (!containsUnsolvableCells)
        {
            combinedStack.Push((currentBoard, bestCell, bestUsedDigits));
            lastDigitStack.Push(0); // No digit was tried at this position
        }

        // Always try to move after expand
        command = "move";
    }

    private void Collapse()
    {
        combinedStack.Pop();
        lastDigitStack.Pop();

        if (combinedStack.Any())
            command = "move"; // Always try to move after collapse
        else
            command = "fail";
    }

    private void Move()
    {
        (Board currentBoard, CellLocation move, bool[] usedDigits) = combinedStack.Peek();
        int digitToMove = lastDigitStack.Pop();

        int movedToDigit = digitToMove + 1;
        while (movedToDigit <= 9 && usedDigits[movedToDigit - 1])
            movedToDigit += 1;

        if (digitToMove > 0)
        {
            usedDigits[digitToMove - 1] = false;
            currentBoard.Set(move.Row, move.Column, 0);
        }

        if (movedToDigit <= 9)
        {
            lastDigitStack.Push(movedToDigit);
            usedDigits[movedToDigit - 1] = true;
            currentBoard.Set(move.Row, move.Column, movedToDigit);

            if (!currentBoard.IsSolved())
                command = "expand";
            else
            {
                command = "complete";
                solvedBoardState = currentBoard.State;
            }
        }
        else
        {
            // No viable candidate was found at current position - pop it in the next iteration
            lastDigitStack.Push(0);
            command = "collapse";
        }
    }
}
