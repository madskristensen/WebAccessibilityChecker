using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;

namespace WebAccessibilityChecker
{
    class ErrorListService
    {
        public static void ProcessLintingResults(AccessibilityResult result)
        {
            TableDataSource.Instance.CleanErrors(result.Url);

            if (!VSPackage.Options.ShowWarnings)
            {
               result.Violations = result.Violations.Where(r => r.GetSeverity() != __VSERRORCATEGORY.EC_WARNING);
            }

            if (!VSPackage.Options.ShowMessages)
            {
                result.Violations = result.Violations.Where(r => r.GetSeverity() != __VSERRORCATEGORY.EC_MESSAGE);
            }

            if (result.Violations.Any())
            {
                TableDataSource.Instance.AddErrors(result);
            }
        }
    }
}
