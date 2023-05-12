using EnvDTE;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OrderProjectsInSlnFile
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        class ProjectLine
        {
            public string Line { get; set; }
            public string Name { get; set; }
        }
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            OrderProjects();
        }

        public void OrderProjects()
        {
            string solutionFilePath = GetSolutionPath();

            if (string.IsNullOrEmpty(solutionFilePath) || !File.Exists(solutionFilePath))
            {
                Debug.WriteLine($"Solution file '{solutionFilePath}' does not exist.");
                return;
            }

            try
            {
                List<ProjectLine> projectLines = new List<ProjectLine>();
                List<int> lineNumber = new List<int>();
                string[] linesInFile = File.ReadAllLines(solutionFilePath);
                string patternProject = @"^Project\(""\{[A-Z0-9-]+\}""\) = ""([^""]+)"",.+(}"")$";

                for (int i = 0; i < linesInFile.Length; i++)
                {
                    var matchesProject = Regex.Matches(linesInFile[i], patternProject);
                    if (matchesProject.Count > 0)
                    {
                        lineNumber.Add(i);
                        foreach (Match match in matchesProject)
                        {
                            string line = match.Value;
                            string projectName = match.Groups[1].Value;
                            projectLines.Add(new ProjectLine { Line = line, Name = projectName });
                        }
                    }
                }

                projectLines = projectLines.OrderBy(p => p.Name).ToList();

                for (int i = 0; i < linesInFile.Length; i++)
                {
                    if (i == lineNumber[0])
                    {
                        linesInFile[i] = projectLines[0].Line;
                        projectLines.RemoveAt(0);
                        lineNumber.RemoveAt(0);

                        if (lineNumber.Count == 0)
                        {
                            break;
                        }
                    }
                }

                File.WriteAllLines(solutionFilePath, linesInFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static string GetSolutionPath()
        {
            string currentDirectory = Environment.CurrentDirectory;
            string solutionPath = null;

            while (currentDirectory != null)
            {
                string[] solutionFiles = System.IO.Directory.GetFiles(currentDirectory, "*.sln");
                if (solutionFiles.Length > 0)
                {
                    solutionPath = solutionFiles[0];
                    break;
                }

                currentDirectory = System.IO.Directory.GetParent(currentDirectory)?.FullName;
            }

            return solutionPath;
        }
    }
}
