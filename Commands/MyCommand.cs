using EnvDTE;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OrderProjectsInSlnFile.Classes;
using EnvDTE80;
using Microsoft.VisualStudio.PlatformUI;
using OrderProjectsInSlnFile.Forms;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace OrderProjectsInSlnFile
{
    [Command(PackageIds.MyCommand)]
    internal sealed class MyCommand : BaseCommand<MyCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var options = await General.GetLiveInstanceAsync();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            DTE dte = await Package.GetServiceAsync(typeof(DTE)) as DTE;

            OrderProjects(options, dte.Solution);
        }

        // Called every time menu with command is opened. Updates Enabled/Disabled state depending if a solution is opened or not.
        protected override void BeforeQueryStatus(EventArgs e)
        {
            base.BeforeQueryStatus(e);
            _ = UpdateCommandStateAsync();
        }

        private async Task UpdateCommandStateAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            DTE dte = await Package.GetServiceAsync(typeof(DTE)) as DTE;
            Command.Enabled = !string.IsNullOrEmpty(dte.Solution.FileName);
        }

        public void OrderProjects(General options, EnvDTE.Solution solution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var solutionFilePath = solution.FileName;

            if (string.IsNullOrEmpty(solutionFilePath) || !File.Exists(solutionFilePath))
            {
                System.Windows.Forms.MessageBox.Show($"Solution file '{solutionFilePath}' does not exist.", "File does not exist", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            try
            {
                ProjectLines projectLines = new ProjectLines();
                ProjectLines guidLines = new ProjectLines();

                var linesInFile = new List<string>();
                System.Text.Encoding encoding;
                using (var reader = new StreamReader(solutionFilePath))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        linesInFile.Add(line);
                    }
                    encoding = reader.CurrentEncoding;
                }

                string patternProject = @"^Project\(""\{[A-Z0-9-]+\}""\) = ""([^""]+)"",.+\{([A-Z0-9-]+)\}""$";
                string patternGuid = @"^\s+\{([A-Z0-9-]+).+";

                for (int i = 0; i < linesInFile.Count(); i++)
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

                using (var writer = new StreamWriter(solutionFilePath, false, encoding))
                {
                    foreach (var line in linesInFile)
                    {
                        writer.WriteLine(line);
                    }
                    writer.Flush();
                }
                // Reset IsDirty flag to avoid another "Save .sln file" message box.
                solution.IsDirty = false;

                if (!options.DoNotShowMesssageAnymore)
                {
                    MyMessageDialog dialogForm = new MyMessageDialog(Path.GetFileName(solutionFilePath));
                    DialogResult result = dialogForm.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        options.DoNotShowMesssageAnymore = dialogForm.DoNotShowMesssageAnymoreChecked;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "An error occurred", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private List<string> ChangeLines(ProjectLines projectLines, List<string> linesInFile)
        {
            Tuple<int, string> projectLinesDequeue = projectLines.Dequeue();
            int nums = 0;
            while (projectLinesDequeue != null)
            {
                for (int i = nums; i < linesInFile.Count; i++)
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
        private string GetSolutionPath()
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
