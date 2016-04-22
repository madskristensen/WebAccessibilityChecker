using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "Web", "Accessibility Checker", 101, 111, true, new[] { "a11y", "wai", "section508", "wcag" }, ProvidesLocalizedCategoryName = false)]
    [Guid(PackageGuids.guidPackageString)]
    [ProvideAutoLoad("{349C5852-65DF-11dA-9384-00065B846F21}")] // WAP
    [ProvideAutoLoad("{E24C65DC-7377-472b-9ABA-BC803B73C61A}")] // WebSite
    [ProvideAutoLoad("{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}")] // ProjectK
    public sealed class VSPackage : Package
    {
        public static Options Options { get; private set; }

        protected override void Initialize()
        {
            Logger.Initialize(this, Vsix.Name);
            Telemetry.Initialize(this, Vsix.Version, "1c740e68-eead-45bb-a583-0f1cf4c33100");

            Options = (Options)GetDialogPage(typeof(Options));

            ClearAllErrorsCommand.Initialize(this);
            ToggleAutoRunCommand.Initialize(this);
            OpenSettingsCommand.Initialize(this);
            RunNowCommand.Initialize(this);
            SpecifyRulesCommand.Initialize(this);

            base.Initialize();
        }
    }
}
