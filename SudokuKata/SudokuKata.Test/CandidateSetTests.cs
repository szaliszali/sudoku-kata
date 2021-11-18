using NUnit.Framework;

namespace SudokuKata.Test;

[TestFixture]
internal class CandidateSetTests
{
    [Test]
    public void CandidatSetIsEmptyByDefault()
    {
        var sut = new CandidateSet();
        Assert.Multiple(() =>
        {
            Assert.That(sut.NumCandidates, Is.EqualTo(0));
            Assert.That(sut.Contains(3), Is.False);
        });
    }

    [Test]
    public void Include()
    {
        var sut = new CandidateSet();
        sut.Include(3);
        Assert.Multiple(() =>
        {
            Assert.That(sut.NumCandidates, Is.EqualTo(1));
            Assert.That(sut.Contains(3), Is.True);
        });
    }

    [Test]
    public void IncludeAll()
    {
        var sut = new CandidateSet();
        sut.IncludeAll();
        Assert.Multiple(() =>
        {
            Assert.That(sut.NumCandidates, Is.EqualTo(9));
            for (int digit = 1; digit <= 9; ++digit) Assert.That(sut.Contains(digit), Is.True, "contains " + digit);
        });
    }

    [Test]
    public void Exclude()
    {
        var sut = new CandidateSet();
        sut.Include(2);
        sut.Include(7);
        sut.Exclude(2);
        Assert.Multiple(() =>
        {
            Assert.That(sut.NumCandidates, Is.EqualTo(1));
            Assert.That(sut.Contains(2), Is.False);
            Assert.That(sut.Contains(7), Is.True);
        });
    }

    [Test]
    public void SingleCandidate()
    {
        var sut = new CandidateSet();
        sut.Include(3);
        Assert.That(sut.SingleCandidate, Is.EqualTo(3));
    }

    [Test]
    public void SingleCandidateThrowsIfNoCandidates()
    {
        var sut = new CandidateSet();
        Assert.That(() => sut.SingleCandidate, Throws.Exception);
    }

    [Test]
    public void SingleCandidateThrowsIfMultipleCandidates()
    {
        var sut = new CandidateSet();
        sut.Include(2);
        sut.Include(3);
        Assert.That(() => sut.SingleCandidate, Throws.Exception);
    }

    [Test]
    public void CandidatePair()
    {
        var sut = new CandidateSet();
        sut.Include(3);
        sut.Include(2);
        Assert.That(sut.CandidatePair, Is.EqualTo((2, 3)));
    }

    [Test]
    public void CandidatePairThrowsIfLessThan2Candidates()
    {
        var sut = new CandidateSet();
        sut.Include(3);
        Assert.That(() => sut.CandidatePair, Throws.Exception);
    }

    [Test]
    public void CandidatePairThrowsIfMoreThan2Candidates()
    {
        var sut = new CandidateSet();
        sut.Include(3);
        sut.Include(4);
        sut.Include(5);
        Assert.That(() => sut.CandidatePair, Throws.Exception);
    }

}
