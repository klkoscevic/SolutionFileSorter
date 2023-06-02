using OrderProjectsInSlnFile.Classes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OrderProjectsInSlnFile
{
    public class SolutionFile
    {
        public SolutionFile(TextReader reader)
        {
            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
                linesInFile.Add(line);
            }
        }

        public IEnumerable<string> Sort()
        {
            var projectLines = new ProjectLines();
            var guidLines = new ProjectLines();

            string patternProject = @"^Project\(""\{[A-Z0-9-]+\}""\) = ""([^""]+)"",.+\{([A-Z0-9-]+)\}""$";
            string patternGuid = @"^\s+\{([A-Z0-9-]+).+";

            for (int i = 0; i < linesInFile.Count; i++)
            {
                var matchesProject = Regex.Matches(linesInFile[i], patternProject);
                var matchesGuid = Regex.Matches(linesInFile[i], patternGuid);

                if (matchesProject.Count > 0)
                {
                    foreach (Match match in matchesProject)
                    {
                        string line = match.Value;
                        string projectName = match.Groups[1].Value;
                        string guid = match.Groups[2].Value;

                        ProjectLine projectLine = new ProjectLine { Line = line, Name = projectName, GUID = guid };

                        projectLines.Enqueue(i, projectLine);
                    }
                }
                else if (matchesGuid.Count > 0)
                {
                    foreach (Match match in matchesGuid)
                    {
                        string line = match.Value;
                        string guid = match.Groups[1].Value;

                        ProjectLine projectLine = new ProjectLine { Line = line, GUID = guid };

                        guidLines.Enqueue(i, projectLine);
                    }
                }
            }

            projectLines.Sort();

            guidLines.AddProjectName(projectLines);
            guidLines.Sort();

            linesInFile = ChangeLines(projectLines, linesInFile);
            linesInFile = ChangeLines(guidLines, linesInFile);

            return linesInFile;
        }

        private List<string> ChangeLines(ProjectLines projectLines, List<string> linesInFile)
        {
            Tuple<int, string> projectLinesDequeue = projectLines.Dequeue();
            int nums = 0;
            while (projectLinesDequeue != null)
            {
                for (int i = nums; i < linesInFile.Count(); i++)
                {
                    nums++;

                    if (i == projectLinesDequeue.Item1)
                    {
                        linesInFile[i] = projectLinesDequeue.Item2;
                        break;
                    }
                }

                projectLinesDequeue = projectLines.Dequeue();
            }

            return linesInFile;
        }

        private List<string> linesInFile = new List<string>();
    }
}