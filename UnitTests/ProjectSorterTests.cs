using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProjectsInSlnFile;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UnitTests
{
    [TestClass]
    public class ProjectSorterTests
    {
        [TestMethod]
        public void SortOnEntriesInTheRootReturnsProjectsSortedAlphabeticallyByTheirNames()
        {
            IEnumerable<ProjectEntry> projects = new List<ProjectEntry> { new ProjectEntry("m", "guid", Range.Empty), new ProjectEntry("z", "guid", Range.Empty), new ProjectEntry("a", "guid", Range.Empty) };

            var sorted = new ProjectsSorter().GetSorted(projects);

            Assert.AreEqual("a", sorted.ElementAt(0).Name);
            Assert.AreEqual("m", sorted.ElementAt(1).Name);
            Assert.AreEqual("z", sorted.ElementAt(2).Name);
        }

        [TestMethod]
        public void SortOnSolutionWithProjectNamesContainingCroatianAndGermanCharactersReturnsProjectsSortedAlphabetically()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithProjectNamesContainingCroatianAndGermanCharacters");
            SolutionParser slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionParser(reader);
            }

            Assert.AreEqual(14, slnFile.ProjectEntries.Count());

            var sorted = new ProjectsSorter(new System.Globalization.CultureInfo("hr")).GetSorted(slnFile.ProjectEntries);

            Assert.AreEqual("Abc", sorted.ElementAt(0).Name);
            Assert.AreEqual("Cure", sorted.ElementAt(1).Name);
            Assert.AreEqual("Čekić", sorted.ElementAt(2).Name);
            Assert.AreEqual("Ćup", sorted.ElementAt(3).Name);
            Assert.AreEqual("Def", sorted.ElementAt(4).Name);
            Assert.AreEqual("Ober", sorted.ElementAt(5).Name);
        }

        //[TestMethod]
        //public void SortOnProjectsInSolutionFoldersReturnsProjectsSortedAlphabeticallyByTheirSolutionFolderNames()
        //{
        //    var project1 = new ProjectEntry("m", "guid", Range.Empty);
        //    project1.SetParent(new ProjectEntry("z", "guid", Range.Empty), Range.Empty);

        //    var project2 = new ProjectEntry("z", "guid", Range.Empty);
        //    project2.SetParent(new ProjectEntry("a", "guid", Range.Empty), Range.Empty);

        //    var project3 = new ProjectEntry("a", "guid", Range.Empty);
        //    project3.SetParent(new ProjectEntry("c", "guid", Range.Empty), Range.Empty);

        //    IEnumerable<ProjectEntry> projects = new List<ProjectEntry> { project1, project2, project3 };

        //    var sorted = ProjectsSorter.GetSorted(projects);

        //    Assert.AreEqual("z", sorted.ElementAt(0).Name);
        //    Assert.AreEqual("a", sorted.ElementAt(1).Name);
        //    Assert.AreEqual("m", sorted.ElementAt(2).Name);
        //}

        [TestMethod]
        public void IsSortedReturnsFalseForUnsortedEntries()
        {
            IEnumerable<ProjectEntry> projects = new List<ProjectEntry> { new ProjectEntry("m", "guid", Range.Empty), new ProjectEntry("z", "guid", Range.Empty), new ProjectEntry("a", "guid", Range.Empty) };

            Assert.IsFalse(new ProjectsSorter().IsSorted(projects));
        }

        [TestMethod]
        public void IsSortedReturnsTrueForSortedEntries()
        {
            IEnumerable<ProjectEntry> projects = new List<ProjectEntry> { new ProjectEntry("m", "guid", Range.Empty), new ProjectEntry("z", "guid", Range.Empty), new ProjectEntry("a", "guid", Range.Empty) };

            var sorter = new ProjectsSorter();

            var sorted = sorter.GetSorted(projects);

            Assert.IsTrue(sorter.IsSorted(sorted));
        }
    }
}