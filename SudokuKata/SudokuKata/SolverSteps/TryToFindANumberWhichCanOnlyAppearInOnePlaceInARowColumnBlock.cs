namespace SudokuKata.SolverSteps;

internal class TryToFindANumberWhichCanOnlyAppearInOnePlaceInARowColumnBlock
{
    public static IEnumerable<ISolverCommand> Solve(SolverState solverState)
    {
        List<(string groupDescription, CellLocation location, int candidate)> candidates =
            Enumerable.Range(1, solverState.Board.Size)
                .SelectMany(digit => solverState.CellGroups
                    .Select(g => (g, count: g.Count(c => solverState.Candidates.Get(c.Location).Contains(digit)), digit))
                    .Where(g => g.count == 1))
                .OrderBy(g => g.digit)
                .ThenBy(g => g.g.First().Discriminator % solverState.Board.Size) // HACK: original code enumerated cell groups in different order
                .Select(g => (description: g.g.First().Description.Capitalize(), location: g.g.Single(c => solverState.Candidates.Get(c.Location).Contains(g.digit)).Location, g.digit))
                .ToList();

        if (candidates.Count > 0)
        {
            int index = solverState.Rng.Next(candidates.Count);
            (string description, CellLocation location, int digit) = candidates.ElementAt(index);

            string message = $"{description} can contain {digit} only at {location.ShortString()}.";

            yield return new SetCellCommand(location, digit);

            yield return new PrintMessageCommand(message);
        }
    }
}
