using System;
using System.Collections.Generic;

namespace SortingLibrary
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