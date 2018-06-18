using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Demystify_Configure_Await_False
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            a.Text = (SynchronizationContext.Current != null).ToString();
            Task _ = ExecAsync();
        }

        private async Task ExecAsync()
        {
            Task t = Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(500);
                    Trace.WriteLine((SynchronizationContext.Current != null).ToString());
                }
            });
            //.ConfigureAwait(false);

            ConfiguredTaskAwaitable c = t.ConfigureAwait(false);
            //await t;
            //Trace.WriteLine("End " + (SynchronizationContext.Current != null).ToString());
            await c;
            Trace.WriteLine("End " + (SynchronizationContext.Current != null).ToString());
        }
    }
}
