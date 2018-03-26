using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Web.BrowserLink;

namespace WebAccessibilityChecker
{
    [Export(typeof(IBrowserLinkExtensionFactory))]
    public class CheckerFactory : IBrowserLinkExtensionFactory
    {
        static SolutionEvents _solutionEvents;
        static string _script;

        static CheckerFactory()
        {
            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            _solutionEvents = dte.Events.SolutionEvents;
            _solutionEvents.AfterClosing += SolutionEvents_AfterClosing;
            _solutionEvents.ProjectRemoved += _solutionEvents_ProjectRemoved;

            string checkerJs = GetScriptFromAssembly("WebAccessibilityChecker.BrowserLink.Checker.js");
            string axeJs = GetScriptFromAssembly("WebAccessibilityChecker.BrowserLink.Axe.min.js");

            _script = checkerJs.Replace("//[axe.min.js]", axeJs);
        }

        public BrowserLinkExtension CreateExtensionInstance(BrowserLinkConnection connection)
        {
            if (connection.Project == null || !VSPackage.Options.Enabled)
                return null;

            return CheckerExtension.Instance;
        }

        public string GetScript()
        {
            return _script;
        }

        private static string GetScriptFromAssembly(string path)
        {
            using (Stream stream = typeof(CheckerFactory).Assembly.GetManifestResourceStream(path))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        static void _solutionEvents_ProjectRemoved(Project Project)
        {
            TableDataSource.Instance.CleanAllErrors();
        }

        static void SolutionEvents_AfterClosing()
        {
            TableDataSource.Instance.CleanAllErrors();
        }
    }
}
