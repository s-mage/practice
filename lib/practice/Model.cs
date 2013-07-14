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

        // Update field 'ready' to value state for given id of site.
        //
        private void SetSiteState(int siteId, string state) {
            string command = String.Format("ready = '{0}'", state);
            sites.Update(command).Where("id = " + siteId).All();
        }

        // Get the first row of table 'sites' where field 'ready' is state.
        //
        private NpgsqlDataReader GetSiteWithState(string state) {
            string command = String.Format("ready = '{0}'", state);
            Table result = sites.Select("*").Where(command).First().All();
            return result.data;
        }

        // Add new row to table 'reports' and link it with site.
        //
        public int NewReport(int siteId) {
            object result = reports.Insert(siteId.ToString(), "site_id").
                Returning("id");
            Console.WriteLine(result.GetType());
            return (int) result;
        }

        // Get first url that needs to be processed.
        //
        public NpgsqlDataReader GetUrl() {
            return GetSiteWithState("nothing");
        }

        // Get first url that already analyzed but report for what is
        // not generated.
        //
        public NpgsqlDataReader GetUrlForReport() {
            return GetSiteWithState("data");
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
            string condition = String.Format("name = '{0}'", rule);
            Table result = rules.Select("message").Where(condition).
                First().All();
            if (result.data.Read()) { return result.data.GetString(0); }
            return "";
        }


        // Put result of analysis of one subpage to table 'subpages'
        // and link it with report.
        //
        public void PutRules(int reportId, string url,
            Dictionary<string, bool> rules) {
            string values = String.Format("'{0}', '{1}', {2}",
                url, rules.ToJson(), reportId.ToString());
            subpages.Insert(values, "url, rules, report_id").All();
        }

        // Update field 'ready' to value 'data' for given id of site.
        //
        public void DataIsReady(int siteId) {
            SetSiteState(siteId, "data");
        }

        // Mark site with given id as processed.
        //
        public void MarkSiteProcessed(int siteId) {
            SetSiteState(siteId, "processing");
        }

        // Say that processing of site with given id was failed.
        //
        public void MarkSiteFailed(int siteId) {
            SetSiteState(siteId, "failed");
        }
    }
}