/*
MIT License

Copyright(c) 2023 Klara Koščević

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingLibrary;
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