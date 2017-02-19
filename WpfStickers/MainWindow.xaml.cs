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
using MahApps.Metro.Controls;
using System.Threading;

namespace WpfStickers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        WebClient wc;
        CurrentItems items;
        string st = "items_730_1484574578.csv";
        CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ShowButton_Click(object sender, RoutedEventArgs e)
        {

            if (showButton.Content.Equals("Cancel"))
            {
                if (wc.IsBusy)
                {
                    wc.CancelAsync();
                }
                else
                {
                    //cts.Cancel();        
                }
            }
            else
            {
                showButton.Content = "Cancel";
                testProgressBar.Minimum = 0;
                testProgressBar.Maximum = 100;


                using (wc = new WebClient())
                {
                    try
                    {
                        string jsonItems = await wc.DownloadStringTaskAsync(new Uri("https://market.csgo.com/itemdb/current_730.json"));
                        items = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<CurrentItems>(jsonItems));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += Wc_DownloadFileCompletedAsync;

                    try
                    {
                        wc.DownloadFileAsync(new Uri($"https://market.csgo.com/itemdb/{items.Db}"), $"c:/Users/UBI-Note/Downloads/{items.Db}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
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

            if (e.Cancelled == false)
            {
                testProgressBar.Value = 0;
                labelSpeed.Content = "Загрузка завершена.";
                labelPercent.Content = null;

                labelSpeed.Content = "Идет парсинг.";
                testProgressBar.IsIndeterminate = true;
                //await items.ParceCsv(items.Db, cts.Token);
                testProgressBar.IsIndeterminate = false;
                labelSpeed.Content = "Парсинг завершен.";
                showButton.Content = "Download";
            }
            else
            {
                labelSpeed.Content = "Загрузка отменена.";
                showButton.Content = "Download";
            }

        }

        private async void testButton_Click(object sender, RoutedEventArgs e)
        {
            //if (testLabel.Content.Equals(""))
            //{
            //    testLabel.Content = "Magic!";
            //}
            //else
            //{
            //    testLabel.Content = "";
            //}

            if (!testButton.Content.Equals("Stop"))
            {
                cts = new CancellationTokenSource();
            }
            else
            {
                cts.Cancel();
            }

            testButton.Content = "Stop";

            using (wc = new WebClient())
            {
                    string jsonItems = await wc.DownloadStringTaskAsync(new Uri("https://market.csgo.com/itemdb/current_730.json"));
                    items = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<CurrentItems>(jsonItems));
                    items.Db = st;
            }

            labelSpeed.Content = "Идет парсинг.";
            testProgressBar.IsIndeterminate = true;

            try
            {
                await items.ParceCsv(st, cts.Token);
            }
            catch (OperationCanceledException)
            {
                labelSpeed.Content = "Парсинг отменен.";
            }
            catch (Exception)
            {
                labelSpeed.Content = "Ошибка при парсинге.";
            }
            finally
            {
                cts.Dispose();
            }

            testProgressBar.IsIndeterminate = false;
            //labelSpeed.Content = "Парсинг завершен.";

            testButton.Content = "Parse";
        }

        
    }
}
