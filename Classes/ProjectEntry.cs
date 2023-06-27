using Microsoft.VisualStudio.RpcContracts.Utilities;
using System.Collections.Generic;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

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

        private readonly List<Range> configurationPlatforms = new List<Range>();

        public IEnumerable<Range> ConfigurationPlatforms { get { return configurationPlatforms; } }
    }
}