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

global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using KKoščević.SolutionFileSorter.VSExtension.Forms;
using KKoščević.SolutionFileSorter.Shared;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace KKoščević.SolutionFileSorter.VSExtension
{
    [ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.OrderProjectsInSlnFileString)]
    [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "Sort projects in .sln file", "General", 0, 0, true, SupportsProfiles = true)]

    public sealed class OrderProjectsInSlnFilePackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await this.RegisterCommandsAsync();

            dte = (DTE)await GetServiceAsync(typeof(DTE)).ConfigureAwait(false);
            options = await General.GetLiveInstanceAsync();

            var solutionEvents = dte.Events.SolutionEvents;
            solutionEvents.BeforeClosing += SolutionEvents_BeforeClosing;
            solutionEvents.AfterClosing += SolutionEvents_AfterClosing;
        }

        private void SolutionEvents_AfterClosing()
        {
            if (solutionNotSaved)
            {
                CheckIfSlnFileShouldSort();
            }
            if (sortSlnFile)
            {
                myCommand.OrderProjects(options, solutionFullName);
            }
        }

        private void SolutionEvents_BeforeClosing()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            solutionFullName = dte.Solution.FullName;

            if (!options.NeverSortAfterClosingSolution && dte.Solution.Saved)
            {
                CheckIfSlnFileShouldSort();
            }
            else if (!dte.Solution.Saved)
            {
                solutionNotSaved = true;
            }
        }

        private void CheckIfSlnFileShouldSort()
        {
            ProjectsSorter sorter;

            using (var reader = new StreamReader(solutionFullName))
            {
                var parser = new SolutionParser(reader);
                var projectEntries = parser.ProjectEntries;

                sorter = new ProjectsSorter();
                if (!sorter.IsSorted(projectEntries))
                {
                    if (System.Windows.MessageBox.Show($"Sort .sln file\n\nAre you sure you want to sort projects in current '{Path.GetFileName(dte.Solution.FileName)}' solution file?",
                                            "Microsoft Visual Studio",
                                            System.Windows.MessageBoxButton.YesNo,
                                            System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
                    {
                        sortSlnFile = true;
                    }
                }
            }
        }

        private DTE dte;
        private General options;
        private bool sortSlnFile = false;
        private string solutionFullName;
        private bool solutionNotSaved = false;
        private Command myCommand = new Command();

    }
}