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

namespace SimpleAsync
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = Stopwatch.StartNew();
            MainMethods.RunDownloadSync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"\nTotal execution time: {elapsedMs}";

        }
       
        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress; 
            var watch = Stopwatch.StartNew();
            //await RunDownloadASync();       // wolniejsze ale po kolei wysweitla wyniki
            await MainMethods.RunDownloadASync(progress); //szybsze ale wyswietla wszystkie wyniki na raz
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
            var watch = Stopwatch.StartNew();

            var results = await MainMethods.RunDownloadParallelASync();
            PrintResults(results);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execution time: {elapsedMs}";

        }

        private void cancelOperation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrintResults(List<WebsiteDataModel> results)
        {
            resultsWindow.Text = "";
            foreach (var item in results)
            {
                resultsWindow.Text += $"{item.WebsiteUrl} downloaded: {item.WebsiteData.Length} characters!";
            }
        }


       

        
    }
}
