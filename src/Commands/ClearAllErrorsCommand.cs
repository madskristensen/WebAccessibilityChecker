using System;
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    internal sealed class ClearAllErrorsCommand
    {
        private readonly Package _package;
        private readonly BuildEvents _events;

        private ClearAllErrorsCommand(Package package, OleMenuCommandService commandService)
        {
            _package = package;

            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            _events = dte.Events.BuildEvents;
            _events.OnBuildBegin += OnBuildBegin;

            if (commandService != null)
            {
                var id = new CommandID(PackageGuids.guidPackageCmdSet, PackageIds.ClearAllErrors);
                var cmd = new OleMenuCommand(MenuItemCallback, id);
                cmd.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(cmd);
            }
        }

        public static ClearAllErrorsCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package, OleMenuCommandService commandService)
        {
            Instance = new ClearAllErrorsCommand(package, commandService);
        }

        private void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            if (Action == vsBuildAction.vsBuildActionClean || Action == vsBuildAction.vsBuildActionRebuildAll)
            {
                TableDataSource.Instance.CleanAllErrors();
            }
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            button.Enabled = TableDataSource.Instance.HasErrors;
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            TableDataSource.Instance.CleanAllErrors();
        }
    }
}
