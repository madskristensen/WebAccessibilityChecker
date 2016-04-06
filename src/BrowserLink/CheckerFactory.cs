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

        static CheckerFactory()
        {
            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            _solutionEvents = dte.Events.SolutionEvents;
            _solutionEvents.AfterClosing += SolutionEvents_AfterClosing;
            _solutionEvents.ProjectRemoved += _solutionEvents_ProjectRemoved;
        }

        public BrowserLinkExtension CreateExtensionInstance(BrowserLinkConnection connection)
        {
            if (connection.Project == null)
                return null;

            return new CheckerExtension(connection.Project);
        }

        public string GetScript()
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream("WebAccessibilityChecker.BrowserLink.Checker.js"))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        static void _solutionEvents_ProjectRemoved(Project Project)
        {
            // Clear error list
        }

        static void SolutionEvents_AfterClosing()
        {
            // Clear error list
        }
    }
}
