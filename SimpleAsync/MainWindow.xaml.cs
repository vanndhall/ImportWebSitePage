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
using System.Diagnostics;
using Processor;
using SimpleAsync.Models;
using System.Threading;

namespace SimpleAsync
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        static AutoResetEvent _restart = new AutoResetEvent(false);
        public MainWindow()
        {
            InitializeComponent();
        }

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            resultsWindow.Text = "";
            var watch = Stopwatch.StartNew();
            MainMethods.RunDownloadSync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"\nTotal execution time: {elapsedMs}";

        }
       
        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            resultsWindow.Text = "";
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress; 
            var watch = Stopwatch.StartNew();
            //await RunDownloadASync();       // wolniejsze ale po kolei wysweitla wyniki
            _restart.Set();
            try
            {

                var results = await MainMethods.RunDownloadASync(progress, cts.Token); //szybsze ale wyswietla wszystkie wyniki na raz
                PrintResults(results);
            }
            catch (OperationCanceledException)
            {
                resultsWindow.Text += $"\nThe async download was cancelled {Environment.NewLine}";
               
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"\nTotal execution time: {elapsedMs}";
        }

        private void ReportProgress(object sender, ProgressReportModel e)
        {
            dashboardProgress.Value = e.PercentageComplete;
            PrintResults(e.SitesDownloaded);
        }

        private async void executeParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            resultsWindow.Text = "";
            var watch = Stopwatch.StartNew();

            var results = await MainMethods.RunDownloadParallelASync();
            PrintResults(results);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"\nTotal execution time: {elapsedMs}";

        }

        private void cancelOperation_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            
        }

        private void PrintResults(List<WebsiteDataModel> results)
        {
            resultsWindow.Text = "";
            foreach (var item in results)
            {
                resultsWindow.Text += $"\n{item.WebsiteUrl} downloaded: {item.WebsiteData.Length} characters!";
            }
        }


       

        
    }
}
