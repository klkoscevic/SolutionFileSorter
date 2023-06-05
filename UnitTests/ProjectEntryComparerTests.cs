using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProjectsInSlnFile;
using System;

namespace UnitTests
{
    [TestClass]
    public class ProjectEntryComparerTests
    {
        ProjectEntryComparer comparer = new ProjectEntryComparer();

        [TestMethod]
        public void CompareReturnsNameComparisonResultForTwoEntriesWithoutParent()
        {
            var pe1 = new ProjectEntry("abc", "guid", Range.Empty);
            var pe2 = new ProjectEntry("abd", "guid", Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe2) < 0);

            var pe3 = new ProjectEntry("abcd", "guid", Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe3) < 0);

            var pe4 = new ProjectEntry("abc", "guid", Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe4) == 0);

            var pe5 = new ProjectEntry("abc c", "guid", Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe5) < 0);

            var pe6 = new ProjectEntry("abc d", "guid", Range.Empty);
            Assert.IsTrue(comparer.Compare(pe5, pe6) < 0);
        }

        [TestMethod]
        public void CompareIgnoresCase()
        {
            var pe1 = new ProjectEntry("abc", "guid", Range.Empty);
            var pe2 = new ProjectEntry("ABC", "guid", Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe2) == 0);
        }

        [TestMethod]
        public void CompareReturnsResultOfParentComparisonIfTheirNamesDiffer()
        {
            var pe1 = new ProjectEntry("abc", "guid", Range.Empty);
            var parent1 = new ProjectEntry("parent1", "guid", Range.Empty);
            pe1.SetParent(parent1, Range.Empty);

            var pe2 = new ProjectEntry("abc", "guid", Range.Empty);
            var parent2 = new ProjectEntry("parent2", "guid", Range.Empty);
            pe2.SetParent(parent2, Range.Empty);

            Assert.IsTrue(comparer.Compare(pe1, pe2) < 0);
        }

        [TestMethod]
        public void CompareReturnsResultOfNameComparisonForCommonParent()
        {
            var pe1 = new ProjectEntry("abcd", "guid", Range.Empty);
            var parent = new ProjectEntry("parent", "guid", Range.Empty);
            pe1.SetParent(parent, Range.Empty);

            var pe2 = new ProjectEntry("abc", "guid", Range.Empty);
            pe2.SetParent(parent, Range.Empty);

            Assert.IsTrue(comparer.Compare(pe1, pe2) > 0);
        }

        [TestMethod]
        public void EntryWithoutParentPrecedesEntryWithParentEvenIfParentAndEntryNamesAreIdentical()
        {
            var pe1 = new ProjectEntry("abc", "guid", Range.Empty);
            var parent1 = new ProjectEntry("abc", "guid", Range.Empty);
            pe1.SetParent(parent1, Range.Empty);

            var pe2 = new ProjectEntry("abc", "guid", Range.Empty);

            Assert.IsTrue(comparer.Compare(pe1, pe2) > 0);
        }

        [TestMethod]
        public void CompareReturnsResultOfComparisonForMultipleParentLevels()
        {
            var pe1 = new ProjectEntry("abc", "guid", Range.Empty);
            var parent1 = new ProjectEntry("abc", "guid", Range.Empty);
            pe1.SetParent(parent1, Range.Empty);
            var grandparent1 = new ProjectEntry("abc", "guid", Range.Empty);
            parent1.SetParent(grandparent1, Range.Empty);

            var pe2 = new ProjectEntry("abc", "guid", Range.Empty);
            var parent2 = new ProjectEntry("abcd", "guid", Range.Empty);
            pe2.SetParent(parent2, Range.Empty);
            var grandparent2 = new ProjectEntry("abc", "guid", Range.Empty);
            parent2.SetParent(grandparent2, Range.Empty);

            Assert.IsTrue(comparer.Compare(pe1, pe2) < 0);
        }

        [TestMethod]
        public void EntryWithSpaceInNameFollowsParentWithTrimmedName()
        {
            var pe1 = new ProjectEntry("abc def", "guid", Range.Empty);

            var pe2 = new ProjectEntry("abc", "guid", Range.Empty);
            var parent2 = new ProjectEntry("abc", "guid", Range.Empty);
            pe2.SetParent(parent2, Range.Empty);

            Assert.IsTrue(comparer.Compare(pe1, pe2) > 0);

            var pe3 = new ProjectEntry("abc", "guid", Range.Empty);
            var parent3 = new ProjectEntry("abc def", "guid", Range.Empty);
            pe3.SetParent(parent3, Range.Empty);

            var pe4 = new ProjectEntry("abc", "guid", Range.Empty);

            Assert.IsTrue(comparer.Compare(pe3, pe4) > 0);
        }
    }
}