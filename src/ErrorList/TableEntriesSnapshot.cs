using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.TableManager;


namespace WebAccessibilityChecker
{
    class TableEntriesSnapshot : TableEntriesSnapshotBase
    {
        private string _projectName;
        private DTE2 _dte;

        internal TableEntriesSnapshot(AccessibilityResult result)
        {
            Errors.AddRange(result.Violations);
            _dte = (DTE2)Package.GetGlobalService(typeof(DTE));
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

            if ((index >= 0) && (index < Errors.Count))
            {
                if (columnName == StandardTableKeyNames.DocumentName)
                {
                    content = Errors[index].FileName;
                }
                else if (columnName == StandardTableKeyNames.ErrorCategory)
                {
                    content = Vsix.Name;
                }
                else if (columnName == StandardTableKeyNames.ErrorSource)
                {
                    content = Vsix.Name;
                }
                else if (columnName == StandardTableKeyNames.Line)
                {
                    content = Errors[index].Line;
                }
                else if (columnName == StandardTableKeyNames.Column)
                {
                    content = Errors[index].Column;
                }
                else if (columnName == StandardTableKeyNames.Text)
                {
                    content = Errors[index].Help;
                }
                else if (columnName == StandardTableKeyNames.FullText || columnName == StandardTableKeyNames.Text)
                {
                    content = Errors[index].Description + "\r\n\r\n" +
                              "URL: " + Url + "\r\n" +
                              "HTML: " + Errors[index].Html;
                }
                else if (columnName == StandardTableKeyNames.PriorityImage)
                {
                    content = KnownMonikers.Accessibility;
                }
                else if (columnName == StandardTableKeyNames.ErrorSeverity)
                {
                    content = Errors[index].GetSeverity();
                }
                else if (columnName == StandardTableKeyNames.Priority)
                {
                    content = vsTaskPriority.vsTaskPriorityMedium;
                }
                else if (columnName == StandardTableKeyNames.ErrorSource)
                {
                    content = ErrorSource.Other;
                }
                else if (columnName == StandardTableKeyNames.BuildTool)
                {
                    content = Vsix.Name;
                }
                else if (columnName == StandardTableKeyNames.ErrorCode)
                {
                    content = Errors[index].Id;
                }
                else if (columnName == StandardTableKeyNames.ProjectName)
                {
                    if (string.IsNullOrEmpty(_projectName) && !string.IsNullOrEmpty(Errors[index].FileName))
                    {
                        var _item = _dte.Solution.FindProjectItem(Errors[index].FileName);

                        if (_item != null && _item.Properties != null && _item.ContainingProject != null)
                            _projectName = _item.ContainingProject.Name;
                    }

                    content = _projectName;
                }
                else if ((columnName == StandardTableKeyNames.ErrorCodeToolTip) || (columnName == StandardTableKeyNames.HelpLink))
                {
                    var error = Errors[index];
                    string url;

                    if (!string.IsNullOrEmpty(error.HelpUrl))
                    {
                        url = error.HelpUrl;
                    }
                    else
                    {
                        url = string.Format("http://www.bing.com/search?q={0} {1}", Vsix.Name, Errors[index].Id);
                    }

                    content = Uri.EscapeUriString(url);
                }
            }

            return content != null;
        }
    }
}
