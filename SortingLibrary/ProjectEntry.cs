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

using System;
using System.Collections.Generic;

namespace KKoščević.SolutionFileSorter.Shared
{
    public class ProjectEntry
    {
        // Prefer using constructor instead of public setters in order to protect data from uncontrolled changes
        public ProjectEntry(string name, string guid, bool isSolutionFolder, Range content)
        {
            Name = name;
            Guid = guid;
            IsSolutionFolder = isSolutionFolder;
            Parent = null;
            Content = content;
            Nesting = Range.Empty;
        }

        public bool IsSolutionFolder { get; private set; }

        public string Name { get; private set; }

        public string Guid { get; private set; }

        public Range Content { get; private set; }

        public ProjectEntry Parent { get; private set; }

        public Range Nesting { get; private set; }

        public void AddConfigurationPlatform(Range range)
        {
            configurationPlatforms.Add(range);
        }

        public void SetParent(ProjectEntry parent, Range nesting)
        {
            if (Parent != null)
            {
                throw new InvalidOperationException($"'{Name}' entry already has a parent");
            }
            if (!parent.IsSolutionFolder)
            {
                throw new InvalidOperationException($"Parent '{parent.Name}' is not a solution folder");
            }
            Parent = parent;
            Nesting = nesting;
        }

        // Gets full path with this project entry as the first elements, followed by optional
        // parent project enties in reverse order.
        public Stack<ProjectEntry> GetFullPath()
        {
            var result = new Stack<ProjectEntry>();
            var parent = this;
            do
            {
                result.Push(parent);
            } while ((parent = parent.Parent) != null);
            return result;
        }

        private readonly List<Range> configurationPlatforms = new List<Range>();

        public IEnumerable<Range> ConfigurationPlatforms { get { return configurationPlatforms; } }


    }
}