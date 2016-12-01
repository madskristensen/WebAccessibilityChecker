using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    internal sealed class ToggleAutoRunCommand
    {
        private readonly Package _package;

        private ToggleAutoRunCommand(Package package, OleMenuCommandService commandService)
        {
            _package = package;

            if (commandService != null)
            {
                var id = new CommandID(PackageGuids.guidPackageCmdSet, PackageIds.EnableAccessibilityId);
                var cmd = new OleMenuCommand(MenuItemCallback, id);
                cmd.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(cmd);
            }
        }

        public static ToggleAutoRunCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package, OleMenuCommandService commandService)
        {
            Instance = new ToggleAutoRunCommand(package, commandService);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (MenuCommand)sender;
            button.Checked = VSPackage.Options.RunOnPageLoad;
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            var button = (MenuCommand)sender;

            VSPackage.Options.RunOnPageLoad = !button.Checked;
            VSPackage.Options.SaveSettingsToStorage();

            if (!VSPackage.Options.RunOnPageLoad)
            {
                TableDataSource.Instance.CleanAllErrors();
            }
        }
    }
}
