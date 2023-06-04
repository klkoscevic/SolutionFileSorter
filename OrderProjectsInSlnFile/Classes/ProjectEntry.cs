﻿using System.Collections.Generic;
using System.Text;

namespace OrderProjectsInSlnFile
{
    public class ProjectEntry
    {
        // Prefer using constructor instead of public setters in order to protect data from uncontrolled changes
        public ProjectEntry(string name, string guid, Range content)
        {
            Name = name;
            Guid = guid;
            Parent = null;
            Content = content;
        }

        public string Name { get; private set; }

        public string Guid { get; private set; }

        public Range Content { get; private set; }

        public ProjectEntry Parent { get; private set; }

        public void AddConfigurationPlatform(Range range)
        {
            configurationPlatforms.Add(range);
        }
        public void SetParent(ProjectEntry parent)
        {
            if (Parent != null)
            {
                const string message = "'{0}' entry already has a parent";
                throw new InvalidOperationException(string.Format(message, Name));
            }
            Parent = parent;
        }

        public string GetParentPath()
        {
            if (Parent == null)
            {
                return string.Empty;
            }
            var result = new StringBuilder(Parent.Name);
            var parent = Parent.Parent;
            while (parent != null)
            {
                result.Insert(0, $"{parent.Name}{ParentDelimiter}");
                parent = parent.Parent;
            }
            return result.ToString();
        }

        private readonly List<Range> configurationPlatforms = new List<Range>();

        public IEnumerable<Range> ConfigurationPlatforms { get { return configurationPlatforms; } }

        // Character before first valid character (space) is used as a delimiter to ensure correct sorting.
        public const char ParentDelimiter = (char)(' ' - 1);
    }
}
