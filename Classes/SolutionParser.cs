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
            fileContent = reader.ReadToEnd();

            CheckHeader();
            projects = ReadProjectLines();
            if (projects.IsEmpty())
            {
                return;
            }
            projectConfigurationPlatforms = ReadProjectConfigurationPlatforms();
        }

        public IEnumerable<ProjectEntry> ProjectEntries { get { return projectEntries; } }

        // These ranges will be used to replace contents sorted alphabetically.
        public readonly Range projects;
        public readonly Range projectConfigurationPlatforms = Range.Empty;

        // Solution file must start with "Microsoft Visual Studio Solution File, Format Version n.00". See e.g. https://stackoverflow.com/a/32753067 
        private void CheckHeader()
        {
            const string patternFileHeader = @"\s*Microsoft Visual Studio Solution File, Format Version (8|9|1\d)\.00";
            var match = Regex.Match(fileContent, patternFileHeader);
            if (!match.Success || match.Index != 0)
            {
                throw new FileFormatException(MessageInvalidFile);
            }
        }

        private Range ReadProjectLines()
        {
            const string patternGuid = @"\{[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}\}";
            const string patternProject = $"^Project\\(\\\"{patternGuid}\\\"\\) = \\\"([^\\\"]+)\\\", \\\"[^\\\"]+\\\", \\\"({patternGuid})\\\"";
            const string patternProjectEnd = @"^EndProject";

            var regexProject = new Regex(patternProject, RegexOptions.Multiline);
            var regexProjectEnd = new Regex(patternProjectEnd, RegexOptions.Multiline);

            var projectMatches = regexProject.Matches(fileContent);
            int projectEnd = 0;
            foreach (Match projectMatch in projectMatches)
            {
                if (projectMatch.Index <= projectEnd)
                {
                    throw new FileFormatException(MessageProjectEntriesOverlapping);
                }

                var projectEndMatch = regexProjectEnd.Match(fileContent, projectMatch.Index + projectMatch.Length);
                if (!projectEndMatch.Success)
                {
                    throw new FileFormatException(MessageProjectEndNotFound);
                }

                var projectName = projectMatch.Groups[1].Value;
                var guid = projectMatch.Groups[2].Value;
                var projectStart = projectMatch.Index;
                projectEnd = projectEndMatch.Index + projectEndMatch.Length;
                var projectEntry = new ProjectEntry(name: projectName, guid: guid, content: new Range(projectStart, projectEnd));
                projectEntries.Add(projectEntry);
            }
            return projectEntries.Any() ? new Range(projectEntries.First().Content.Start, projectEntries.Last().Content.End) : Range.Empty;
        }

        private Range ReadProjectConfigurationPlatforms()
        {
            const string projectConfigurationPattern = @"^\s+GlobalSection\(ProjectConfigurationPlatforms\) = postSolution";
            var projectConfigurationRegex = new Regex(projectConfigurationPattern, RegexOptions.Multiline);
            var projectConfigurationMatch = projectConfigurationRegex.Match(fileContent, projects.End);
            if (!projectConfigurationMatch.Success)
            {
                throw new FileFormatException(MessageConfiurationPlatformsNotFound);
            }
            var projectConfigurationEndMatch = endGlobalSectionRegex.Match(fileContent, projectConfigurationMatch.Index + projectConfigurationMatch.Length);
            if (!projectConfigurationEndMatch.Success)
            {
                throw new FileFormatException(MessageEndTagForConfigurationPlatformsNotFound);
            }
            var start = projectConfigurationMatch.Index + projectConfigurationMatch.Length;
            var end = projectConfigurationEndMatch.Index;
            // Limit regex searches up to the end of the section.
            var configurationPlatformSection = fileContent.Substring(0, end);
            foreach (var project in projectEntries)
            {
                var guid = project.Guid;
                var guidRegex = new Regex(@$"^\s+({guid})\..*$", RegexOptions.Multiline);
                foreach (Match match in guidRegex.Matches(configurationPlatformSection, start))
                {
                    project.AddConfigurationPlatform(new Range(match.Index, match.Index + match.Length));
                }
            }
            return new Range(start, end);
        }

        private readonly string fileContent;

        private readonly List<ProjectEntry> projectEntries = new List<ProjectEntry>();

        // This regex is used in several places so we make it a class member to avoid multiple initalization.
        const string endGlobalSectionPattern = @"^\s+EndGlobalSection";
        private readonly Regex endGlobalSectionRegex = new Regex(endGlobalSectionPattern, RegexOptions.Multiline);

        // These message strings are public to make them visible in tests.
        public const string MessageInvalidFile = "Not a valid Microsoft Visual Studio Solution File";
        public const string MessageProjectEntriesOverlapping = "Project entries are overlapping";
        public const string MessageProjectEndNotFound = "'ProjectEnd' tag not found";
        public const string MessageConfiurationPlatformsNotFound = "'GlobalSection(ProjectConfigurationPlatforms)' tag not found";
        public const string MessageEndTagForConfigurationPlatformsNotFound = "'EndGlobalSection' tag for GlobalSection(ProjectConfigurationPlatforms) not found";
    }
}