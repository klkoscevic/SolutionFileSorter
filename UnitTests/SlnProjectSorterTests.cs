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
using KKoščević.SolutionFileSorter.Shared;
using System.IO;
using System.Reflection;

namespace KKoščević.SolutionFileSorter.UnitTests
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