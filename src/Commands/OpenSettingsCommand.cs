using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    internal sealed class OpenSettingsCommand
    {
        private readonly Package _package;

        private OpenSettingsCommand(Package package, OleMenuCommandService commandService)
        {
            _package = package;

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

        public static void Initialize(Package package, OleMenuCommandService commandService)
        {
            Instance = new OpenSettingsCommand(package, commandService);
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            _package.ShowOptionPage(typeof(Options));
        }
    }
}
