using System.Collections.Generic;
using System.Linq;

namespace WebAccessibilityChecker
{
    class ErrorListService
    {
        public static void ProcessLintingResults(AccessibilityResult result, bool showErrorList)
        {
            TableDataSource.Instance.CleanAllErrors();

            if (result.Violations.Any())
            {
                TableDataSource.Instance.AddErrors(result.Violations);

                if (showErrorList)
                    TableDataSource.Instance.BringToFront();
            }
        }
    }
}
