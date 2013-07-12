using System;
using System.Data;
using System.Collections.Generic;
using ServiceStack.Text;
using Npgsql;

namespace Rooletochka {
    public class Model {
        private Table users;
        private Table sites;
        private Table subpages;
        private Table reports;
        private Table rules;

        public Model(NpgsqlConnection connection) {
            if(connection.State == ConnectionState.Closed) {
                connection.Open();
            }
            users = new Table("users", connection);
            sites = new Table("sites", connection);
            subpages = new Table("subpages", connection);
            reports = new Table("reports", connection);
            rules = new Table("rules", connection);
        }

        // Add new row to table 'reports' and link it with site.
        //
        public long NewReport(int siteId) {
            reports.Insert(siteId.ToString(), "site_id");
            Table result = reports.Select("lastval()").All();
            result.data.Read();
            return result.data.GetInt64(0);
        }

        // Get first url that needs to be processed.
        //
        public NpgsqlDataReader GetUrl() {
            Table result = sites.Select("*").Where("ready = 'nothing'").
                First().All();
            result.data.Read();
            return result.data;
        }

        // Get first url that already analyzed but report for what is
        // not generated.
        //
        public NpgsqlDataReader GetUrlForReport() {
            Table result = sites.Select("*").Where("ready = 'data'").
                First().All();
            result.data.Read();
            return result.data;
        }

        // Get last report id given site id.
        //
        public int GetReportId(int siteId) {
            string condition = "site_id  = " + siteId;
            Table result = reports.Select("id").Where(condition).Order("-id").
                First().All();
            if (result.data.Read()) { return result.data.GetInt32(0); }
            return 0;
        }

        // Get URLs and rules for subpages linked with given reportId.
        //
        public NpgsqlDataReader GetSubpages(int reportId) {
            string condition = "report_id = " + reportId;
            return subpages.Select("url, rules").Where(condition).All().data;
        }

        // Get explanation of rule given it name.
        //
        public string Explain(string rule) {
            Table result = rules.Select("message").Where("name = " + rule).
                First().All();
            if (result.data.Read()) { return result.data.GetString(0); }
            return "";
        }


        // Put result of analysis of one subpage to table 'subpages'
        // and link it with report.
        //
        public void PutRules(long reportId, string url,
            Dictionary<string, bool> rules) {
            string values = String.Format("'{0}', '{1}', {2}",
                url, rules.ToJson(), reportId.ToString());
            subpages.Insert(values, "url, rules, report_id");
        }

        // Update field 'ready' to value 'data' for given id of site.
        //
        public void DataIsReady(int siteId) {
            sites.Update("ready = 'data'").Where("id = " + siteId).All();
        }
    }
}