using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WebAccessibilityChecker
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideOptionPage(typeof(Options), "Web", "Accessibility Checker", 101, 111, true, new[] { "a11y", "wai", "section508", "wcag" }, ProvidesLocalizedCategoryName = false)]
    [Guid(PackageGuids.guidPackageString)]
    public sealed class VSPackage : Package
    {
        public VSPackage()
        {
            Options = (Options)GetDialogPage(typeof(Options));
        }

        public static Options Options { get; private set; }
        
        protected override void Initialize()
        {
            Logger.Initialize(this, Vsix.Name);
            Telemetry.Initialize(this, Vsix.Version, "1c740e68-eead-45bb-a583-0f1cf4c33100");

            EnableCommand.Initialize(this);
            ClearAllErrorsCommand.Initialize(this);
            OpenSettingsCommand.Initialize(this);
            SpecifyRulesCommand.Initialize(this);

            base.Initialize();
        }
    }
}
