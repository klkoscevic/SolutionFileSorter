using EnvDTE;
using OrderProjectsInSlnFile.Forms;
using System.IO;
using System.Windows.Forms;

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
                SolutionParser solutionFile = null;
                System.Text.Encoding encoding = null;
                using (var reader = new StreamReader(solutionFilePath))
                {
                    solutionFile = new SolutionParser(reader);
                    encoding = reader.CurrentEncoding;
                }

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
