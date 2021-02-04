using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SimpleAsync;
using SimpleAsync.Models;
using System.Threading;

namespace Processor
{
    public static class MainMethods
    {
        public static List<string> PrepSampleData()
        {
            List<string> output = new List<string>();
          
            output.Add("https://www.yahoo.com");
            output.Add("https://www.google.com");
            output.Add("https://www.microsoft.com");
            output.Add("https://www.wp.com");
            output.Add("https://www.codeproject.com");
            output.Add("https://www.stackoverflow.com");
           

            return output;
        }

        public static List<WebsiteDataModel> RunDownloadSync()
        {
            List<string> websites = PrepSampleData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();

            foreach (string site in websites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
                output.Add(results);
            }

            return output;
        }
        // działa tak szybko jak metoda Sync() - podobne czasy
        public static async Task<List<WebsiteDataModel>> RunDownloadASync(IProgress<ProgressReportModel> progress,CancellationToken cancellationToken)
        {
            List<string> websites = PrepSampleData();
            List<WebsiteDataModel> output = new List<WebsiteDataModel>();
            ProgressReportModel report = new ProgressReportModel();

            foreach (string site in websites)
            {
                WebsiteDataModel results = await DownloadWebsiteAsync(site);
                output.Add(results);
                
                cancellationToken.ThrowIfCancellationRequested(); // anulowanie zadania za pomocą cancelationToken i metody ThrowIfCancellationRequested przez klikniecie Cancel.
                report.SitesDownloaded = output;
                report.PercentageComplete = (output.Count * 100) / websites.Count;
                progress.Report(report);
            }
            return output;
        }

        //bardziej zoptymalizowana metoda Async która jest 3 razy szybsza od Sync()
        public static async Task<List<WebsiteDataModel>> RunDownloadParallelASync()
        {
            List<string> websites = PrepSampleData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();
            foreach (string site in websites)
            {
                // tasks.Add(Task.Run(() => DownloadWebsite(site))); //wolniejsze metoda DownloadWebsite bez Async
                tasks.Add(DownloadWebsiteAsync(site)); //szybsze metoda DownloadWebsite z Async - niepotrzebny Task.Run
            }

            var results = await Task.WhenAll(tasks);

            return new List<WebsiteDataModel>(results);
        }

        public static WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteURL;
            output.WebsiteData = client.DownloadString(websiteURL);
            return output;
        }

        public static async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteURL;
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteURL);
            return output;
        }

    }
}
