using System;
using System.Data;
using Npgsql;

// Rooletochka is just like Rooletka, but a little smaller.
//
namespace Rooletochka
{
    public enum QueryType { Insert, Select, Update, Delete };

    // Implements table of database, because writing sql commands
    // is not good idea.
    //
    // The idea of the class is the following. It would be good
    // to have an object-oriented implementation of all this sql-
    // commands. As it's known, result of any sql-query is table.
    // It looks evident to implement class Table and some common
    // methods, such as select and insert.
    //
    public class Table
    {
        private string command;
        private NpgsqlConnection connection;
        private QueryType type;
        public NpgsqlDataReader data;

        public Table(string com, NpgsqlConnection con, QueryType t = QueryType.Select)
        {
            command = com;
            connection = con;
            type = t;
        }

        public Table(string com)
        {
            Console.Write("User Id: ");
            string userId = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            string connect = String.Format(@"Server=127.0.0.1;Port=5432;
                User Id={0};Password={1};Database=practice;", userId, password);
            connection = new NpgsqlConnection(connect);

            command = com;
        }

        // Initialize data field. It should be initialized once, 
        // or it would be slow as I don't know what.
        //
        public Table All()
        {
            var query = new NpgsqlCommand(command, connection);

            switch(type) {
                case QueryType.Select:
                    data = query.ExecuteReader();
                    break;
                case QueryType.Update:
                    query.ExecuteNonQuery();
                    break;
            }

            return this;
        }

        // Select operator, you can write something like
        // myAwesomeTable.Select("*").All() instead of
        // NpgsqlCommand("select * from myAwesomeTable", connection).
        //
        public Table Select(string query)
        {
            string result = String.Format("select {0} from {1}",
                query, command);
            return new Table(result, connection);
        }

        public Table Update(string query)
        {
            string result = String.Format("update {0} set {1}",
                    command, query);
            return new Table(result, connection, QueryType.Update);
        }

        public Table Where(string statement)
        {
            string result = String.Format("{0} where ({1})",
                command, statement);
            return new Table(result, connection);
        }

        public Table Limit(int limit)
        {
            string result = String.Format("{0} limit {1}",
                    command, limit.ToString());
            return new Table(result, connection);
        }

        public Table First()
        {
            return Limit(1);
        }

        public void Insert(string values, string fields = "")
        {
            string query;

            if(fields == "") {
                query = String.Format(@"insert into {0}
                    values ({1})", command, values);
            } else {
                query = String.Format(@"insert into {0} ({1})
                    values ({2})", command, fields, values);
            }
            NpgsqlCommand insertCommand = new NpgsqlCommand(query, connection);
            insertCommand.ExecuteNonQuery();
        }
    }
}
