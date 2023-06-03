﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProjectsInSlnFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class SolutionFileTests
    {

        [TestMethod]
        public void SortDoesntChangeContentForEmptySolution()
        {
            const string sln = "\r\nMicrosoft Visual Studio Solution File, Format Version 12.00\r\n# Visual Studio Version 17\r\nVisualStudioVersion = 17.6.33723.286\r\nMinimumVisualStudioVersion = 10.0.40219.1\r\nGlobal\r\n\tGlobalSection(SolutionProperties) = preSolution\r\n\t\tHideSolutionNode = FALSE\r\n\tEndGlobalSection\r\n\tGlobalSection(ExtensibilityGlobals) = postSolution\r\n\t\tSolutionGuid = {26D0A5FF-D281-4407-9DF4-2189B568BDC9}\r\n\tEndGlobalSection\r\nEndGlobal";

            SolutionFile slnFile = null;
            using (var reader = new StringReader(sln))
            {
                slnFile = new SolutionFile(reader);
            }
            slnFile.Sort();

            IEnumerable<string> original = sln.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            Assert.IsTrue(slnFile.LinesInFile.SequenceEqual(original));
        }

        [TestMethod]
        public void SortDoesntChangeContentForSolutionWithASingleProject()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithASingleProject.original");
            SolutionFile slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionFile(reader);
            }
            slnFile.Sort();

            var expected = ReadLinesFromResource("UnitTests.Resources.SolutionWithASingleProject.sorted");

            Assert.IsTrue(slnFile.LinesInFile.SequenceEqual(expected));
        }

        [TestMethod]
        public void SortDoesntChangeContentForSolutionWithTwoProjectsThatAreSortedAlready()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithTwoProjectsThatAreSortedAlready.original");
            SolutionFile slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionFile(reader);
            }
            slnFile.Sort();

            var expected = ReadLinesFromResource("UnitTests.Resources.SolutionWithTwoProjectsThatAreSortedAlready.sorted");

            Assert.IsTrue(slnFile.LinesInFile.SequenceEqual(expected));
        }

        [TestMethod]
        public void SortReordersContentForSolutionWithTwoProjectsThatAreNotSorted()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithTwoProjectsThatAreNotSorted.original");
            SolutionFile slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionFile(reader);
            }
            slnFile.Sort();

            var expected = ReadLinesFromResource("UnitTests.Resources.SolutionWithTwoProjectsThatAreNotSorted.sorted");

            Assert.IsTrue(slnFile.LinesInFile.SequenceEqual(expected));
        }

        [TestMethod]
        public void SortReordersContentForSolutionWithSeveralProjectsAndFoldersThatAreNotSorted()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithFilesAndFolders.original");
            SolutionFile slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionFile(reader);
            }
            slnFile.Sort();

            var expected = ReadLinesFromResource("UnitTests.Resources.SolutionWithFilesAndFolders.sorted");

            Assert.IsTrue(slnFile.LinesInFile.SequenceEqual(expected));
        }

        private IEnumerable<string> ReadLinesFromResource(string resource)
        {
            var expected = new List<string>();
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
            using (var reader = new StreamReader(stream))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    expected.Add(line);
                }
            }
            return expected;
        }
    }
}

