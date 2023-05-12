﻿using EnvDTE;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
                System.Windows.Forms.MessageBox.Show($"Solution file '{solutionFilePath}' does not exist.", "File does not exist", MessageBoxButtons.OK, MessageBoxIcon.Information)
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
                System.Windows.Forms.MessageBox.Show("Your projects in the .sln file are sorted alphabetically.", "Sorting is done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
