using System;
using Npgsql;
using ServiceStack.Text;
using Features=System.Collections.Generic.Dictionary<string, bool>;

namespace Rooletochka {
    internal class Program {
        public static Model CreateModel() {
            string connect = @"Server=127.0.0.1;Port=5432;User Id=s;
                Database=practice;";
            // Connection string for windows.
            // string connect = @"Server=127.0.0.1;Port=5432;User Id=postgres;
            //    Password=1111;Database=test_db;Preload Reader = true;";
            var connection = new NpgsqlConnection(connect);
            return new Model(connection);
        }

        // Example: analyze one url from database.
        //
        public static void Analyze(Model model) {
            try {
                NpgsqlDataReader urlRow = model.GetUrl();
                int siteId = urlRow.GetInt32(0);

                string url = urlRow.GetString(1);
                Console.WriteLine(url);

                Analyzer analyzer = new Analyzer(url);
                Report report = new Report(model, siteId);
                report = analyzer.Analyze(report.Id);
                report.PutIntoDB(model, siteId);
            }
            catch (InvalidOperationException ex) {
                Console.WriteLine("Analyze Error: {0}", ex.Message);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        // Example: grab information for .pdf generation from database.
        //
        public static void WriteReadyData(Model model) {
            NpgsqlDataReader urlRow = model.GetUrlForReport();
            int siteId = urlRow.GetInt32(0);
            int reportId = model.GetReportId(siteId);
            NpgsqlDataReader subpages = model.GetSubpages(reportId);
            while (subpages.Read()) {
                string featuresAddress = subpages.GetString(0);
                Console.WriteLine(featuresAddress);

                Features rules = subpages.GetString(1).FromJson<Features>();
                foreach (var rule in rules) {
                    Console.WriteLine(rule.Key + " = " + rule.Value);
                    if (rule.Value) {
                        Console.WriteLine(model.Explain(rule.Key));
                    }
                }
            }
        }

        // Let's test!
        //
        private static void Main(string[] args) {
            var model = CreateModel();
            Analyze(model);

            //WriteReadyData(model);
            Console.Read();
        }
    }
}