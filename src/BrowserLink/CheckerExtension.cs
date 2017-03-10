using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.Web.BrowserLink;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebAccessibilityChecker
{
    public class CheckerExtension : BrowserLinkExtension
    {
        private List<BrowserLinkConnection> _connections = new List<BrowserLinkConnection>();

        private CheckerExtension()
        { }

        public static CheckerExtension Instance { get; } = new CheckerExtension();

        public bool HasConnections =>
            _connections.Count > 0;


        public override void OnConnected(BrowserLinkConnection connection)
        {
            if (connection.Project == null)
                return;

            if (!_connections.Contains(connection))
                _connections.Add(connection);

            Browsers.Client(connection).Invoke("initialize", GetOptions(connection), connection.Project.Name);

            if (VSPackage.Options.RunOnPageLoad)
                CheckA11y(connection);

            base.OnConnected(connection);
        }

        public void CheckA11y(params BrowserLinkConnection[] connections)
        {
            connections = (connections == null || connections.Length == 0) ? _connections.ToArray() : connections;

            if (connections == null)
                return;

            foreach (var connection in connections)
            {
                Browsers.Client(connection).Invoke("check");
            }
        }

        private string GetOptions(BrowserLinkConnection connection)
        {
            var dir = new DirectoryInfo(connection.Project.GetRootFolder());
            string folder = FindConfigFolder(dir);
            string file = Path.Combine(folder, Constants.ConfigFileName);
            string options = "{}";

            if (File.Exists(file))
            {
                var content = File.ReadAllText(file);
                var obj = JObject.Parse(content, new JsonLoadSettings { CommentHandling = CommentHandling.Ignore });
                options = obj.ToString();
            }

            return options;
        }

        public override void OnDisconnecting(BrowserLinkConnection connection)
        {
            if (_connections.Contains(connection))
                _connections.Remove(connection);

            base.OnDisconnecting(connection);
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
                ErrorListService.ProcessLintingResults(result);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }
    }
}
