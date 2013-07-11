using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using ServiceStack.Text;

namespace Rooletochka {
    internal class Program {
        public static Model CreateModel() {
            string connect = "Server=127.0.0.1;Port=5432;User Id=s;Database=practice;";
            var connection = new NpgsqlConnection(connect);
            return new Model(connection);
        }

        // Let's test!
        //
        private static void Main(string[] args) {
            var model = CreateModel();

            NpgsqlDataReader urlRow = model.GetUrl();
            int siteId = urlRow.GetInt32(0);
            long reportId = model.NewReport(siteId);

            string url = urlRow.GetString(1);
            Console.WriteLine(url);

            Analyzer analyzer = new Analyzer(url);
            Report report = new Report(model, siteId);
            report = analyzer.Analyze(report.Id);
            Console.WriteLine(report.mainPageResult.ToJson());
            Console.WriteLine("Error404 " + report.Error404);
            Console.WriteLine("Robots " + report.RobotsTxt);
            report.PutIntoDB(model, siteId);
            Console.Read();
        }
    }
}
