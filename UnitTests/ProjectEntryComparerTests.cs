using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProjectsInSlnFile;
using System;

namespace UnitTests
{
    [TestClass]
    public class ProjectEntryComparerTests
    {
        private readonly ProjectEntryComparer comparer = new ProjectEntryComparer();

        [TestMethod]
        public void CompareReturnsNameComparisonResultForTwoProjectEntriesInTheRoot()
        {
            var pe1 = new ProjectEntry("abc", "guid", false, Range.Empty);
            var pe2 = new ProjectEntry("abd", "guid", false, Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe2) < 0);

            var pe3 = new ProjectEntry("abcd", "guid", false, Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe3) < 0);

            var pe4 = new ProjectEntry("abc", "guid", false, Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe4) == 0);

            var pe5 = new ProjectEntry("abc c", "guid", false, Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe5) < 0);

            var pe6 = new ProjectEntry("abc d", "guid", false, Range.Empty);
            Assert.IsTrue(comparer.Compare(pe5, pe6) < 0);
        }

        [TestMethod]
        public void CompareReturnsNameComparisonResultForTwoSolutionFolderEntriesInTheRoot()
        {
            var pe1 = new ProjectEntry("abc", "guid", false, Range.Empty);
            var pe2 = new ProjectEntry("abd", "guid", false, Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe2) < 0);

            var pe3 = new ProjectEntry("abcd", "guid", false, Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe3) < 0);

            var pe4 = new ProjectEntry("abc", "guid", false, Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe4) == 0);

            var pe5 = new ProjectEntry("abc c", "guid", false, Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe5) < 0);

            var pe6 = new ProjectEntry("abc d", "guid", false, Range.Empty);
            Assert.IsTrue(comparer.Compare(pe5, pe6) < 0);
        }

        [TestMethod]
        public void CompareIgnoresCase()
        {
            var pe1 = new ProjectEntry("abc", "guid", true, Range.Empty);
            var pe2 = new ProjectEntry("ABC", "guid", true, Range.Empty);
            Assert.IsTrue(comparer.Compare(pe1, pe2) == 0);
        }

        [TestMethod]
        public void CompareReturnsResultOfParentComparisonIfTheirNamesDiffer()
        {
            var pe1 = new ProjectEntry("abc", "guid", true, Range.Empty);
            var parent1 = new ProjectEntry("parent1", "guid", true, Range.Empty);
            pe1.SetParent(parent1, Range.Empty);

            var pe2 = new ProjectEntry("abc", "guid", true, Range.Empty);
            var parent2 = new ProjectEntry("parent2", "guid", true, Range.Empty);
            pe2.SetParent(parent2, Range.Empty);

            Assert.IsTrue(comparer.Compare(pe1, pe2) < 0);
        }

        [TestMethod]
        public void CompareReturnsResultOfNameComparisonForCommonParent()
        {
            var pe1 = new ProjectEntry("abcd", "guid", true, Range.Empty);
            var parent = new ProjectEntry("parent", "guid", true, Range.Empty);
            pe1.SetParent(parent, Range.Empty);

            var pe2 = new ProjectEntry("abc", "guid", true, Range.Empty);
            pe2.SetParent(parent, Range.Empty);

            Assert.IsTrue(comparer.Compare(pe1, pe2) > 0);
        }

        [TestMethod]
        public void ComparePlacesProjectEntryWithParentSolutionFolderBeforeProjectEntryWithParentSolutionFolderEvenIfParentAndEntryNamesAreIdentical()
        {
            var pe1 = new ProjectEntry("abc", "guid", false, Range.Empty);
            var parent1 = new ProjectEntry("abc", "guid", true, Range.Empty);
            pe1.SetParent(parent1, Range.Empty);

            var pe2 = new ProjectEntry("abc", "guid", false, Range.Empty);

            Assert.IsTrue(comparer.Compare(pe1, pe2) < 0);
        }

        [TestMethod]
        public void CompareReturnsResultOfComparisonForMultipleParentLevels()
        {
            var pe1 = new ProjectEntry("abc", "guid", true, Range.Empty);
            var parent1 = new ProjectEntry("abc", "guid", true, Range.Empty);
            pe1.SetParent(parent1, Range.Empty);
            var grandparent1 = new ProjectEntry("abc", "guid", true, Range.Empty);
            parent1.SetParent(grandparent1, Range.Empty);

            var pe2 = new ProjectEntry("abc", "guid", true, Range.Empty);
            var parent2 = new ProjectEntry("abcd", "guid", true, Range.Empty);
            pe2.SetParent(parent2, Range.Empty);
            var grandparent2 = new ProjectEntry("abc", "guid", true, Range.Empty);
            parent2.SetParent(grandparent2, Range.Empty);

            Assert.IsTrue(comparer.Compare(pe1, pe2) < 0);
        }

        [TestMethod]
        public void ComparePlacesProjectEntryWithSpaceInNameAfterProjectEntryTrimmedName()
        {
            var pe1 = new ProjectEntry("abc def", "guid", false, Range.Empty);
            var pe2 = new ProjectEntry("abc", "guid", false, Range.Empty);

            Assert.IsTrue(comparer.Compare(pe1, pe2) > 0);

            var pe3 = new ProjectEntry("abcd efg", "guid", true, Range.Empty);
            var pe4 = new ProjectEntry("abcd", "guid", true, Range.Empty);

            Assert.IsTrue(comparer.Compare(pe3, pe4) > 0);
        }
    }
}
    
