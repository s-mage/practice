using System;
using Npgsql;
using System.Data;
using System.Collections;

namespace Rooletochka
{
    class Model
    {
        private Table users;
        private Table sites;
        private Table subpages;

        public Model(NpgsqlConnection connection)
        {
            if(connection.State == ConnectionState.Closed) { connection.Open(); }
            users = new Table("users", connection);
            sites = new Table("sites", connection);
            subpages = new Table("subpages", connection);
        }

        public NpgsqlDataReader GetUrl()
        {
            sites.Select("*").Where("ready = 'nothing'").First().All();
            users.data.Read();
            return users.data;
        }

        public NpgsqlDataReader GetUrlForReport()
        {
            sites.Select("*").Where("ready = 'data'").First().All();
            users.data.Read();
            return users.data;
        }
        
        
        public void PutRules(int reportId, string url, HashTable rules)
        {
            string hash = "";
            foreach(DictionaryEntry item in rules) {
                hash += String.Format("{0} => {1}", 
                    item.Key.ToString(), item.Value.ToString());
            }
            string values = String.Format("'{0}', '{1}', {2}",
                url, hash, reportId.ToString());
            subpages.Insert(values, "url, rules, report_id");
        }
    }
}
