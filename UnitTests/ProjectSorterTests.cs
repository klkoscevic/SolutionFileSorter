using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingLibrary;
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
            IEnumerable<ProjectEntry> projects = new List<ProjectEntry> { new ProjectEntry("m", "guid", true, Range.Empty), new ProjectEntry("z", "guid", true, Range.Empty), new ProjectEntry("a", "guid", true, Range.Empty) };

            var sorted = new ProjectsSorter().GetSorted(projects);

            Assert.AreEqual("a", sorted.ElementAt(0).Name);
            Assert.AreEqual("m", sorted.ElementAt(1).Name);
            Assert.AreEqual("z", sorted.ElementAt(2).Name);
        }

        [TestMethod]
        public void SortOnProjectsInSolutionFoldersReturnsProjectsSortedAlphabeticallyByTheirSolutionFolderNames()
        {
            var project1 = new ProjectEntry("m", "guid", false, Range.Empty);
            project1.SetParent(new ProjectEntry("z", "guid", true, Range.Empty), Range.Empty);

            var project2 = new ProjectEntry("z", "guid", false, Range.Empty);
            project2.SetParent(new ProjectEntry("a", "guid", true, Range.Empty), Range.Empty);

            var project3 = new ProjectEntry("a", "guid", false, Range.Empty);
            project3.SetParent(new ProjectEntry("c", "guid", true, Range.Empty), Range.Empty);

            IEnumerable<ProjectEntry> projects = new List<ProjectEntry> { project1, project2, project3 };

            var sorted = new ProjectsSorter().GetSorted(projects);

            Assert.AreEqual("z", sorted.ElementAt(0).Name);
            Assert.AreEqual("a", sorted.ElementAt(1).Name);
            Assert.AreEqual("m", sorted.ElementAt(2).Name);
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
            Assert.AreEqual("Öffnen", sorted.ElementAt(6).Name);
            Assert.AreEqual("Ozean", sorted.ElementAt(7).Name);
            Assert.AreEqual("Ua", sorted.ElementAt(8).Name);
            Assert.AreEqual("Über", sorted.ElementAt(9).Name);
            Assert.AreEqual("Unter", sorted.ElementAt(10).Name);
            Assert.AreEqual("Visual", sorted.ElementAt(11).Name);
            Assert.AreEqual("Zubar", sorted.ElementAt(12).Name);
            Assert.AreEqual("Žezlo", sorted.ElementAt(13).Name);
        }

        [TestMethod]
        public void SortOnSolutionWithMultilineProject()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithVsPackageProject");
            SolutionParser slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionParser(reader);
            }

            Assert.AreEqual(17, slnFile.ProjectEntries.Count());

            var sorted = new ProjectsSorter(new System.Globalization.CultureInfo("hr")).GetSorted(slnFile.ProjectEntries);

            Assert.AreEqual("Libraries", sorted.ElementAt(0).Name);
            Assert.AreEqual("C++ Libraries", sorted.ElementAt(1).Name);
            Assert.AreEqual("VS Package", sorted.ElementAt(2).Name);
            Assert.AreEqual("VSPackage", sorted.ElementAt(3).Name);
            Assert.AreEqual("VSPackageUI", sorted.ElementAt(4).Name);
            Assert.AreEqual("VSPackageVSIX", sorted.ElementAt(5).Name);
            Assert.AreEqual("MFCLibrary", sorted.ElementAt(6).Name);
            Assert.AreEqual("RaspberryBlink", sorted.ElementAt(7).Name);
            Assert.AreEqual("C#NetFrameworkClassLibrary", sorted.ElementAt(8).Name);
            Assert.AreEqual("ClassLibraryC#", sorted.ElementAt(9).Name);
            Assert.AreEqual("ClassLibraryUniversalWindows", sorted.ElementAt(10).Name);
            Assert.AreEqual("VbNetClassLibrary", sorted.ElementAt(11).Name);
            Assert.AreEqual("WpfCustomControlLibrary", sorted.ElementAt(12).Name);
            Assert.AreEqual("Python projects", sorted.ElementAt(13).Name);
            Assert.AreEqual("ApplicationInPython", sorted.ElementAt(14).Name);
            Assert.AreEqual("ConsoleApplication", sorted.ElementAt(15).Name);
            Assert.AreEqual("WinFormsApp", sorted.ElementAt(16).Name);
        }

        [TestMethod]
        public void SortOnSolutionWithWhitespacesInProjectAndFolderNamesReturnsProjectsSortedAlphabetically()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithWhitespacesInProjectAndFolderNames");
            SolutionParser slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionParser(reader);
            }

            Assert.AreEqual(10, slnFile.ProjectEntries.Count());

            var sorted = new ProjectsSorter().GetSorted(slnFile.ProjectEntries);

            Assert.AreEqual("folder ab c", sorted.ElementAt(0).Name);
            Assert.AreEqual("ab b", sorted.ElementAt(1).Name);
            Assert.AreEqual("ab c", sorted.ElementAt(2).Name);
            Assert.AreEqual("folder abc", sorted.ElementAt(3).Name);
            Assert.AreEqual("ab a", sorted.ElementAt(4).Name);
            Assert.AreEqual("folder abc d", sorted.ElementAt(5).Name);
            Assert.AreEqual("abc", sorted.ElementAt(6).Name);
            Assert.AreEqual("abc d", sorted.ElementAt(7).Name);
            Assert.AreEqual("abcd", sorted.ElementAt(8).Name);
            Assert.AreEqual("abcd ef", sorted.ElementAt(9).Name);
        }

        [TestMethod]
        public void IsSortedReturnsFalseForUnsortedEntries()
        {
            IEnumerable<ProjectEntry> projects = new List<ProjectEntry> { new ProjectEntry("m", "guid", true, Range.Empty), new ProjectEntry("z", "guid", true, Range.Empty), new ProjectEntry("a", "guid", true, Range.Empty) };

            Assert.IsFalse(new ProjectsSorter().IsSorted(projects));
        }

        [TestMethod]
        public void IsSortedReturnsTrueForSortedEntries()
        {
            IEnumerable<ProjectEntry> projects = new List<ProjectEntry> { new ProjectEntry("m", "guid", true, Range.Empty), new ProjectEntry("z", "guid", true, Range.Empty), new ProjectEntry("a", "guid", true, Range.Empty) };

            var sorter = new ProjectsSorter();

            var sorted = sorter.GetSorted(projects);

            Assert.IsTrue(sorter.IsSorted(sorted));
        }
    }
}