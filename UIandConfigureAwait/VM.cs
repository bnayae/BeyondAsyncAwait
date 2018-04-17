using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Effects;
using SixLabors.ImageSharp.Processing.Filters;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UIandConfigureAwait
{
    public class VM : INotifyPropertyChanged
    {
        private const string URL_PATTERN = "https://source.unsplash.com/{0}x{0}/?{1}/";
        private const string URL_PATTERN_RND = "https://source.unsplash.com/random/{0}x{0}";
        private const string MEDIA_TYPE = "image/jpeg";

        public event PropertyChangedEventHandler PropertyChanged;
        private IProgress<string> _progress;
        private CancellationTokenSource _cancellation = new CancellationTokenSource();

        public VM()
        {
            Cancel = new MyCommand(_cancellation);
            _progress = new Progress<string>(data => Reports.Add(data));
            Task _ = GetMergeAsync(2000);
        }

        private byte[] _picture;

        public byte[] Picture
        {
            get { return _picture; }
            set
            {
                _picture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Picture)));
            }
        }

        private int _advance;

        public int Advance => _advance;

        public ObservableCollection<string> Reports { get; } = new ObservableCollection<string>();

        public ICommand Cancel { get; }

        #region GetMergeAsync

        private async Task GetMergeAsync(
            int size, string topic = null)
        {
            Task<Image<Rgba32>> t1 = GetWithFilterAsync(_cancellation.Token, size, topic);
            Task<Image<Rgba32>> t2 = GetWithFilterAsync(_cancellation.Token, size, topic);

            var results = await Task.WhenAll(t1, t2).ConfigureAwait(false);
            if (_cancellation.Token.IsCancellationRequested)
                return;
            using (Image<Rgba32> img0 = results[0])
            using (Image<Rgba32> img1 = results[1])
            using (var imageProcessor = new Image<Rgba32>(size * 2, size))
            using (var outStream = new MemoryStream())
            {
                imageProcessor.Mutate(x => x
                                    .DrawImage(img0, 1)
                                    .DrawImage(img1, 1, new Point(size, 0)));
                imageProcessor.SaveAsJpeg(outStream);
                byte[] manipedImage = outStream.ToArray();
                Picture = manipedImage;
            }
        }

        #endregion // GetMergeAsync

        #region GetWithFilterAsync

        public async Task<Image<Rgba32>> GetWithFilterAsync(
            CancellationToken ct,
            int size,
            string topic = null)
        {
            if (ct.IsCancellationRequested)
            {
                _progress.Report("Cancel");
                return null;
            }
            //Advance += 1;
            Interlocked.Increment(ref _advance);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Advance)));
            Trace.WriteLine("Start");
            _progress.Report("Start");
            string url;
            if (string.IsNullOrEmpty(topic))
                url = string.Format(URL_PATTERN_RND, size);
            else
                url = string.Format(URL_PATTERN, size, topic);

            using (var http = new HttpClient())
            {
                var image = await http.GetByteArrayAsync(url).ConfigureAwait(false);
                if (ct.IsCancellationRequested)
                {
                    _progress.Report("Cancel");
                    return null;
                }
                Trace.WriteLine("Processing");
                _progress.Report("Processing");

                Image<Rgba32> imageProcessor = Image.Load(image);
                imageProcessor.Mutate(x => x
                        .OilPaint()
                        .Grayscale());
                Trace.WriteLine("Processed");
                Interlocked.Increment(ref _advance);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Advance)));
                _progress.Report("Processed");
                return imageProcessor;
            }
        }

        #endregion // GetWithFilterAsync

        private class MyCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;
            private CancellationTokenSource _cts;

            public MyCommand(CancellationTokenSource  cts)
            {
                _cts = cts;
            }

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                //_cts.Token.Register(() ={ });
                _cts.Cancel();
            }
        }
    }
}
