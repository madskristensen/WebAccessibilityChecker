using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    internal sealed class EnableCommand
    {
        private readonly Package _package;

        private EnableCommand(Package package)
        {
            _package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var id = new CommandID(PackageGuids.guidPackageCmdSet, PackageIds.EnableAccessibilityId);
                var cmd = new OleMenuCommand(MenuItemCallback, id);
                cmd.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(cmd);
            }
        }
        
        public static EnableCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package)
        {
            Instance = new EnableCommand(package);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (MenuCommand)sender;
            button.Checked = VSPackage.Options.Enabled;
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            var button = (MenuCommand)sender;

            VSPackage.Options.Enabled = !button.Checked;
            VSPackage.Options.SaveSettingsToStorage();
            
            if (button.Checked)
            {
                TableDataSource.Instance.CleanAllErrors();
            }
        }
    }
}
