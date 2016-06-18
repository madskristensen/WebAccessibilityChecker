using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    internal sealed class RunNowCommand
    {
        private readonly Package _package;

        private RunNowCommand(Package package)
        {
            _package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var id = new CommandID(PackageGuids.guidPackageCmdSet, PackageIds.RunNow);
                var cmd = new OleMenuCommand(MenuItemCallback, id);
                cmd.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(cmd);
            }
        }

        public static RunNowCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package)
        {
            Instance = new RunNowCommand(package);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (MenuCommand)sender;
            button.Enabled = CheckerExtension.Instance.HasConnections;
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            TableDataSource.Instance.CleanAllErrors();
            CheckerExtension.Instance.CheckA11y();
        }
    }
}
