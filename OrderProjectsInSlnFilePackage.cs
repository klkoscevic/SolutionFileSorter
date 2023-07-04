global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using SortingLibrary;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

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
            if (sortSlnFile)
            {
                var myCommand = new MyCommand();
                myCommand.OrderProjects(options, solutionFullName);
            }
        }

        private void SolutionEvents_BeforeClosing()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            solutionFullName = dte.Solution.FullName;
            ProjectsSorter sorter;

            using (var reader = new StreamReader(dte.Solution.FullName))
            {
                var parser = new SolutionParser(reader);
                var projectEntries = parser.ProjectEntries;

                sorter = new ProjectsSorter();
                if (sorter.IsSorted(projectEntries))
                {
                    sortSlnFile = false;
                }
                else
                {
                    sortSlnFile = true;
                }
            }
        }

        private DTE dte;
        private General options;
        private bool sortSlnFile;
        private string solutionFullName;
    }
}