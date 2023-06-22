using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProjectsInSlnFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class ProjectEntryTests
    {
        [TestMethod]
        public void SetParentMethodAssignsParentEntry()
        {
            var parent = new ProjectEntry("Parent", "GUID", true, Range.Empty);
            var child = new ProjectEntry("Child", "GUID", false, Range.Empty);
            child.SetParent(parent, Range.Empty);
            Assert.AreEqual(parent, child.Parent);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetParentThrowsExceptionIfEntryAlreadyHasParent()
        {
            var parent = new ProjectEntry("Parent", "GUID", true, Range.Empty);
            var child = new ProjectEntry("Child", "GUID", true, Range.Empty);
            child.SetParent(parent, Range.Empty);
            child.SetParent(parent, Range.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetParentThrowsExceptionIfParentIsNotSolutionFolder()
        {
            var parent = new ProjectEntry("Parent", "GUID", false, Range.Empty);
            var child = new ProjectEntry("Child", "GUID", true, Range.Empty);
            child.SetParent(parent, Range.Empty);
        }

        [TestMethod]
        public void GetFullPathMethodReturnsStackWithsNameEntryForAnEntryWithoutParent()
        {
            var singleEntry = new ProjectEntry("Name", "GUID", true, Range.Empty);
            
            var pathNames = singleEntry.GetFullPath().Select(entry => entry.Name);
            Assert.IsTrue(pathNames.SequenceEqual(new string[] { "Name" }));
        }

        [TestMethod]
        public void GetFullPathMethodReturnsStackWithParentNameForChildWithSingleParent()
        {
            var parent = new ProjectEntry("Parent", "GUID", true, Range.Empty);
            var child = new ProjectEntry("Child", "GUID", true, Range.Empty);
            child.SetParent(parent, Range.Empty);
            
            var pathNames = child.GetFullPath().Select(entry => entry.Name);
            Assert.IsTrue(pathNames.SequenceEqual(new Stack<string>(new string[] { "Child", "Parent" })));
        }

        [TestMethod]
        public void GetFullPathMethodReturnsStackWithParentNamesForChildWithMultipleParents()
        {
            var grandgrandparent = new ProjectEntry("GrandGrandParent", "GUID", true, Range.Empty);
            var grandparent = new ProjectEntry("GrandParent", "GUID", true, Range.Empty);
            grandparent.SetParent(grandgrandparent, Range.Empty);
            var parent = new ProjectEntry("Parent", "GUID", true, Range.Empty);
            parent.SetParent(grandparent, Range.Empty);
            var child = new ProjectEntry("Child", "GUID", true, Range.Empty);
            child.SetParent(parent, Range.Empty);

            var pathNames = child.GetFullPath().Select(entry => entry.Name);
            Assert.IsTrue(pathNames.SequenceEqual(new Stack<string>(new string[] { "Child", "Parent", "GrandParent", "GrandGrandParent" })));
        }
    }
}
