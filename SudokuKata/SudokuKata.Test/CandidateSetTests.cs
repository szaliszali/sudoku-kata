using NUnit.Framework;

namespace SudokuKata.Test;

[TestFixture]
internal class CandidateSetTests
{
    [Test]
    public void CandidatSetIsEmptyByDefault()
    {
        var sut = new CandidateSet();
        Assert.Multiple(() => {
            Assert.That(sut.NumCandidates, Is.EqualTo(0));
            Assert.That(sut.Candidates, Is.Empty);
        });
    }
}
