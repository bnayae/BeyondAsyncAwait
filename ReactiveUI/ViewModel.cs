using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI
{
    internal class ViewModel : INotifyPropertyChanged
    {
        //private readonly TaskScheduler _uiSync = TaskScheduler.FromCurrentSynchronizationContext();

        public Command Click => new Command(StartProgress);

        public ObservableCollection<string> Items { get; } = new ObservableCollection<string>();
        private int _progress;
        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
            }
        }


        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void StartProgress()
        {
            //Task t = DoProcess();
            //t.ContinueWith(c =>
            //{
            //    Message = "Done";
            //    Items.Insert(0, "Done");
            //}, _uiSync);
            //Message = "I'm Out";

            //Task fireForget = ExeuteAsync();
            Task fireForget = ExeuteLocalAsync();
            async Task ExeuteLocalAsync()
            {
                await DoProcess();
                Message = "Done";
                Items.Insert(0, "Done");
            }
        }

        //private async Task ExeuteAsync()
        //{
        //    await DoProcess();
        //    Message = "Done";
        //    Items.Insert(0, "Done");
        //}

        private async Task DoProcess()
        {
            for (int i = 0; i < 100; i += 5)
            {
                Progress = i;
                Items.Add(i.ToString());
                //Thread.Sleep(300);
                await Task.Delay(200);//.ConfigureAwait(false);
            }
        }
    }
}
