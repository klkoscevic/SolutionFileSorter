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
using Community.VisualStudio.Toolkit;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                OrderProjects(options);
            }

            dte.Solution.IsDirty = false;
            ((OrderProjectsInSlnFilePackage)Package).IsSorted = true;
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
        public void OrderProjects(General options, string solutionFilePath = null)
        {
            if (string.IsNullOrEmpty(solutionFilePath))
            {
                solutionFilePath = ((OrderProjectsInSlnFilePackage)Package).SolutionFilename;
            }

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
                        if (options.DoNotShowMesssageAnymore != dialogForm.DoNotShowMesssageAnymoreChecked)
                        {
                            options.DoNotShowMesssageAnymore = dialogForm.DoNotShowMesssageAnymoreChecked;
                            options.Save();
                        }
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
