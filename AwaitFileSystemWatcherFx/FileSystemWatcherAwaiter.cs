using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using static System.String;

namespace Bnaya.Samples
{
    public class FileSystemWatcherAwaiter : INotifyCompletion
    {
        private string _result;
        private readonly FileSystemWatcher _fsw;
        private Action _continuation;
        private Exception _error;

        public FileSystemWatcherAwaiter(
            FileSystemWatcher fsw)
        {
            _fsw = fsw;
            //_fsw.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
            //_fsw.IncludeSubdirectories = true;
        }

        public bool IsCompleted => !IsNullOrEmpty(_result);

        public string GetResult()
        {
            _fsw.Changed -= OnChanged;
            _fsw.Deleted -= OnChanged;
            _fsw.Created -= OnChanged;
            _fsw.Renamed -= OnChanged;
            _fsw.Error -= OnError;

            if (_error != null)
                throw _error;
            return _result;
        }

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
            _fsw.Changed += OnChanged;
            _fsw.Deleted += OnChanged;
            _fsw.Created += OnChanged;
            _fsw.Renamed += OnChanged;
            _fsw.Error += OnError;
            _fsw.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _result = $"{e.Name}: {e.ChangeType}";
            // signal the await to call GetResult and 
            // schedule the next state on the async state machine
            _continuation();
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            if (IsNullOrEmpty(_result))
                _error = e.GetException();
            _continuation();
        }
    }
}
