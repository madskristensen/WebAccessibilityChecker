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

        private ClearAllErrorsCommand(Package package)
        {
            _package = package;

            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            _events = dte.Events.BuildEvents;
            _events.OnBuildBegin += OnBuildBegin;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var id = new CommandID(PackageGuids.guidPackageCmdSet, PackageIds.ClearAllErrors);
                var cmd = new MenuCommand(MenuItemCallback, id);
                commandService.AddCommand(cmd);
            }
        }

        public static ClearAllErrorsCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package)
        {
            Instance = new ClearAllErrorsCommand(package);
        }

        private void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            if (Action == vsBuildAction.vsBuildActionClean || Action == vsBuildAction.vsBuildActionRebuildAll)
            {
                TableDataSource.Instance.CleanAllErrors();
            }
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            TableDataSource.Instance.CleanAllErrors();
            Telemetry.TrackEvent("Clear all errors");
        }
    }
}
