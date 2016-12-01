using System;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace WebAccessibilityChecker
{
    internal sealed class SpecifyRulesCommand
    {
        private readonly Package _package;
        private readonly DTE2 _dte;

        private SpecifyRulesCommand(Package package, OleMenuCommandService commandService)
        {
            _package = package;
            _dte = (DTE2)Package.GetGlobalService(typeof(DTE));

            if (commandService != null)
            {
                var id = new CommandID(PackageGuids.guidPackageCmdSet, PackageIds.SpecifyRules);
                var cmd = new OleMenuCommand(MenuItemCallback, id);
                cmd.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(cmd);
            }
        }

        public static SpecifyRulesCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package, OleMenuCommandService commandService)
        {
            Instance = new SpecifyRulesCommand(package, commandService);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            button.Enabled = false;

            if (_dte.Solution == null || string.IsNullOrEmpty(_dte.Solution.FullName))
                return;

            string solDir = Path.GetDirectoryName(_dte.Solution.FullName);
            string destFile = Path.Combine(solDir, Constants.ConfigFileName);

            button.Enabled = !File.Exists(destFile);
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            string solDir = Path.GetDirectoryName(_dte.Solution.FullName);
            string destFile = Path.Combine(solDir, Constants.ConfigFileName);

            string sourceFile = GetFileName("json\\schema\\" + Constants.ConfigFileName);
            File.Copy(sourceFile, destFile, true);

            AddFileToSolutionFolder(destFile, (Solution2)_dte.Solution);
            _dte.ItemOperations.OpenFile(destFile);

            var command = _dte.Commands.Item("SolutionExplorer.SyncWithActiveDocument");
            if (command.IsAvailable)
            {
                _dte.ExecuteCommand(command.Name);
            }
        }

        public static string GetFileName(string relativePath)
        {
            string assembly = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(assembly);
            return Path.Combine(folder, relativePath);
        }

        public static void AddFileToSolutionFolder(string file, Solution2 solution)
        {
            Project currentProject = null;

            foreach (Project project in solution.Projects)
            {
                if (project.Kind == EnvDTE.Constants.vsProjectKindSolutionItems && project.Name == "Solution Items")
                {
                    currentProject = project;
                    break;
                }
            }

            if (currentProject == null)
                currentProject = solution.AddSolutionFolder("Solution Items");

            currentProject.ProjectItems.AddFromFile(file);
        }
    }
}
