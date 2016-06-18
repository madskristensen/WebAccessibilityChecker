using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    internal sealed class OpenSettingsCommand
    {
        private readonly Package _package;

        private OpenSettingsCommand(Package package)
        {
            _package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var id = new CommandID(PackageGuids.guidPackageCmdSet, PackageIds.OpenSettings);
                var cmd = new OleMenuCommand(MenuItemCallback, id);
                commandService.AddCommand(cmd);
            }
        }

        public static OpenSettingsCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package)
        {
            Instance = new OpenSettingsCommand(package);
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            _package.ShowOptionPage(typeof(Options));
        }
    }
}
