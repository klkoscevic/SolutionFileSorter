using System.Collections.Generic;

namespace OrderProjectsInSlnFile
{
    /// <summary>
    /// Represents a project entry in .sln file. Two types of entries are distinguished: solution folders or projects.
    /// Solution folder can contain one or more projects and/or solution folders.
    /// </summary>
    public class ProjectEntry
    {
        /// <summary>
        /// Initializes project entry.
        /// </summary>
        /// <param name="name">Project entry name.</param>
        /// <param name="guid">Project GUID.</param>
        /// <param name="isSolutionFolder">
        /// Flag if entry is a solution folder. If this argument is <c>true</c> then entry is solution folder, 
        /// if it is <c>false</c> then the entry is a project.
        /// </param>
        /// <param name="content">Range inside of .sln file with project information.</param>
        public ProjectEntry(string name, string guid, bool isSolutionFolder, Range content)
        {
            Name = name;
            Guid = guid;
            IsSolutionFolder = isSolutionFolder;
            Content = content;
            Parent = null;
            Nesting = Range.Empty;
        }

        /// <summary>
        /// Is entry a solution folder. If this property is <c>true</c> then entry is solution folder, 
        /// if it is <c>false</c> then the entry is a project.
        /// </summary>
        public bool IsSolutionFolder { get; private set; }

        /// <summary>
        /// Name of the project.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// GUID of the project.
        /// </summary>
        public string Guid { get; private set; }

        /// <summary>
        /// Range in the original .sln file inside which project entry is defined, from leading <c>Project</c> 
        /// tag to closing <c>EndProject</c> tag. Range includes tags and trailing CR/LF characters. For example:
        /// <code>
        /// Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "WinFormsApp", "WinFormsApp\WinFormsApp.csproj", "{92FC289C-7B86-4FF6-8390-1D9B3C0301F8}"
        /// EndProject
        /// </code>
        /// </summary>
        public Range Content { get; private set; }

        /// <summary>
        /// Parent solution folder entry. For entries in the root this property is null.
        /// </summary>
        public ProjectEntry Parent { get; private set; }

        /// <summary>
        /// Range in the original .sln file inside which nesting for this entry is defined (in section GlobalSection(NestedProjects)). 
        /// Range covers entire line, including trailing CR/LF characters. For example:
        /// <code>
        /// 		{DE9AC350-A60B-46DF-AE8A-EBD510ABBEBC} = {62FBCEB2-EA6C-4083-BDE3-54E892F34295}
        /// </code>
        /// </summary>
        public Range Nesting { get; private set; }

        /// <summary>
        /// Adds range in .sln file that contains corresponding line with configuration platform (in section GlobalSection(ProjectConfigurationPlatforms)).
        /// Example of 6 configuration platforms for a project with GUID {DABC7AC1-CDA4-4E05-95F7-FCCA53AA5ECA}:
        /// <code>
		///        {DABC7AC1-CDA4-4E05-95F7-FCCA53AA5ECA}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		///        {DABC7AC1-CDA4-4E05-95F7-FCCA53AA5ECA}.Debug|x64.ActiveCfg = Debug|Any CPU
		///        {DABC7AC1-CDA4-4E05-95F7-FCCA53AA5ECA}.Debug|x86.ActiveCfg = Debug|Any CPU
		///        {DABC7AC1-CDA4-4E05-95F7-FCCA53AA5ECA}.Release|Any CPU.ActiveCfg = Release|Any CPU
		///        {DABC7AC1-CDA4-4E05-95F7-FCCA53AA5ECA}.Release|x64.ActiveCfg = Release|Any CPU
		///        {DABC7AC1-CDA4-4E05-95F7-FCCA53AA5ECA}.Release|x86.ActiveCfg = Release|Any CPU
        /// </code>
        /// </summary>
        /// <param name="range">Range with one configuration platform line.</param>
        public void AddConfigurationPlatform(Range range)
        {
            configurationPlatforms.Add(range);
        }

        /// <summary>
        /// Sets the parent of current project entry.
        /// </summary>
        /// <param name="parent">Parent entry.</param>
        /// <param name="nesting">Range in .sln file which defines the parent.</param>
        /// <exception cref="InvalidOperationException">
        /// If entry has a parent already or if parent is not solution folder.
        /// </exception>
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

        /// <summary>
        /// Gets full path for the current entry. Current entry is the first element, followed by optional
        /// parent project entries in reverse order. The last element corresponds to the component 
        /// at the root of the solution.
        /// </summary>
        /// <returns>Stack consisting of project entries on the path.</returns>
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
