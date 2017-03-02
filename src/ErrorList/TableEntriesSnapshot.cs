using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using System;
using System.Collections.Generic;

namespace WebAccessibilityChecker
{
    class TableEntriesSnapshot : WpfTableEntriesSnapshotBase
    {
        private string _projectName;
        private DTE2 _dte;

        internal TableEntriesSnapshot(AccessibilityResult result)
        {
            _dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            _projectName = result.Project;
            Errors.AddRange(result.Violations);
            Url = result.Url;
        }

        public List<Rule> Errors { get; } = new List<Rule>();

        public override int VersionNumber { get; } = 1;

        public override int Count
        {
            get { return Errors.Count; }
        }

        public string Url { get; set; }

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            content = null;

            if (index < 0 || index >= Errors.Count)
            {
                return false;
            }

            Rule error = Errors[index];

            switch (columnName)
            {
                case StandardTableKeyNames.DocumentName:
                    content = error.FileName;
                    return true;
                case StandardTableKeyNames.ErrorCategory:
                    content = vsTaskCategories.vsTaskCategoryMisc;
                    return true;
                case StandardTableKeyNames.BuildTool:
                    content = Vsix.Name;
                    return true;
                case StandardTableKeyNames.Line:
                    if (!string.IsNullOrEmpty(error.FileName))
                    {
                        content = error.Line;
                        return true;
                    }
                    return false;
                case StandardTableKeyNames.Column:
                    if (!string.IsNullOrEmpty(error.FileName))
                    {
                        content = error.Column;
                        return true;
                    }
                    return false;
                case StandardTableKeyNames.Text:
                    content = error.Description;
                    return true;
                case StandardTableKeyNames.PriorityImage:
                case StandardTableKeyNames.ErrorSeverityImage:
                    content = KnownMonikers.Accessibility;
                    return true;
                case StandardTableKeyNames.ErrorSeverity:
                    content = error.GetSeverity();
                    return true;
                case StandardTableKeyNames.Priority:
                    content = vsTaskPriority.vsTaskPriorityMedium;
                    return true;
                case StandardTableKeyNames.ErrorSource:
                    content = ErrorSource.Other;
                    return true;
                case StandardTableKeyNames.ErrorCode:
                    content = error.Id;
                    return true;
                case StandardTableKeyNames.ProjectName:
                    content = _projectName;
                    return true;
                case StandardTableKeyNames.ErrorCodeToolTip:
                    content = error.Help;
                    return true;
                case StandardTableKeyNames.HelpLink:
                    content = Uri.EscapeUriString(error.HelpUrl);
                    return true;
                default:
                    content = null;
                    return false;
            }
        }

        public override bool CanCreateDetailsContent(int index)
        {
            return !string.IsNullOrEmpty(Errors[index].Html);
        }

        public override bool TryCreateDetailsStringContent(int index, out string content)
        {
            var error = Errors[index];
            content = $"Impact: {error.Impact}\r\nURL: {Url}\r\nHTML: {error.Html}";
            return true;
        }
    }
}
