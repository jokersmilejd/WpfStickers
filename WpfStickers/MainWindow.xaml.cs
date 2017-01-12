using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
using Newtonsoft.Json;
using System.ComponentModel;

namespace WpfStickers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CurrentItems items;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ShowButton_Click(object sender, RoutedEventArgs e)
        {
            showButton.IsEnabled = false;
            testProgressBar.Minimum = 0;
            testProgressBar.Maximum = 100;


            using (var wc = new WebClient())
            {
                string jsonItems = await wc.DownloadStringTaskAsync(new Uri("https://market.csgo.com/itemdb/current_730.json"));
                items = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<CurrentItems>(jsonItems));

                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_DownloadFileCompletedAsync;

                try
                {
                    wc.DownloadFileAsync(new Uri($"https://market.csgo.com/itemdb/{items.Db}"), $"c:/Users/UBI-Note/Downloads/{items.Db}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MessageBox.Show(items.Db);
                }

                //labelSpeed.Content = "Идет парсинг.";
                //await items.ParceCsv(items.Db);
                //labelSpeed.Content = "Парсинг завершен.";
            }
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();

            testProgressBar.Value = e.ProgressPercentage;
            labelPercent.Content = e.ProgressPercentage.ToString() + "%";
            labelSpeed.Content = "Идет загрузка...";
            labelDownloaded.Content = string.Format("{0} MB's / {1} MB's",
                (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));
        }

        private async void Wc_DownloadFileCompletedAsync(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            showButton.IsEnabled = true;
            testProgressBar.Value = 0;
            labelSpeed.Content = "Загрузка завершена.";
            labelPercent.Content = null;

            //labelSpeed.Content = "Идет парсинг.";
            //await items.ParceCsv(items.Db);
            //labelSpeed.Content = "Парсинг завершен.";
            BackgroundWorker bw = new BackgroundWorker();
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.DoWork += Bw_DoWork;
        }

        private async void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //0throw new NotImplementedException();
            await items.ParceCsv(items.Db);
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
            testProgressBar.Value = e.ProgressPercentage;
            labelPercent.Content = e.ProgressPercentage.ToString() + "%";
        }

        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            if (testLabel.Content.Equals(""))
            {
                testLabel.Content = "Magic!";
            }
            else
            {
                testLabel.Content = "";
            }
        }

        
    }
}
