using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    [Guid(PackageGuids.guidPackageString)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "Web", "Accessibility Checker", 101, 111, true, new[] { "a11y", "wai", "section508", "wcag" }, ProvidesLocalizedCategoryName = false)]
    [ProvideAutoLoad(ActivationContextGuid)]
    [ProvideUIContextRule(ActivationContextGuid, "Load Package",
        "WAP | WebSite | ProjectK | DotNetCoreWeb",
        new string[] {
            "WAP",
            "WebSite",
            "ProjectK",
            "DotNetCoreWeb"
        },
        new string[] {
            "SolutionHasProjectFlavor:{349C5851-65DF-11DA-9384-00065B846F21}",
            "SolutionHasProjectFlavor:{E24C65DC-7377-472B-9ABA-BC803B73C61A}",
            "SolutionHasProjectFlavor:{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}",
            "SolutionHasProjectCapability:DotNetCoreWeb"
        })]
    public sealed class VSPackage : AsyncPackage
    {
        private const string ActivationContextGuid = "{d3114de0-d3b7-451e-b670-9a1740424cba}";

        public static Options Options
        {
            get;
            private set;
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Logger.Initialize(Vsix.Name);

            Options = (Options)GetDialogPage(typeof(Options));

            var commandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (commandService != null)
            {
                ClearAllErrorsCommand.Initialize(this, commandService);
                ToggleAutoRunCommand.Initialize(this, commandService);
                OpenSettingsCommand.Initialize(this, commandService);
                RunNowCommand.Initialize(this, commandService);
                SpecifyRulesCommand.Initialize(this, commandService);
            }
        }
    }
}
