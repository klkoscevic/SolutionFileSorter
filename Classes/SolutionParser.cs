using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OrderProjectsInSlnFile
{
    public class SolutionParser
    {
        public SolutionParser(TextReader reader)
        {
            // Read entire content into one string instead of reading line by line (which is slower). 
            FileContent = reader.ReadToEnd();

            CheckHeader();
            Projects = ReadProjectLines();
            if (Projects.IsEmpty())
            {
                return;
            }
            ProjectConfigurationPlatforms = ReadProjectConfigurationPlatforms();
            ProjectNestings = ReadProjectNestings();
        }

        public readonly string FileContent;

        public IEnumerable<ProjectEntry> ProjectEntries { get { return projectEntries; } }

        // These ranges will be used to replace contents sorted alphabetically.
        public readonly Range Projects;
        public readonly Range ProjectConfigurationPlatforms = Range.Empty;
        public readonly Range ProjectNestings = Range.Empty;

        // Solution file must start with "Microsoft Visual Studio Solution File, Format Version n.00". See e.g. https://stackoverflow.com/a/32753067 
        private void CheckHeader()
        {
            const string patternFileHeader = @"\s*Microsoft Visual Studio Solution File, Format Version (8|9|1\d)\.00";
            var match = Regex.Match(FileContent, patternFileHeader);
            if (!match.Success || match.Index != 0)
            {
                throw new FileFormatException(MessageInvalidFile);
            }
        }

        private Range ReadProjectLines()
        {
            const string patternProject = $"^Project\\(\\\"({patternGuid})\\\"\\)\\s*=\\s*\\\"([^\\\"]+)\\\"\\s*,\\s*\\\"[^\\\"]+\\\"\\s*,\\s*\\\"({patternGuid})\\\"\\s*[\\r?\\n]";
            const string patternProjectEnd = @"^\s*EndProject\s*[\r?\n]";

            var regexProject = new Regex(patternProject, RegexOptions.Multiline);
            var regexProjectEnd = new Regex(patternProjectEnd, RegexOptions.Multiline);

            var projectMatches = regexProject.Matches(FileContent);
            int projectEnd = 0;
            foreach (Match projectMatch in projectMatches)
            {
                if (projectMatch.Index < projectEnd)
                {
                    throw new FileFormatException(MessageProjectEntriesOverlapping);
                }

                var projectEndMatch = regexProjectEnd.Match(FileContent, projectMatch.Index + projectMatch.Length);
                if (!projectEndMatch.Success)
                {
                    throw new FileFormatException(MessageProjectEndNotFound);
                }

                var typeGuid = projectMatch.Groups[1].Value;
                var isSolutionFolder = typeGuid == SolutionFolderGuid;
                var projectName = projectMatch.Groups[2].Value;
                var guid = projectMatch.Groups[3].Value;
                var projectStart = projectMatch.Index;
                projectEnd = projectEndMatch.Index + projectEndMatch.Length;
                var projectEntry = new ProjectEntry(projectName, guid: guid, isSolutionFolder: isSolutionFolder, content: new Range(projectStart, projectEnd));
                projectEntries.Add(projectEntry);
            }
            return projectEntries.Any() ? new Range(projectEntries.First().Content.Start, projectEntries.Last().Content.End) : Range.Empty;
        }

        private Range ReadProjectConfigurationPlatforms()
        {
            const string projectConfigurationPattern = @"^\s+GlobalSection\(ProjectConfigurationPlatforms\) = postSolution\s*[\r?\n]";
            var projectConfigurationRegex = new Regex(projectConfigurationPattern, RegexOptions.Multiline);
            var projectConfigurationMatch = projectConfigurationRegex.Match(FileContent, Projects.End);
            if (!projectConfigurationMatch.Success)
            {
                throw new FileFormatException(MessageConfigurationPlatformsNotFound);
            }

            var start = projectConfigurationMatch.Index + projectConfigurationMatch.Length;
            var projectConfigurationEndMatch = endGlobalSectionRegex.Match(FileContent, start);
            
            if (!projectConfigurationEndMatch.Success)
            {
                throw new FileFormatException(MessageEndTagForConfigurationPlatformsNotFound);
            }
            var end = projectConfigurationEndMatch.Index;
            // Limit regex searches up to the end of the section.
            var configurationPlatformSection = FileContent.Substring(0, end);
            foreach (var project in projectEntries)
            {
                var guid = project.Guid;
                var guidRegex = new Regex(@$"^\s+({guid})\..*[\r?\n]", RegexOptions.Multiline);
                foreach (Match match in guidRegex.Matches(configurationPlatformSection, start))
                {
                    project.AddConfigurationPlatform(new Range(match.Index, match.Index + match.Length));
                }
            }
            return new Range(start, end);
        }

        private Range ReadProjectNestings()
        {
            const string projectNestingPattern = @"^\s+GlobalSection\(NestedProjects\) = preSolution\s*[\r?\n]";
            var projectNestingRegex = new Regex(projectNestingPattern, RegexOptions.Multiline);
            var projectNestingMatch = projectNestingRegex.Match(FileContent, ProjectConfigurationPlatforms.End);
            if (!projectNestingMatch.Success)
            {
                return Range.Empty;
            }
            var start = projectNestingMatch.Index + projectNestingMatch.Length;
            var projectConfigurationEndMatch = endGlobalSectionRegex.Match(FileContent, start);
            if (!projectConfigurationEndMatch.Success)
            {
                throw new FileFormatException(MessageEndTagForNestedProjectsNotFound);
            }
            var end = projectConfigurationEndMatch.Index;
            // Limit regex searches up to the end of the section.
            var nestedProjectSection = FileContent.Substring(0, end);
            const string nestingEntryPattern = @$"^\s+({patternGuid}) = ({patternGuid})\s*[\r?\n]";
            var nestingRegex = new Regex(nestingEntryPattern, RegexOptions.Multiline);
            var nestingMatches = nestingRegex.Matches(nestedProjectSection, start);
            foreach (Match match in nestingMatches)
            {
                var childGuid = match.Groups[1].Value;
                var child = FindProjectEntryByGuid(childGuid);
                var parentGuid = match.Groups[2].Value;
                var parent = FindProjectEntryByGuid(parentGuid);
                child.SetParent(parent, new Range(match.Index, match.Index + match.Length));
            }
            return new Range(start, end);
        }

        private ProjectEntry FindProjectEntryByGuid(string guid)
        {
            var found = projectEntries.FirstOrDefault(pe => pe.Guid == guid);
            if (found == null)
            {
                throw new ArgumentException($"Entry with GUID '{guid}' not found");
            }
            return found;
        }

        private readonly List<ProjectEntry> projectEntries = new List<ProjectEntry>();

        // These patterns and regex are used in several places so we make it a class member to avoid multiple initalization.
        const string patternGuid = @"\{[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}\}";

        // This regex is used in several places so we make it a class member to avoid multiple initalization.
        const string endGlobalSectionPattern = @"^\s+EndGlobalSection";
        private readonly Regex endGlobalSectionRegex = new Regex(endGlobalSectionPattern, RegexOptions.Multiline);

        private const string SolutionFolderGuid = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

        private const string MessageInvalidFile = "Not a valid Microsoft Visual Studio Solution File";
        private const string MessageProjectEntriesOverlapping = "Project entries are overlapping";
        private const string MessageProjectEndNotFound = "'ProjectEnd' tag not found";
        private const string MessageConfigurationPlatformsNotFound = "'GlobalSection(ProjectConfigurationPlatforms)' tag not found";
        private const string MessageEndTagForConfigurationPlatformsNotFound = "'EndGlobalSection' tag for 'GlobalSection(ProjectConfigurationPlatforms)' not found";
        private const string MessageEndTagForNestedProjectsNotFound = "'EndGlobalSection' tag for 'GlobalSection(NestedProjects)' not found";
    }
}