global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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

            solutionFilename = dte.Solution.FileName;
            sortSlnFile = false;

            var solutionEvents = dte.Events.SolutionEvents;
            solutionEvents.Opened += SolutionEvents_Opened;
            solutionEvents.BeforeClosing += SolutionEvents_BeforeClosing;
            solutionEvents.AfterClosing += SolutionEvents_AfterClosing;
        }

        private void SolutionEvents_Opened()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            solutionFilename = dte.Solution.FileName;

            isSorted = IsSortedSolution();

            dte.Solution.IsDirty = false;
        }

        private void SolutionEvents_AfterClosing()
        {
            if (sortSlnFile)
            {
                var myCommand = new MyCommand();
                myCommand.OrderProjects(options, solutionFilename);
            }

            solutionFilename = string.Empty;
        }

        private void SolutionEvents_BeforeClosing()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            wasDirtyBeforeSave = dte.Solution.IsDirty;
            isSorted = isSorted ? IsSortedSolution() : isSorted;

            if (options.SortAlwaysWithoutAsking && (!isSorted || wasDirtyBeforeSave))
            {
                sortSlnFile = true;
            }
            else if (!isSorted || wasDirtyBeforeSave)
            {
                sortSlnFile = IsSortedSolution();
            }
        }

        private bool IsSortedSolution()
        {
            SolutionParser solutionParser = null;
            using (var reader = new StreamReader(solutionFilename))
            {
                solutionParser = new SolutionParser(reader);
            }

           var projectsSorter = new ProjectsSorter();
            return projectsSorter.IsSorted(solutionParser.ProjectEntries);

        }


    private DTE dte;
        private General options;
        private string solutionFilename;
        private bool wasDirtyBeforeSave = false;
        private bool isSorted;
        private bool sortSlnFile;
        private OrderProjectsInSlnFilePackage orderProjectsPackage;


        public string SolutionFilename => solutionFilename;
        public bool IsSorted
        {
            get { return isSorted; }
            set { isSorted = value; }
        }
    }
}