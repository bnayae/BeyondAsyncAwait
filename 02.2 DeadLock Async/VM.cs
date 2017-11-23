using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace _02._2_DeadLock_Async
{
    public class VM : INotifyPropertyChanged
    {
        private const string URL = "http://lorempixel.com/1200/1200/animals/";

        public event PropertyChangedEventHandler PropertyChanged;

        public VM()
        {
            Process();
        }

        private void Process()
        {
            for (int i = 0; i < 10; i++)
            {
                Data = DownloadAsync(URL).Result; // deadlock
                //Data = await DownloadAsync(URL);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));

            }
        }

        private async Task<byte[]> DownloadAsync(string url)
        {
            using (var http = new HttpClient())
            {
                //byte[] data = http.GetByteArrayAsync(url).Result; // freeze
                byte[] data = await http.GetByteArrayAsync(url);
                return data;
            }
        }

        public byte[] Data { get; set; }
    }
}
