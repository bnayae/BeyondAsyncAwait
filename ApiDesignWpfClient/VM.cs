using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;


namespace ApiDesignWpfClient
{
    public class VM : PropertyChangedBase
    {
        private static readonly string[] TOPICS = { "Coin", "Kid", "Face", "Stream", "Travel", "Mountain", "Sea", "Home", "ant", "butterfly", "dog", "cat", "elephant", "wild", "woman", "man", "flower", "moon", "sport", "food" };
        private const string URL_PATTERN = "http://localhost:56368/api/design{0}/async/{1}/{2}";
        private IProgress<byte[]> _imageUpdater;
        private IProgress<TimeSpan> _durationUpdater;

        #region Ctor

        public VM()
        {
            _imageUpdater = new Progress<byte[]>(img => Images.Add(img));
            _durationUpdater = new Progress<TimeSpan>(time => Durations.Insert(0, time));
        }

        #endregion // Ctor

        #region Size

        private int _size = 1000;
        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                base.NotifyOfPropertyChange();
            }
        }

        #endregion // Size

        #region Multiply

        private int _multiply = 3;

        public int Multiply
        {
            get { return _multiply; }
            set
            {
                _multiply = value;
                base.NotifyOfPropertyChange();
                base.NotifyOfPropertyChange(nameof(TotalWork));
            }
        }

        #endregion // Multiply

        #region Bad

        private bool _bad = true;
        public bool Bad
        {
            get { return _bad; }
            set
            {
                _bad = value;
                base.NotifyOfPropertyChange();
            }
        }

        #endregion // Bad

        #region Ok

        private bool _ok;
        public bool Ok
        {
            get { return _ok; }
            set
            {
                _ok = value;
                base.NotifyOfPropertyChange();
            }
        }

        #endregion // Ok

        #region Better

        private bool _better;
        public bool Better
        {
            get { return _better; }
            set
            {
                _better = value;
                base.NotifyOfPropertyChange();
            }
        }

        #endregion // Better

        #region Started

        private int _started;
        public int Started
        {
            get { return _started; }
            set
            {
                _started = value;
                base.NotifyOfPropertyChange();
            }
        }

        #endregion // Started

        #region Completed

        private int _completed;
        public int Completed
        {
            get { return _completed; }
            set
            {
                _completed = value;
                base.NotifyOfPropertyChange();
            }
        }

        #endregion // Completed

        #region TotalWork

        public int TotalWork => TOPICS.Length * Multiply;

        #endregion // TotalWork

        #region Images

        public ObservableCollection<byte[]> Images { get; set; } = new ObservableCollection<byte[]>();

        #endregion // Images

        #region Images

        public ObservableCollection<TimeSpan> Durations { get; set; } = new ObservableCollection<TimeSpan>();

        #endregion // Images

        #region GetRouting

        private string GetRouting()
        {
            if (Bad)
                return "bad";
            if (Ok)
                return "ok";
            return "better";
        }

        #endregion // GetRouting

        #region Start / Async

        public void Start()
        {
            Task _ = StartAsync();

            Task StartAsync()
            {
                Images.Clear();
                Durations.Clear();
                Started = 0;
                Completed = 0;
                var sw = Stopwatch.StartNew();
                var tasks =
                            from i in Enumerable.Range(0, Multiply) // just multiply the calls (like external loop)
                            from topic in TOPICS
                            let route = GetRouting()
                            let url = string.Format(URL_PATTERN, route, Size, topic)
                            select DownloadImageAsync(url, sw);
                return Task.WhenAll(tasks);
            }
        }

        #endregion // Start / Async

        #region DownloadImageAsync

        private async Task DownloadImageAsync(string url, Stopwatch sw)
        {
            Started++; // still on the UI thread (single thread execution)

            using (var http = new HttpClient())
            {
                var response = await http.GetAsync(url).ConfigureAwait(false);
                byte[] data = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                _durationUpdater.Report(sw.Elapsed);
                _imageUpdater.Report(data);
            }

            Interlocked.Increment(ref _completed); // not on the UI thread
            base.NotifyOfPropertyChange(nameof(Completed));
        }

        #endregion // DownloadImageAsync

    }
}
