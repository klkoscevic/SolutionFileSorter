global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
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

            Dte = (DTE)await GetServiceAsync(typeof(DTE)).ConfigureAwait(false);
            solutionFilename = Dte.Solution.FileName;

            var solutionEvents = Dte.Events.SolutionEvents;
            solutionEvents.Opened += SolutionEvents_Opened;
            solutionEvents.AfterClosing += SolutionEvents_AfterClosing;
        }

        private void SolutionEvents_Opened()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            solutionFilename = Dte.Solution.FileName;
        }

        private void SolutionEvents_AfterClosing()
        {
            // TODO: order projects here if not ordered already.

            solutionFilename = string.Empty;
        }

        private DTE Dte;
        private string solutionFilename;

        public string SolutionFilename => solutionFilename;
    }
}