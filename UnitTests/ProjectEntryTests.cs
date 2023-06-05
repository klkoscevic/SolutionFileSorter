using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProjectsInSlnFile;
using System;

namespace UnitTests
{
    [TestClass]
    public class ProjectEntryTests
    {
        [TestMethod]
        public void SetParentMethodAssignsParentEntry()
        {
            var parent = new ProjectEntry("Parent", "GUID", Range.Empty);
            var child = new ProjectEntry("Child", "GUID", Range.Empty);
            child.SetParent(parent, Range.Empty);
            Assert.AreEqual(parent, child.Parent);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetParentThrowsExceptionIfEntryAlreadyHasParent()
        {
            var parent = new ProjectEntry("Parent", "GUID", Range.Empty);
            var child = new ProjectEntry("Child", "GUID", Range.Empty);
            child.SetParent(parent, Range.Empty);
            child.SetParent(parent, Range.Empty);
        }

        [TestMethod]
        public void GetFullPathhReturnsNameEntryForWithoutParent()
        {
            var entry = new ProjectEntry("Name", "GUID", Range.Empty);
            Assert.AreEqual("Name", entry.GetFullPath());
        }

        [TestMethod]
        public void GetFullPathMethodReturnsParentNameForChildWithSingleParent()
        {
            var parent = new ProjectEntry("Parent", "GUID", Range.Empty);
            var child = new ProjectEntry("Child", "GUID", Range.Empty);
            child.SetParent(parent, Range.Empty);
            Assert.AreEqual(string.Format("Parent{0}Child", ProjectEntry.ParentDelimiter), child.GetFullPath());
        }

        [TestMethod]
        public void GetFullPathMethodReturnsConcatenatedParentNamesForChildWithMultipleParents()
        {
            var grandgrandparent = new ProjectEntry("GrandGrandParent", "GUID", Range.Empty);
            var grandparent = new ProjectEntry("GrandParent", "GUID", Range.Empty);
            grandparent.SetParent(grandgrandparent, Range.Empty);
            var parent = new ProjectEntry("Parent", "GUID", Range.Empty);
            parent.SetParent(grandparent, Range.Empty);
            var child = new ProjectEntry("Child", "GUID", Range.Empty);
            child.SetParent(parent, Range.Empty);
            Assert.AreEqual(string.Format("GrandGrandParent{0}GrandParent{0}Parent{0}Child", ProjectEntry.ParentDelimiter), child.GetFullPath());
        }
    }
}