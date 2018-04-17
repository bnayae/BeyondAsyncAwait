using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.String;
#pragma warning disable SG0018 // Path traversal

namespace Bnaya.Samples
{
    public class FileSystemWatcherAwaiter : INotifyCompletion
    {
        private string _result;
        private readonly FileSystemWatcher _fsw;
        private Action _continuation;
        private Exception _error;

        #region Ctor

        public FileSystemWatcherAwaiter(
            FileSystemWatcher fsw)
        {
            _fsw = fsw;
            //_fsw.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
            //_fsw.IncludeSubdirectories = true;
        }

        #endregion // Ctor

        #region IsCompleted

        /// <summary>
        /// Gets a value indicating whether this instance is completed.
        /// use for optimization
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompleted => !IsNullOrEmpty(_result);

        #endregion // IsCompleted

        #region OnCompleted

        /// <summary>
        /// called when awaiting 
        /// Schedules the continuation  action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">
        /// The trigger for complete (call for it, ends the awaiting)
        /// </param>
        public void OnCompleted(Action continuation) // called when awaiting
        {
            _continuation = continuation; // keep the completion trigger
            _fsw.EnableRaisingEvents = true;
            _fsw.Changed += OnChanged;
            _fsw.Deleted += OnChanged;
            _fsw.Created += OnChanged;
            _fsw.Renamed += OnChanged;
            _fsw.Error += OnError;
        }

        #endregion // OnCompleted

        #region GetResult

        /// <summary>
        /// Called after completion to fetch the result.
        /// </summary>
        /// <returns></returns>
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

        #endregion // GetResult

        #region OnChanged

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Task fireForget = OnChangedAsync();

            async Task OnChangedAsync() // local function (.NET 4.7)
            {
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Changed:
                        string content = null;
                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {
                                content = File.ReadAllText(e.FullPath);
                                break;
                            }
                            catch (IOException) // temporary locked sometimes
                            {
                                await Task.Delay(100).ConfigureAwait(false);
                            }
                        }

                        _result = $@"##  {e.Name}: {e.ChangeType} ##
{content}";
                        break;

                    case WatcherChangeTypes.Created:
                    case WatcherChangeTypes.Deleted:
                    case WatcherChangeTypes.Renamed:
                    default:
                        {
                            _result = $"{e.Name}: {e.ChangeType}";
                            break;
                        }
                }
                // signal the await to call GetResult and 
                // schedule the next state on the async state machine
                _continuation();
            }
        }

        #endregion // OnChanged

        #region OnError

        private void OnError(object sender, ErrorEventArgs e)
        {
            if (IsNullOrEmpty(_result))
                _error = e.GetException();
            _continuation();
        }

        #endregion // OnError
    }
}
