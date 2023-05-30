global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using System.Reflection;
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

            solutionFilename = dte.Solution.FileName;

            var solutionEvents = dte.Events.SolutionEvents;
            solutionEvents.Opened += SolutionEvents_Opened;
            solutionEvents.BeforeClosing += SolutionEvents_BeforeClosing;
            solutionEvents.AfterClosing += SolutionEvents_AfterClosing;
        }

        private void SolutionEvents_Opened()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            solutionFilename = dte.Solution.FileName;

            dte.Solution.IsDirty = false;
            wasDirtyBeforeSave = false;
        }

        private async void SolutionEvents_AfterClosing()
        {
            if (wasDirtyBeforeSave && options.SortProjectsAfterClosingSolution)
            {
                var myCommand = new MyCommand();
                await myCommand.ExecutePublicAsync(new OleMenuCmdEventArgs(null, IntPtr.Zero));
            }

            solutionFilename = string.Empty;
        }

        private void SolutionEvents_BeforeClosing()
        {
            wasDirtyBeforeSave = dte.Solution.IsDirty;
        }

        private DTE dte;
        private General options;
        private string solutionFilename;
        private bool wasDirtyBeforeSave;

        public string SolutionFilename => solutionFilename;
    }
}