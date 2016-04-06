using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidPackageString)]
    public sealed class VSPackage : Package
    {
        protected override void Initialize()
        {
            Logger.Initialize(this, Vsix.Name);
            Telemetry.Initialize(this, Vsix.Version, "1c740e68-eead-45bb-a583-0f1cf4c33100");

            EnableCommand.Initialize(this);

            base.Initialize();
        }
    }
}
