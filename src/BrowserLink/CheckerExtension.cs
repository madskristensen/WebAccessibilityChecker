using System;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Web.BrowserLink;
using Newtonsoft.Json;

namespace WebAccessibilityChecker
{
    public class CheckerExtension : BrowserLinkExtension
    {
        private Project _project;
        
        public CheckerExtension(Project project)
        {
            _project = project;
        }

        public override void OnConnected(BrowserLinkConnection connection)
        {
            if (connection.Project == null)
                return;

            var dir = new DirectoryInfo(connection.Project.GetRootFolder());
            string folder = FindWorkingDirectory(dir);
            string file = Path.Combine(folder, Constants.ConfigFileName);
            string options = "{}";

            if (File.Exists(file))
            {
                options = File.ReadAllText(file);
            }

            Browsers.Client(connection).Invoke("initialize", options);

            base.OnConnected(connection);
        }

        protected virtual string FindWorkingDirectory(DirectoryInfo dir)
        {
            while (dir != null)
            {
                string rc = Path.Combine(dir.FullName, Constants.ConfigFileName);
                if (File.Exists(rc))
                    return dir.FullName;

                dir = dir.Parent;
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        [BrowserLinkCallback]
        public void ProcessResult(string jsonResult)
        {
            var result = JsonConvert.DeserializeObject<AccessibilityResult>(jsonResult);
            ErrorListService.ProcessLintingResults(result, true);
        }
    }
}
