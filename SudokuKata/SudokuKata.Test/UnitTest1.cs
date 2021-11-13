using System;
using System.IO;
using ApprovalTests;
using NUnit.Framework;

namespace SudokuKata.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            Program.Play();
            Approvals.Verify(output);
        }
    }
}