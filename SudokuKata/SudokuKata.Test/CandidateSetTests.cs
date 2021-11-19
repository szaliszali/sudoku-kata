﻿using NUnit.Framework;

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
    public void Clear()
    {
        var sut = new CandidateSet();
        sut.Include(2);
        sut.Include(5);
        sut.Clear();
        Assert.That(sut.NumCandidates, Is.Zero);
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

    [Test]
    public void HasAtLeastOneCommon()
    {
        var sut = new CandidateSet();
        sut.Include(2);
        sut.Include(3);

        var other = new CandidateSet();
        other.Include(1);
        other.Include(3);

        var otherDistinct = new CandidateSet();
        otherDistinct.Include(1);
        otherDistinct.Include(7);

        Assert.Multiple(() =>
        {
            Assert.That(sut.HasAtLeastOneCommon(other), Is.True);
            Assert.That(sut.HasAtLeastOneCommon(otherDistinct), Is.False);
        });
    }

    [Test]
    public void HasAtLeastOneDifferent()
    {
        var sut = new CandidateSet();
        sut.Include(2);
        sut.Include(3);

        var other = new CandidateSet();
        other.Include(1);
        other.Include(3);

        var otherSame = new CandidateSet();
        otherSame.Include(2);
        otherSame.Include(3);

        var otherSubset = new CandidateSet();
        otherSubset.Include(3);

        Assert.Multiple(() =>
        {
            Assert.That(sut.HasAtLeastOneDifferent(other), Is.True);
            Assert.That(sut.HasAtLeastOneDifferent(otherSame), Is.False);
            Assert.That(sut.HasAtLeastOneDifferent(otherSubset), Is.False);
        });
    }

    [Test]
    public void CandidateSetEquals()
    {
        CandidateSet sut = new CandidateSet();
        sut.Include(3);

        CandidateSet otherSame = new CandidateSet();
        otherSame.Include(3);

        CandidateSet otherDifferent = new CandidateSet();
        otherDifferent.Include(1);

        Assert.Multiple(() =>
        {
            Assert.That(sut.Equals(sut), Is.True, "Equals() itself");
            Assert.That(sut.Equals(otherSame), Is.True, "Equals() different object with same value");
            Assert.That(sut.Equals(otherDifferent), Is.False, "does not Equals() different object with different value");

            Assert.That(sut == sut, Is.True, "== itself");
            Assert.That(sut == otherSame, Is.True, "== different object with same value");
            Assert.That(sut == otherDifferent, Is.False, "does not == different object with different value");

            Assert.That(sut != sut, Is.False, "not == itself");
            Assert.That(sut != otherSame, Is.False, "not != different object with same value");
            Assert.That(sut != otherDifferent, Is.True, "!= different object with different value");
        });
    }

    [Test]
    public void AllPossibleCandidateSets()
    {
        var sut = CandidateSet.AllPossibleCandidateSets;
        var only9 = new CandidateSet();
        only9.Include(9);
        Assert.Multiple(() =>
        {
            Assert.That(sut.Count, Is.EqualTo(1 << 9));
            Assert.That(sut[256], Is.EqualTo(only9));
        });
    }
}
