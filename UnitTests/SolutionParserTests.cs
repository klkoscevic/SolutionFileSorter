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
using System.IO;
using System.Linq;
using System.Reflection;

namespace UnitTests
{
    [TestClass]
    public class SolutionParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(FileFormatException))]
        public void ConstructorThrowsExceptionForFileWithMissingHeader()
        {
            const string sln = "\r\n# Visual Studio Version 17";
            using (var reader = new StringReader(sln))
            {
                new SolutionParser(reader);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FileFormatException))]
        public void ConstructorThrowsExceptionForFileThatDoesNotStartWithHeader()
        {
            const string sln = "a\r\n\r\nMicrosoft Visual Studio Solution File, Format Version 12.00";
            using (var reader = new StringReader(sln))
            {
                new SolutionParser(reader);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FileFormatException))]
        public void ConstructorThrowsExceptionIfHeaderHasVersionIsLessThan8()
        {
            const string sln = "\r\n\r\nMicrosoft Visual Studio Solution File, Format Version 7.00";
            using (var reader = new StringReader(sln))
            {
                new SolutionParser(reader);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FileFormatException))]
        public void ConstructorThrowsExceptionIfHeaderVersionIsNotFollowedByTwoDigitsAfterDot()
        {
            const string sln = "\r\n\r\nMicrosoft Visual Studio Solution File, Format Version 12.";
            using (var reader = new StringReader(sln))
            {
                new SolutionParser(reader);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FileFormatException))]
        public void ConstructorThrowsExceptionIfHeaderVersionIsFollowedByTwoZeroesAfterDot()
        {
            const string sln = "\r\n\r\nMicrosoft Visual Studio Solution File, Format Version 12.01";
            using (var reader = new StringReader(sln))
            {
                new SolutionParser(reader);
            }
        }

        [TestMethod]
        public void ProjectEntriesReturnsEmptyCollectionForEmptySolution()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.EmptySolution");
            SolutionParser slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionParser(reader);
            }

            Assert.IsFalse(slnFile.ProjectEntries.Any());
        }

        [TestMethod]
        public void ProjectEntriesReturnsCollectionWithOneElementForSolutionWithASingleProject()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithSingleProject");
            SolutionParser slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionParser(reader);
            }

            Assert.AreEqual(1, slnFile.ProjectEntries.Count());

            var project = slnFile.ProjectEntries.First();
            Assert.AreEqual("ConsoleApp", project.Name);
            Assert.AreEqual("{86B087EE-2139-42D2-9A8D-425FC7AF5582}", project.Guid);
            Assert.IsNull(project.Parent);
            Assert.AreEqual(172, project.Content.Start);
            Assert.AreEqual(324, project.Content.End);
        }

        [TestMethod]
        public void ProjectEntriesReturnsCollectionWithAllElementForSolutionWithFourProjectsInTheRoot()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithFourProjectsInTheRoot");
            SolutionParser slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionParser(reader);
            }

            var projectEntries = slnFile.ProjectEntries;
            Assert.AreEqual(4, projectEntries.Count());

            var pythonApp = projectEntries.FirstOrDefault(pe => pe.Name == "PythonApplication");
            Assert.IsNotNull(pythonApp);
            Assert.AreEqual("{DABC7AC1-CDA4-4E05-95F7-FCCA53AA5ECA}", pythonApp.Guid);
            Assert.IsNull(pythonApp.Parent);
            Assert.AreEqual(172, pythonApp.Content.Start);
            Assert.AreEqual(345, pythonApp.Content.End);

            var mfcApp = projectEntries.FirstOrDefault(pe => pe.Name == "MFCApplication");
            Assert.IsNotNull(mfcApp);
            Assert.AreEqual("{55590147-00C7-4EAE-8CA8-DE4594DA2CAA}", mfcApp.Guid);
            Assert.IsNull(mfcApp.Parent);
            Assert.AreEqual(345, mfcApp.Content.Start);
            Assert.AreEqual(510, mfcApp.Content.End);

            var cppConsoleApp = projectEntries.FirstOrDefault(pe => pe.Name == "CppConsoleApplication");
            Assert.IsNotNull(cppConsoleApp);
            Assert.AreEqual("{A423D6CA-39CF-4C15-BCA9-BCAA327B6E44}", cppConsoleApp.Guid);
            Assert.IsNull(cppConsoleApp.Parent);
            Assert.AreEqual(510, cppConsoleApp.Content.Start);
            Assert.AreEqual(696, cppConsoleApp.Content.End);

            var csharpConsoleApp = projectEntries.FirstOrDefault(pe => pe.Name == "C#ConsoleApp");
            Assert.IsNotNull(csharpConsoleApp);
            Assert.AreEqual("{6A51BE4B-3BBF-4F26-8528-D21593BAEDE8}", csharpConsoleApp.Guid);
            Assert.IsNull(csharpConsoleApp.Parent);
            Assert.AreEqual(696, csharpConsoleApp.Content.Start);
            Assert.AreEqual(854, csharpConsoleApp.Content.End);
        }

        [TestMethod]
        public void ProjectEntriesReturnsCollectionWithAllElementForSolutionWithFourProjectsInTheRootLfLineEndings()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithFourProjectsInTheRootLfLineEndings");
            SolutionParser slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionParser(reader);
            }

            var projectEntries = slnFile.ProjectEntries;
            Assert.AreEqual(4, projectEntries.Count());

            var pythonApp = projectEntries.FirstOrDefault(pe => pe.Name == "PythonApplication");
            Assert.IsNotNull(pythonApp);
            Assert.AreEqual("{DABC7AC1-CDA4-4E05-95F7-FCCA53AA5ECA}", pythonApp.Guid);
            Assert.IsNull(pythonApp.Parent);
            Assert.AreEqual(172, pythonApp.Content.Start);
            Assert.AreEqual(345, pythonApp.Content.End);

            var mfcApp = projectEntries.FirstOrDefault(pe => pe.Name == "MFCApplication");
            Assert.IsNotNull(mfcApp);
            Assert.AreEqual("{55590147-00C7-4EAE-8CA8-DE4594DA2CAA}", mfcApp.Guid);
            Assert.IsNull(mfcApp.Parent);
            Assert.AreEqual(345, mfcApp.Content.Start);
            Assert.AreEqual(510, mfcApp.Content.End);

            var cppConsoleApp = projectEntries.FirstOrDefault(pe => pe.Name == "CppConsoleApplication");
            Assert.IsNotNull(cppConsoleApp);
            Assert.AreEqual("{A423D6CA-39CF-4C15-BCA9-BCAA327B6E44}", cppConsoleApp.Guid);
            Assert.IsNull(cppConsoleApp.Parent);
            Assert.AreEqual(510, cppConsoleApp.Content.Start);
            Assert.AreEqual(696, cppConsoleApp.Content.End);

            var csharpConsoleApp = projectEntries.FirstOrDefault(pe => pe.Name == "C#ConsoleApp");
            Assert.IsNotNull(csharpConsoleApp);
            Assert.AreEqual("{6A51BE4B-3BBF-4F26-8528-D21593BAEDE8}", csharpConsoleApp.Guid);
            Assert.IsNull(csharpConsoleApp.Parent);
            Assert.AreEqual(696, csharpConsoleApp.Content.Start);
            Assert.AreEqual(854, csharpConsoleApp.Content.End);
        }

        [TestMethod]
        public void ProjectEntriesReturnsCollectionWithAllElementForSolutionWithFourProjectsInTheRootWithVariousSpacings()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithFourProjectsInTheRootWithVariousSpacings");
            SolutionParser slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionParser(reader);
            }

            var projectEntries = slnFile.ProjectEntries;
            Assert.AreEqual(4, projectEntries.Count());

            var pythonApp = projectEntries.FirstOrDefault(pe => pe.Name == "PythonApplication");
            Assert.IsNotNull(pythonApp);
            Assert.AreEqual("{DABC7AC1-CDA4-4E05-95F7-FCCA53AA5ECA}", pythonApp.Guid);
            Assert.IsNull(pythonApp.Parent);
            Assert.AreEqual(172, pythonApp.Content.Start);
            Assert.AreEqual(356, pythonApp.Content.End);

            var mfcApp = projectEntries.FirstOrDefault(pe => pe.Name == "MFCApplication");
            Assert.IsNotNull(mfcApp);
            Assert.AreEqual("{55590147-00C7-4EAE-8CA8-DE4594DA2CAA}", mfcApp.Guid);
            Assert.IsNull(mfcApp.Parent);
            Assert.AreEqual(356, mfcApp.Content.Start);
            Assert.AreEqual(517, mfcApp.Content.End);

            var cppConsoleApp = projectEntries.FirstOrDefault(pe => pe.Name == "CppConsoleApplication");
            Assert.IsNotNull(cppConsoleApp);
            Assert.AreEqual("{A423D6CA-39CF-4C15-BCA9-BCAA327B6E44}", cppConsoleApp.Guid);
            Assert.IsNull(cppConsoleApp.Parent);
            Assert.AreEqual(517, cppConsoleApp.Content.Start);
            Assert.AreEqual(703, cppConsoleApp.Content.End);

            var csharpConsoleApp = projectEntries.FirstOrDefault(pe => pe.Name == "C#ConsoleApp");
            Assert.IsNotNull(csharpConsoleApp);
            Assert.AreEqual("{6A51BE4B-3BBF-4F26-8528-D21593BAEDE8}", csharpConsoleApp.Guid);
            Assert.IsNull(csharpConsoleApp.Parent);
            Assert.AreEqual(703, csharpConsoleApp.Content.Start);
            Assert.AreEqual(857, csharpConsoleApp.Content.End);
        }


        [TestMethod]
        public void ProjectEntriesCollectionContainsRangesForProjectConfigurationSections()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithFourProjectsInTheRoot");
            SolutionParser slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionParser(reader);
            }

            var projectEntries = slnFile.ProjectEntries;
            Assert.AreEqual(4, projectEntries.Count());

            var pythonApp = projectEntries.FirstOrDefault(pe => pe.Name == "PythonApplication");
            Assert.IsNotNull(pythonApp);
            var configurationPlatforms = pythonApp.ConfigurationPlatforms;
            Assert.AreEqual(6, configurationPlatforms.Count());
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1183, 1265)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1265, 1343)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1343, 1421)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1421, 1507)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1507, 1589)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1589, 1671)));

            var mfcApp = projectEntries.FirstOrDefault(pe => pe.Name == "MFCApplication");
            Assert.IsNotNull(mfcApp);
            configurationPlatforms = mfcApp.ConfigurationPlatforms;
            Assert.AreEqual(12, configurationPlatforms.Count());

            var cppConsoleApp = projectEntries.FirstOrDefault(pe => pe.Name == "CppConsoleApplication");
            Assert.IsNotNull(cppConsoleApp);
            configurationPlatforms = cppConsoleApp.ConfigurationPlatforms;
            Assert.AreEqual(12, configurationPlatforms.Count());

            var csharpConsoleApp = projectEntries.FirstOrDefault(pe => pe.Name == "C#ConsoleApp");
            Assert.IsNotNull(csharpConsoleApp);
            configurationPlatforms = csharpConsoleApp.ConfigurationPlatforms;
            Assert.AreEqual(12, configurationPlatforms.Count());
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3519, 3601)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3601, 3681)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3681, 3759)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3759, 3835)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3835, 3913)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3913, 3989)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3989, 4075)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(4075, 4159)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(4159, 4241)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(4241, 4321)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(4321, 4403)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(4403, 4483)));
        }

        [TestMethod]
        public void ProjectEntryProvidesParentPathForAProjectInSolutionFolder()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithMultipleProjectsOneInSolutionFolder");
            SolutionParser slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionParser(reader);
            }

            var projectEntries = slnFile.ProjectEntries;
            Assert.AreEqual(4, projectEntries.Count());

            var solutionFolder = projectEntries.FirstOrDefault(pe => pe.Name == "Folder with project");
            Assert.IsNotNull(solutionFolder);

            var cppConsoleApp = projectEntries.FirstOrDefault(pe => pe.Name == "VisualCppConsoleApplication");
            Assert.IsNotNull(cppConsoleApp);
            Assert.AreEqual(solutionFolder, cppConsoleApp.Parent);
        }

        [TestMethod]
        public void ProjectEntriesCollectionContainsRangesForProjectEntryWithMultipleLines()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithVsPackageProject");
            SolutionParser slnFile = null;
            using (var reader = new StreamReader(stream))
            {
                slnFile = new SolutionParser(reader);
            }

            var projectEntries = slnFile.ProjectEntries;
            Assert.AreEqual(17, projectEntries.Count());

            var vsPackageApp = projectEntries.FirstOrDefault(pe => pe.Name == "VSPackage");
            Assert.IsNotNull(vsPackageApp);
            Assert.AreEqual(new Range(1295, 1600), vsPackageApp.Content);
            Assert.AreEqual(20, vsPackageApp.ConfigurationPlatforms.Count());
            var vsPackageFolder = projectEntries.FirstOrDefault(pe => pe.Name == "VS Package");
            Assert.IsNotNull(vsPackageFolder);
            Assert.AreEqual(vsPackageFolder, vsPackageApp.Parent);
        }
    }
}
