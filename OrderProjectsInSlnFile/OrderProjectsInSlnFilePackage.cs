global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using OrderProjectsInSlnFile.Forms;
using SortingLibrary;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace OrderProjectsInSlnFile
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
        private MyCommand myCommand = new MyCommand();

    }
}