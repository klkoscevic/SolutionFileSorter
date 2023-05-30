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

            if (System.Windows.MessageBox.Show("Are you sure you want to sort projects in current solution file?",
                                               "Sorting .sln file",
                                               System.Windows.MessageBoxButton.YesNo,
                                               System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
            {
                OrderProjects(options, dte.Solution);
            }
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
                SolutionFile solutionFile = null;
                System.Text.Encoding encoding = null;
                using (var reader = new StreamReader(solutionFilePath))
                {
                    solutionFile = new SolutionFile(reader);
                    encoding = reader.CurrentEncoding;
                }

                var linesInFile = solutionFile.Sort();

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
    }
}
