using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;

namespace WebAccessibilityChecker
{
    class ErrorListService
    {
        public static void ProcessLintingResults(AccessibilityResult result, bool showErrorList)
        {
            TableDataSource.Instance.CleanAllErrors();
            IEnumerable<Rule> rules = result.Violations;

            if (!VSPackage.Options.ShowWarnings)
            {
                rules = rules.Where(r => r.GetSeverity() != __VSERRORCATEGORY.EC_WARNING);
            }

            if (!VSPackage.Options.ShowMessages)
            {
                rules = rules.Where(r => r.GetSeverity() != __VSERRORCATEGORY.EC_MESSAGE);
            }

            if (rules.Any())
            {
                TableDataSource.Instance.AddErrors(rules);
            }
        }
    }
}
