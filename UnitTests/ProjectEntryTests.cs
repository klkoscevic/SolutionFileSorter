using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProjectsInSlnFile;
using System;

namespace UnitTests
{
    [TestClass]
    public class ProjectEntryTests
    {
        [TestMethod]
        public void GetPathReturnsEmptyStringForEntryWithoutParent()
        {
            var entry = new ProjectEntry("Name", "GUID", Range.Empty);
            Assert.AreEqual("", entry.GetFullPath());
        }

        [TestMethod]
        public void SetParentMethodAssignsParentEntry()
        {
            var parent = new ProjectEntry("Parent", "GUID", Range.Empty);
            var child = new ProjectEntry("Child", "GUID", Range.Empty);
            child.SetParent(parent);
            Assert.AreEqual(parent, child.Parent);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetParentThrowsExceptionIfEntryAlreadyHasParent()
        {
            var parent = new ProjectEntry("Parent", "GUID", Range.Empty);
            var child = new ProjectEntry("Child", "GUID", Range.Empty);
            child.SetParent(parent);
            child.SetParent(parent);
        }

        [TestMethod]
        public void GetParentMethodReturnsParentNameForChildWithSingleParent()
        {
            var parent = new ProjectEntry("Parent", "GUID", Range.Empty);
            var child = new ProjectEntry("Child", "GUID", Range.Empty);
            child.SetParent(parent);
            Assert.AreEqual("Parent", child.GetFullPath());
        }

        [TestMethod]
        public void GetParentMethodReturnsConcatenatedParentNamesForChildWithMultipleParents()
        {
            var grandgrandparent = new ProjectEntry("GrandGrandParent", "GUID", Range.Empty);
            var grandparent = new ProjectEntry("GrandParent", "GUID", Range.Empty);
            grandparent.SetParent(grandgrandparent);
            var parent = new ProjectEntry("Parent", "GUID", Range.Empty);
            parent.SetParent(grandparent);
            var child = new ProjectEntry("Child", "GUID", Range.Empty);
            child.SetParent(parent);
            Assert.AreEqual(string.Format("GrandGrandParent{0}GrandParent{0}Parent", ProjectEntry.ParentDelimiter), child.GetFullPath());
        }
    }
}