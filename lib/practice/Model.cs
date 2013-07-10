using System;
using Npgsql;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using ServiceStack.Text;

namespace Rooletochka
{
    class Model
    {
        private Table users;
        private Table sites;
        private Table subpages;
        private Table reports;

        public Model(NpgsqlConnection connection)
        {
            if(connection.State == ConnectionState.Closed) {
                connection.Open();
            }
            users = new Table("users", connection);
            sites = new Table("sites", connection);
            subpages = new Table("subpages", connection);
            reports = new Table("reports", connection);
        }

        // Add new row to table 'reports' and link it with site.
        //
        public long NewReport(int site_id)
        {
            reports.Insert(site_id.ToString(), "site_id");
            Table result = reports.Select("lastval()").All();
            result.data.Read();
            return result.data.GetInt64(0);
        }

        // Get first url that needs to be processed.
        //
        public NpgsqlDataReader GetUrl()
        {
            Table result = sites.Select("*").Where("ready = 'nothing'").
                First().All();
            result.data.Read();
            return result.data;
        }

        // Get first url that already analyzed but report for what is
        // not generated.
        //
        public NpgsqlDataReader GetUrlForReport()
        {
            Table result = sites.Select("*").Where("ready = 'data'").
                First().All();
            result.data.Read();
            return result.data;
        }


        // Put result of analysis of one subpage to table 'subpages'
        // and link it with report.
        //
        public void PutRules(int reportId, string url,
            Dictionary<string, bool> rules)
        {
            string values = String.Format("'{0}', '{1}', {2}",
                url, rules.ToJson(), reportId.ToString());
            subpages.Insert(values, "url, rules, report_id");
        }
    }
}
