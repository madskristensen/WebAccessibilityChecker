using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

namespace WebAccessibilityChecker
{
    class TableDataSource : ITableDataSource
    {
        private static TableDataSource _instance;
        private readonly List<SinkManager> _managers = new List<SinkManager>();
        private static Dictionary<string, TableEntriesSnapshot> _snapshots = new Dictionary<string, TableEntriesSnapshot>();

        [Import]
        private ITableManagerProvider TableManagerProvider { get; set; } = null;

        private TableDataSource()
        {
            var compositionService = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            compositionService.DefaultCompositionService.SatisfyImportsOnce(this);

            var manager = TableManagerProvider.GetTableManager(StandardTables.ErrorsTable);
            manager.AddSource(this, StandardTableColumnDefinitions.DetailsExpander, StandardTableColumnDefinitions.DocumentName,
                                     StandardTableColumnDefinitions.ErrorSeverity, StandardTableColumnDefinitions.ErrorCode,
                                     StandardTableColumnDefinitions.ErrorSource, StandardTableColumnDefinitions.BuildTool,
                                     StandardTableColumnDefinitions.ErrorCategory, StandardTableColumnDefinitions.Text,
                                     StandardTableColumnDefinitions.Line, StandardTableColumnDefinitions.Column);
        }

        public static TableDataSource Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TableDataSource();

                return _instance;
            }
        }

        #region ITableDataSource members
        public string SourceTypeIdentifier
        {
            get { return StandardTableDataSources.ErrorTableDataSource; }
        }

        public string Identifier
        {
            get { return PackageGuids.guidPackageString; }
        }

        public string DisplayName
        {
            get { return Vsix.Name; }
        }

        public IDisposable Subscribe(ITableDataSink sink)
        {
            return new SinkManager(this, sink);
        }
        #endregion

        public void AddSinkManager(SinkManager manager)
        {
            // This call can, in theory, happen from any thread so be appropriately thread safe.
            // In practice, it will probably be called only once from the UI thread (by the error list tool window).
            lock (_managers)
            {
                _managers.Add(manager);
            }
        }

        public void RemoveSinkManager(SinkManager manager)
        {
            // This call can, in theory, happen from any thread so be appropriately thread safe.
            // In practice, it will probably be called only once from the UI thread (by the error list tool window).
            lock (_managers)
            {
                _managers.Remove(manager);
            }
        }

        public void UpdateAllSinks()
        {
            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.UpdateSink(_snapshots.Values);
                }
            }
        }

        public void AddErrors(IEnumerable<Rule> errors)
        {
            if (errors == null || !errors.Any())
                return;

            var cleanErrors = errors;//.Where(e => e != null && !string.IsNullOrEmpty(e.FileName));

            foreach (var error in cleanErrors.GroupBy(t => t.Id))
            {
                var snapshot = new TableEntriesSnapshot(error.Key, error);
                _snapshots[error.Key] = snapshot;
            }

            UpdateAllSinks();
        }

        //public void CleanErrors(IEnumerable<string> ruleIds)
        //{
        //    foreach (string id in ruleIds)
        //    {
        //        if (_snapshots.ContainsKey(id))
        //        {
        //            _snapshots[id].Dispose();
        //            _snapshots.Remove(id);
        //        }
        //    }

        //    lock (_managers)
        //    {
        //        foreach (var manager in _managers)
        //        {
        //            manager.RemoveSnapshots(ruleIds);
        //        }
        //    }

        //    UpdateAllSinks();
        //}

        public void CleanAllErrors()
        {
            foreach (string ruleId in _snapshots.Keys)
            {
                var snapshot = _snapshots[ruleId];
                if (snapshot != null)
                {
                    snapshot.Dispose();
                }
            }

            _snapshots.Clear();

            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.Clear();
                }
            }
        }

        public void BringToFront()
        {
            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            dte.ExecuteCommand("View.ErrorList");
        }

        public bool HasErrors()
        {
            return _snapshots.Count > 0;
        }

        public bool HasErrors(string fileName)
        {
            return _snapshots.ContainsKey(fileName);
        }
    }
}
