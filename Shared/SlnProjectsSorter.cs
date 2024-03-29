﻿/*
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

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace KKoščević.SolutionFileSorter.Shared
{
    /// <summary>
    /// Class that writes sorted project entires. 
    /// </summary>
    public class SlnProjectsSorter
    {
        /// <summary>
        /// Creates instance of class that sorts project entries using <c>CultureInfo.CurrentCulture</c>.
        /// </summary>
        /// <param name="reader">Reader used to read .sln file content.</param>
        public SlnProjectsSorter(TextReader reader) : this(reader, CultureInfo.CurrentCulture)
        {
        }

        /// <summary>
        /// Creates instance of class that sorts project entries using <c>CultureInfo</c> provided.
        /// </summary>
        /// <param name="reader">Reader used to read .sln file content.</param>
        /// <param name="cultureInfo"><c>CultureInfo</c> used for sorting.</param>
        public SlnProjectsSorter(TextReader reader, CultureInfo cultureInfo)
        {
            parser = new SolutionParser(reader);
            projectEntries = parser.ProjectEntries;

            var sorter = new ProjectsSorter(cultureInfo);
            if (sorter.IsSorted(projectEntries))
            {
                AlreadySorted = true;
                return;
            }
            projectEntries = sorter.GetSorted(projectEntries);
        }

        /// <summary>
        /// Writes .sln file content with project items sorted.
        /// </summary>
        /// <param name="writer">Writer used to output the content.</param>
        public void WriteSorted(TextWriter writer)
        {
            var originalFileContent = parser.FileContent;
            // Copy original .sln file prologue.
            var prologue = originalFileContent.Substring(0, parser.Projects.Start);
            writer.Write(prologue);
            // Write Project entries sorted.
            foreach (var project in projectEntries.Select(pe => originalFileContent.Substring(pe.Content.Start, pe.Content.End - pe.Content.Start)))
            {
                writer.Write(project);
            }
            // Write original .sln file content up to platform configurations for individual projects.
            writer.Write(originalFileContent.Substring(parser.Projects.End, parser.ProjectConfigurationPlatforms.Start - parser.Projects.End));
            // Write platform configurations sorted by project entries.
            foreach (var projectEntry in projectEntries)
            {
                foreach (var configurationPlatform in projectEntry.ConfigurationPlatforms.Select(cp => originalFileContent.Substring(cp.Start, cp.End - cp.Start)))
                {
                    writer.Write(configurationPlatform);

                }
            }
            if (!parser.ProjectNestings.IsEmpty())
            {
                // Write original .sln file content from platform configurations up to nested projects section.
                writer.Write(originalFileContent.Substring(parser.ProjectConfigurationPlatforms.End, parser.ProjectNestings.Start - parser.ProjectConfigurationPlatforms.End));
                // Write project nestings sorted by project entries.
                foreach (var nesting in projectEntries.Select(pe => originalFileContent.Substring(pe.Nesting.Start, pe.Nesting.End - pe.Nesting.Start)))
                {
                    writer.Write(nesting);
                }
                // Copy original .sln file epilogue.
                var epilogue = originalFileContent.Substring(parser.ProjectNestings.End);
                writer.Write(epilogue);
            }
            else
            {
                writer.Write(originalFileContent.Substring(parser.ProjectConfigurationPlatforms.End));
            }
        }

        /// <summary>
        /// Are project entries already sorted in the original .sln file content.
        /// </summary>
        public readonly bool AlreadySorted = false;

        public string OriginalContent { get { return parser.FileContent; } }

        private readonly SolutionParser parser;
        private readonly IEnumerable<ProjectEntry> projectEntries;
    }
}