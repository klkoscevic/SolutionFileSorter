using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProjectsInSlnFile;
using System;
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
            Assert.AreEqual(322, project.Content.End);
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
            Assert.AreEqual(343, pythonApp.Content.End);

            var mfcApp = projectEntries.FirstOrDefault(pe => pe.Name == "MFCApplication");
            Assert.IsNotNull(mfcApp);
            Assert.AreEqual("{55590147-00C7-4EAE-8CA8-DE4594DA2CAA}", mfcApp.Guid);
            Assert.IsNull(mfcApp.Parent);
            Assert.AreEqual(345, mfcApp.Content.Start);
            Assert.AreEqual(508, mfcApp.Content.End);

            var cppConsoleApp = projectEntries.FirstOrDefault(pe => pe.Name == "CppConsoleApplication");
            Assert.IsNotNull(cppConsoleApp);
            Assert.AreEqual("{A423D6CA-39CF-4C15-BCA9-BCAA327B6E44}", cppConsoleApp.Guid);
            Assert.IsNull(cppConsoleApp.Parent);
            Assert.AreEqual(510, cppConsoleApp.Content.Start);
            Assert.AreEqual(694, cppConsoleApp.Content.End);

            var csharpConsoleApp = projectEntries.FirstOrDefault(pe => pe.Name == "C#ConsoleApp");
            Assert.IsNotNull(csharpConsoleApp);
            Assert.AreEqual("{6A51BE4B-3BBF-4F26-8528-D21593BAEDE8}", csharpConsoleApp.Guid);
            Assert.IsNull(csharpConsoleApp.Parent);
            Assert.AreEqual(696, csharpConsoleApp.Content.Start);
            Assert.AreEqual(852, csharpConsoleApp.Content.End);
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
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1183, 1264)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1265, 1342)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1343, 1420)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1421, 1506)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1507, 1588)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(1589, 1670)));

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
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3519, 3600)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3601, 3680)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3681, 3758)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3759, 3834)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3835, 3912)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3913, 3988)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(3989, 4074)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(4075, 4158)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(4159, 4240)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(4241, 4320)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(4321, 4402)));
            Assert.IsTrue(configurationPlatforms.Contains(new Range(4403, 4482)));
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
    }
}
