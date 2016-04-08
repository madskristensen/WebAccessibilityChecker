using System;
using System.IO;
using System.Reflection;
using EnvDTE;
using Microsoft.VisualStudio.Web.BrowserLink;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            string folder = FindConfigFolder(dir);
            string file = Path.Combine(folder, Constants.ConfigFileName);
            string options = "{}";

            if (File.Exists(file))
            {
                file = File.ReadAllText(file);
                var obj = JObject.Parse(file, new JsonLoadSettings { CommentHandling = CommentHandling.Ignore });
                options = obj.ToString();
            }

            Browsers.Client(connection).Invoke("initialize", options);

            base.OnConnected(connection);
        }

        protected virtual string FindConfigFolder(DirectoryInfo dir)
        {
            while (dir != null)
            {
                string rc = Path.Combine(dir.FullName, Constants.ConfigFileName);
                if (File.Exists(rc))
                    return dir.FullName;

                dir = dir.Parent;
            }

            var bin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(bin, "JSON\\Schema");
        }

        [BrowserLinkCallback]
        public void ProcessResult(string jsonResult)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<AccessibilityResult>(jsonResult);
                ErrorListService.ProcessLintingResults(result, true);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
