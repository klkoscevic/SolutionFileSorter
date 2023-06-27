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
            string filename = dte.Solution.FullName;
            if (string.IsNullOrEmpty(filename) || dte.Solution.Projects.Count <= 1)
            {
                Command.Visible = false;
                return;
            }

            using (var reader = new StreamReader(filename))
            {
                var sorter = new SlnProjectsSorter(reader);
                Command.Visible = true;
                Command.Enabled = !sorter.AlreadySorted;
            }
        }

        public void OrderProjects(General options, EnvDTE.Solution solution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            SlnProjectsSorter sorter;
            using (var reader = new StreamReader(solution.FullName))
            {
                sorter = new SlnProjectsSorter(reader);
            }
            if (!sorter.AlreadySorted)
            {
                using (var writer = new StreamWriter(solution.FullName))
                {
                    sorter.WriteSorted(writer);
                }
            }
        }
    }
}
