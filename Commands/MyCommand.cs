using EnvDTE;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using EnvDTE80;
using Microsoft.VisualStudio.PlatformUI;
using OrderProjectsInSlnFile.Forms;
using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Shell;

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
                OrderProjects(options, dte.Solution.FullName);
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
            DTE dte = await Package.GetServiceAsync(typeof(DTE)) as DTE; string filename = dte.Solution.FullName;
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

            if (!options.DoNotShowMesssageAnymore)
            {
                MyMessageDialog dialogForm = new MyMessageDialog(Path.GetFileName(solutionFullName));
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
