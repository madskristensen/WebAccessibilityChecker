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
        private readonly List<Rule> _errors = new List<Rule>();

        internal TableEntriesSnapshot(IEnumerable<Rule> errors)
        {
            _errors.AddRange(errors);
            _dte = (DTE2)Package.GetGlobalService(typeof(DTE));
        }

        public override int VersionNumber { get; } = 1;

        public override int Count
        {
            get { return _errors.Count; }
        }

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            content = null;

            if ((index >= 0) && (index < _errors.Count))
            {
                if (columnName == StandardTableKeyNames.DocumentName)
                {
                    content = _errors[index].FileName;
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
                    content = _errors[index].Line;
                }
                else if (columnName == StandardTableKeyNames.Column)
                {
                    content = _errors[index].Column;
                }
                else if (columnName == StandardTableKeyNames.Text)
                {
                    content = _errors[index].Help;
                }
                else if (columnName == StandardTableKeyNames.FullText || columnName == StandardTableKeyNames.Text)
                {
                    content = _errors[index].Help + "\r\n" +
                              _errors[index].Description + "\r\n\r\n" +
                              "Tags: " + string.Join(", ", _errors[index].Tags ?? new List<string>()) + "\r\n" +
                              "HTML: " +_errors[index].Html;
                }
                else if (columnName == StandardTableKeyNames.PriorityImage)
                {
                    content = KnownMonikers.Accessibility;
                }
                else if (columnName == StandardTableKeyNames.ErrorSeverity)
                {
                    content = _errors[index].GetSeverity();
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
                    content = _errors[index].Id;
                }
                else if (columnName == StandardTableKeyNames.ProjectName)
                {
                    if (string.IsNullOrEmpty(_projectName) && !string.IsNullOrEmpty(_errors[index].FileName))
                    {
                        var _item = _dte.Solution.FindProjectItem(_errors[index].FileName);

                        if (_item != null && _item.Properties != null && _item.ContainingProject != null)
                            _projectName = _item.ContainingProject.Name;
                    }

                    content = _projectName;
                }
                else if ((columnName == StandardTableKeyNames.ErrorCodeToolTip) || (columnName == StandardTableKeyNames.HelpLink))
                {
                    var error = _errors[index];
                    string url;

                    if (!string.IsNullOrEmpty(error.HelpUrl))
                    {
                        url = error.HelpUrl;
                    }
                    else
                    {
                        url = string.Format("http://www.bing.com/search?q={0} {1}", Vsix.Name, _errors[index].Id);
                    }

                    content = Uri.EscapeUriString(url);
                }
            }

            return content != null;
        }
    }
}
