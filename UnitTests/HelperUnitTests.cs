using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProjectsInSlnFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace UnitTests
{
    [TestClass]
    public class HelperUnitTests
    {
        [TestMethod]
        public void SolutionWithASingleProjectIsSorted()
        {
            string basePath = Directory.GetCurrentDirectory();
            basePath = new DirectoryInfo(basePath).Parent.Parent.FullName;
            string solutionFilePath = Path.Combine(basePath, @"Resources\SolutionWithASingleProject.original");

            Helper helper = new Helper();

            var isSorted = helper.IsSlnFileSorted(solutionFilePath);

            Assert.IsTrue(isSorted);
        }

        [TestMethod]
        public void SolutionWithTwoProjectsIsSorted()
        {
            string basePath = Directory.GetCurrentDirectory();
            basePath = new DirectoryInfo(basePath).Parent.Parent.FullName;
            string solutionFilePath = Path.Combine(basePath, @"Resources\SolutionWithTwoProjectsThatAreSortedAlready.original");

            Helper helper = new Helper();

            var isSorted = helper.IsSlnFileSorted(solutionFilePath);

            Assert.IsTrue(isSorted);
        }

        [TestMethod]
        public void SolutionWithTwoProjectsIsNotSorted()
        {
            string basePath = Directory.GetCurrentDirectory();
            basePath = new DirectoryInfo(basePath).Parent.Parent.FullName;
            string solutionFilePath = Path.Combine(basePath, @"Resources\SolutionWithTwoProjectsThatAreNotSorted.original");

            Helper helper = new Helper();

            var isSorted = helper.IsSlnFileSorted(solutionFilePath);

            Assert.IsFalse(isSorted);
        }

        [TestMethod]
        public void SolutionWithSeveralProjectsAndFoldersIsNotSorted()
        {
            string basePath = Directory.GetCurrentDirectory();
            basePath = new DirectoryInfo(basePath).Parent.Parent.FullName;
            string solutionFilePath = Path.Combine(basePath, @"Resources\SolutionWithFilesAndFolders.original");

            Helper helper = new Helper();

            var isSorted = helper.IsSlnFileSorted(solutionFilePath);

            Assert.IsFalse(isSorted);
        }

        [TestMethod]
        public void SolutionWithSeveralProjectsAndFoldersIsSorted()
        {
            string basePath = Directory.GetCurrentDirectory();
            basePath = new DirectoryInfo(basePath).Parent.Parent.FullName;
            string solutionFilePath = Path.Combine(basePath, @"Resources\SolutionWithFilesAndFolders.sorted");

            Helper helper = new Helper();

            var isSorted = helper.IsSlnFileSorted(solutionFilePath);

            Assert.IsTrue(isSorted);
        }

        //private IEnumerable<string> ReadLinesFromResource(string resource)
        //{
        //    var expected = new List<string>();
        //    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
        //    using (var reader = new StreamReader(stream))
        //    {
        //        string line = null;
        //        while ((line = reader.ReadLine()) != null)
        //        {
        //            expected.Add(line);
        //        }
        //    }
        //    return expected;
        //}
    }
}
