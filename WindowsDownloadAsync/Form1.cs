using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsDownloadAsync
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _http = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void OnClick(object sender, EventArgs e)
        { // async void is not good practice
            try
            {
                Task<Image> t = Downloader.DownloadAsync();
                this.Text = "Loading...";
                //Image picture = t.Result;
                Image picture = await t;
                this.BackgroundImage = picture;
                this.Text = "Done";
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
    }
}
