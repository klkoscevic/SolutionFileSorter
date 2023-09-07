/*
MIT License

Copyright(c) 2023 Klara Koščević

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using EnvDTE;
using KKoščević.SolutionFileSorter.VSExtension.Forms;
using KKoščević.SolutionFileSorter.Shared;
using System.IO;
using System.Windows.Forms;

namespace KKoščević.SolutionFileSorter.VSExtension
{
    [Command(PackageIds.Command)]
    internal sealed class Command : BaseCommand<Command>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var options = await General.GetLiveInstanceAsync();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            DTE dte = await Package.GetServiceAsync(typeof(DTE)) as DTE;

            if (dte.Solution.Saved)
            {
                if (System.Windows.MessageBox.Show($"Sort .sln file\n\nAre you sure you want to sort projects in '{Path.GetFileName(dte.Solution.FileName)}' solution file?",
                                                    "Microsoft Visual Studio",
                                                    System.Windows.MessageBoxButton.YesNo,
                                                    System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
                {
                    OrderProjects(options, dte.Solution.FullName);

                }
            }
            else
            {
                if (System.Windows.MessageBox.Show($"Sort .sln file\n\nSolution '{Path.GetFileName(dte.Solution.FileName)}' is not saved. Do you want to save the solution and sort solution file?",
                                                    "Microsoft Visual Studio",
                                                    System.Windows.MessageBoxButton.YesNo,
                                                    System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
                {
                    dte.Solution.SaveAs(dte.Solution.FullName);
                    OrderProjects(options, dte.Solution.FullName);
                }
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
                var parser = new SolutionParser(reader);
                var sorter = new ProjectsSorter();

                var projectEntries = parser.ProjectEntries;

                Command.Visible = true;

                Command.Enabled = !sorter.IsSorted(projectEntries);
            }
        }

        public void OrderProjects(General options, string solutionFullName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            SlnProjectsSorter sorter;
            try
            {
                using (var reader = new StreamReader(solutionFullName))
                {
                    sorter = new SlnProjectsSorter(reader);
                }

                if (!sorter.AlreadySorted)
                {
                    using (var writer = new StreamWriter(solutionFullName))
                    {
                        sorter.WriteSorted(writer);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            if (!options.DoNotShowMesssageAnymore)
            {
                SortConfirmationDialog dialogForm = new SortConfirmationDialog(Path.GetFileName(solutionFullName));
                DialogResult result = dialogForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (options.DoNotShowMesssageAnymore != dialogForm.DoNotShowMesssageAnymoreChecked)
                    {
                        options.DoNotShowMesssageAnymore = dialogForm.DoNotShowMesssageAnymoreChecked;
                        options.Save();
                    }
                }
            }
        }
    }
}
