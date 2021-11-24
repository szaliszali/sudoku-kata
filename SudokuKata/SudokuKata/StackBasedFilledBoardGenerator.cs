﻿namespace SudokuKata;

internal class StackBasedFilledBoardGenerator
{
    private readonly Random rng;
    private readonly int[] initialState;
    private readonly Stack<(int[] state, int rowIndex, int colIndex, bool[] usedDigits)> combinedStack;
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
        int[] currentState = initialState.ShallowCopy();

        if (combinedStack.Any())
        {
            currentState = combinedStack.Peek().state.ShallowCopy();
        }
        Board currentBoard = new Board(currentState);

        int bestRow = -1;
        int bestCol = -1;
        bool[] bestUsedDigits = null;
        int bestCandidatesCount = -1;
        int bestRandomValue = -1;
        bool containsUnsolvableCells = false;

        foreach (CellLocation cell in currentBoard.AllLocations())
            if (currentState.Get(cell.Row, cell.Column) == 0)
            {
                int row = cell.Row;
                int col = cell.Column;
                int blockRow = row / 3;
                int blockCol = col / 3;

                bool[] isDigitUsed = new bool[9];

                for (int i = 0; i < 9; i++)
                {
                    int rowDigit = currentState.Get(i, col);
                    if (rowDigit > 0)
                        isDigitUsed[rowDigit - 1] = true;

                    int colDigit = currentState.Get(row, i);
                    if (colDigit > 0)
                        isDigitUsed[colDigit - 1] = true;

                    int blockDigit = currentState.Get(blockRow * 3 + i / 3, blockCol * 3 + i % 3);
                    if (blockDigit > 0)
                        isDigitUsed[blockDigit - 1] = true;
                } // for (i = 0..8)

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
                    bestRow = row;
                    bestCol = col;
                    bestUsedDigits = isDigitUsed;
                    bestCandidatesCount = candidatesCount;
                    bestRandomValue = randomValue;
                }
            } // for (index = 0..81)

        if (!containsUnsolvableCells)
        {
            combinedStack.Push((currentState, bestRow, bestCol, bestUsedDigits));
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
        (int[] currentState, int rowToMove, int colToMove, bool[] usedDigits) = combinedStack.Peek();
        int digitToMove = lastDigitStack.Pop();

        int movedToDigit = digitToMove + 1;
        while (movedToDigit <= 9 && usedDigits[movedToDigit - 1])
            movedToDigit += 1;

        if (digitToMove > 0)
        {
            usedDigits[digitToMove - 1] = false;
            currentState.Set(rowToMove, colToMove, 0);
        }

        if (movedToDigit <= 9)
        {
            lastDigitStack.Push(movedToDigit);
            usedDigits[movedToDigit - 1] = true;
            currentState.Set(rowToMove, colToMove, movedToDigit);

            if (currentState.Any(digit => digit == 0))
                command = "expand";
            else
            {
                command = "complete";
                solvedBoardState = currentState.ShallowCopy();
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
