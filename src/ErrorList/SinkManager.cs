using Microsoft.VisualStudio.Shell.TableManager;
using System.Linq;
using System;
using System.Collections.Generic;

namespace WebAccessibilityChecker
{
    class SinkManager : IDisposable
    {
        private readonly ITableDataSink _sink;
        private TableDataSource _errorList;
        private TableEntriesSnapshot _snapshot;

        internal SinkManager(TableDataSource errorList, ITableDataSink sink)
        {
            _sink = sink;
            _errorList = errorList;

            errorList.AddSinkManager(this);
        }

        internal void Clear()
        {
            _sink.RemoveAllSnapshots();
        }

        internal void UpdateSink(TableEntriesSnapshot snapshot)
        {
            if (_snapshot != null)
            {
                _sink.ReplaceSnapshot(_snapshot, snapshot);
            }
            else
            {
                _sink.AddSnapshot(snapshot);
            }

            _snapshot = snapshot;
        }
        
        public void Dispose()
        {
            // Called when the person who subscribed to the data source disposes of the cookie (== this object) they were given.
            _errorList.RemoveSinkManager(this);
        }
    }
}
