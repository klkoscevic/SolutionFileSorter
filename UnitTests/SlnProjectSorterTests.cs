using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortingLibrary;
using System.IO;
using System.Reflection;

namespace UnitTests
{
    [TestClass]
    public class SlnProjectSorterTests
    {
        [TestMethod]
        public void WritesSameContentForEmptySolution()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.EmptySolution");
            using (var reader = new StreamReader(stream))
            using (var writer = new StringWriter())
            {
                var sortedWriter = new SlnProjectsSorter(reader);
                sortedWriter.WriteSorted(writer);

                Assert.AreEqual(sortedWriter.OriginalContent, writer.ToString());
            }
        }

        [TestMethod]
        public void WritesSameContentForSolutionWithSingleProject()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithSingleProject");
            using (var reader = new StreamReader(stream))
            using (var writer = new StringWriter())
            {
                var sortedWriter = new SlnProjectsSorter(reader);
                sortedWriter.WriteSorted(writer);
                var actual = writer.ToString();

                var expected = sortedWriter.OriginalContent;
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void WritesSortedContentForSolutionWithFourProjectInTheRoot()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithFourProjectsInTheRoot");
            using (var reader = new StreamReader(stream))
            using (var writer = new StringWriter())
            {
                var sortedWriter = new SlnProjectsSorter(reader);
                sortedWriter.WriteSorted(writer);
                var actual = writer.ToString();

                stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithFourProjectsInTheRoot.sorted");
                var expected = new StreamReader(stream).ReadToEnd();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void WritesSortedContentForSolutionWithMultipleProjectsOneInSolutionFOlder()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithMultipleProjectsOneInSolutionFolder");
            using (var reader = new StreamReader(stream))
            using (var writer = new StringWriter())
            {
                var sortedWriter = new SlnProjectsSorter(reader);
                sortedWriter.WriteSorted(writer);
                var actual = writer.ToString();

                stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithMultipleProjectsOneInSolutionFolder.sorted");
                var expected = new StreamReader(stream).ReadToEnd();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void WritesSortedContentForSolutionWithLfLineEndings()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithFourProjectsInTheRootLfLineEndings");
            using (var reader = new StreamReader(stream))
            using (var writer = new StringWriter())
            {
                var sortedWriter = new SlnProjectsSorter(reader);
                sortedWriter.WriteSorted(writer);
                var actual = writer.ToString();

                stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnitTests.Resources.SolutionWithFourProjectsInTheRootLfLineEndings.sorted");
                var expected = new StreamReader(stream).ReadToEnd();
                Assert.AreEqual(expected, actual);
            }
        }
    }
}